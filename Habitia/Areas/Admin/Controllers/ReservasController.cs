using Habitia.Data;
using Habitia.Enums;
using Habitia.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= Listado completo de reservas =================
        public async Task<IActionResult> Todas()
        {
            await ReservaHelper.FinalizarReservasVencidasAsync(_context);

            var reservas = await _context.Reservas
                .Include(r => r.Disponibilidad).ThenInclude(d => d.AreaComun)
                .Include(r => r.Usuario)
                .Include(r => r.Vivienda)
                .OrderByDescending(r => r.Disponibilidad.TF_Fecha)
                .ToListAsync();

            var vm = reservas.Select(r => ReservaHelper.MapToVM(r, mostrarDatosResidente: true, esAdmin: true)).ToList();

            ViewBag.EsAdmin = true;
            return View(vm);
        }

        // ================= Cancelar cualquier reserva, sin restricción de anticipación =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarAdmin(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Disponibilidad)
                .FirstOrDefaultAsync(r => r.TN_Id == id);

            if (reserva == null)
                return NotFound();

            if (reserva.TN_Estado == EstadoReservaEnum.Activa)
            {
                reserva.TN_Estado = EstadoReservaEnum.Cancelada;
                reserva.TF_FechaCancelacion = DateTime.Now;
                reserva.Disponibilidad.TB_Reservado = false;
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Reserva cancelada correctamente";
            }
            else
            {
                TempData["Error"] = "Esta reserva ya no está activa.";
            }

            return RedirectToAction(nameof(Todas));
        }
    }
}