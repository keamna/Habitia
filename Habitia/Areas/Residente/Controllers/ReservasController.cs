using Habitia.Data;
using Habitia.Enums;
using Habitia.Helpers;
using Habitia.Models;
using Habitia.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Residente.Controllers
{
    [Area("Residente")]
    [Authorize(Roles = "Residente")]
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ================= Listado de áreas comunes activas =================
        public async Task<IActionResult> Index()
        {
            var areas = await _context.AreasComunes
                .Include(a => a.Fotos)
                .Where(a => a.TB_Estado)
                .ToListAsync();

            return View(areas);
        }

        // ================= Detalle + horarios disponibles =================
        public async Task<IActionResult> Detalle(int id)
        {
            var area = await _context.AreasComunes
                .Include(a => a.Fotos)
                .FirstOrDefaultAsync(a => a.TN_Id == id && a.TB_Estado);

            if (area == null)
                return NotFound();

            var horarios = await _context.Disponibilidades
                .Where(d => d.TN_IdAreaComun == id
                    && d.TB_Estado
                    && !d.TB_Reservado
                    && d.TF_Fecha.Date >= DateTime.Today)
                .OrderBy(d => d.TF_Fecha)
                .ThenBy(d => d.TF_HoraInicio)
                .ToListAsync();

            // Descarta horarios de hoy que ya iniciaron
            horarios = horarios
                .Where(d => d.TF_Fecha.Date > DateTime.Today
                    || d.TF_Fecha.Date.Add(d.TF_HoraInicio) > DateTime.Now)
                .ToList();

            ViewBag.Horarios = horarios;
            return View(area);
        }

        // ================= Confirmar reserva =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reservar(ReservaFormVM vm)
        {
            var disponibilidad = await _context.Disponibilidades
                .Include(d => d.AreaComun)
                .FirstOrDefaultAsync(d => d.TN_Id == vm.IdDisponibilidad);

            if (disponibilidad == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "El motivo de la reserva es obligatorio.";
                return RedirectToAction(nameof(Detalle), new { id = disponibilidad.TN_IdAreaComun });
            }

            // Revalidación de disponibilidad al momento de confirmar (2.10)
            var inicio = disponibilidad.TF_Fecha.Date.Add(disponibilidad.TF_HoraInicio);
            if (!disponibilidad.TB_Estado || disponibilidad.TB_Reservado || inicio <= DateTime.Now)
            {
                TempData["Error"] = "Ese horario ya no está disponible.";
                return RedirectToAction(nameof(Detalle), new { id = disponibilidad.TN_IdAreaComun });
            }

            var userId = _userManager.GetUserId(User);

            var viviendaUsuario = await _context.ViviendaUsuarios
                .FirstOrDefaultAsync(v => v.TC_IdUsuario == userId && v.TB_ViveAhi);

            if (viviendaUsuario == null)
            {
                TempData["Error"] = "No tiene una vivienda asignada para realizar reservas.";
                return RedirectToAction(nameof(Detalle), new { id = disponibilidad.TN_IdAreaComun });
            }

            var reserva = new Habitia.Models.Reserva
            {
                TC_IdUsuario = userId,
                TN_IdVivienda = viviendaUsuario.TN_IdVivienda,
                TN_IdDisponibilidad = disponibilidad.TN_Id,
                TN_Cantidad = disponibilidad.TN_Cantidad, // definida por el admin al crear el horario
                TC_Motivo = vm.Motivo,
                TN_Estado = EstadoReservaEnum.Activa,
                TF_FechaRegistro = DateTime.Now
            };

            disponibilidad.TB_Reservado = true;

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Reserva confirmada correctamente";
            return RedirectToAction(nameof(MisReservas));
        }

        // ================= Mis reservas =================
        public async Task<IActionResult> MisReservas()
        {
            await ReservaHelper.FinalizarReservasVencidasAsync(_context);

            var userId = _userManager.GetUserId(User);

            var reservas = await _context.Reservas
                .Include(r => r.Disponibilidad).ThenInclude(d => d.AreaComun)
                .Where(r => r.TC_IdUsuario == userId)
                .OrderByDescending(r => r.TF_FechaRegistro)
                .ToListAsync();

            var vm = reservas.Select(r => ReservaHelper.MapToVM(r, mostrarDatosResidente: false, esAdmin: false)).ToList();

            return View(vm);
        }

        // ================= Cancelar (con restricción de anticipación) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            var userId = _userManager.GetUserId(User);

            var reserva = await _context.Reservas
                .Include(r => r.Disponibilidad).ThenInclude(d => d.AreaComun)
                .FirstOrDefaultAsync(r => r.TN_Id == id && r.TC_IdUsuario == userId);

            if (reserva == null)
                return NotFound();

            if (reserva.TN_Estado != EstadoReservaEnum.Activa)
            {
                TempData["Error"] = "Esta reserva ya no está activa.";
                return RedirectToAction(nameof(MisReservas));
            }

            var inicio = reserva.Disponibilidad.TF_Fecha.Date
                .Add(reserva.Disponibilidad.TF_HoraInicio);
            var minutos = reserva.Disponibilidad.AreaComun.TN_AnticipacionMinima;

            if (DateTime.Now.AddMinutes(minutos) > inicio)
            {
                var h = minutos / 60;
                var m = minutos % 60;
                TempData["Error"] =
                    $"No se puede cancelar: se requieren al menos {(h > 0 ? h + "h " : "")}{(m > 0 ? m + "min" : "")} de anticipación.";
                return RedirectToAction(nameof(MisReservas));
            }

            reserva.TN_Estado = EstadoReservaEnum.Cancelada;
            reserva.TF_FechaCancelacion = DateTime.Now;
            reserva.Disponibilidad.TB_Reservado = false;

            await _context.SaveChangesAsync();

            TempData["Exito"] = "Reserva cancelada correctamente";
            return RedirectToAction(nameof(MisReservas));
        }
    }
}