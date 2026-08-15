using Habitia.Data;
using Habitia.Enums;
using Habitia.Helpers;
using Habitia.Models;
using Habitia.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Panel Administrador";

            // Deja los estados de reserva al día antes de contarlos.
            await ReservaHelper.FinalizarReservasVencidasAsync(_context);

            var usuario = await _userManager.GetUserAsync(User);
            var hoy = DateTime.Today;
            var manana = hoy.AddDays(1);
            var ahora = DateTime.Now;

            var model = new PanelAdminViewModel
            {
                NombreUsuario = usuario?.TC_Nombre ?? "Administrador"
            };

            // ===================== USUARIOS =====================

            model.UsuariosActivos = await _context.Users
                .CountAsync(u => u.TN_Estado == EstadoUsuarioEnum.Activo);

            model.SolicitudesPendientes = await _context.ViviendaUsuarios
                .CountAsync(v => v.TN_Estado == EstadoUsuarioEnum.Pendiente);

            // ===================== VIVIENDAS =====================

            model.TotalViviendas = await _context.Viviendas.CountAsync();

            model.ViviendasOcupadas = await _context.Viviendas
                .CountAsync(v => v.TN_Estado == EstadoViviendaEnum.Ocupada);

            model.ViviendasDisponibles = await _context.Viviendas
                .CountAsync(v => v.TN_Estado == EstadoViviendaEnum.Disponible);

            // ===================== ÁREAS COMUNES Y RESERVAS =====================

            model.AreasComunesActivas = await _context.AreasComunes
                .CountAsync(a => a.TB_Estado);

            var reservasActivas = await _context.Reservas
                .Include(r => r.Disponibilidad).ThenInclude(d => d.AreaComun)
                .Include(r => r.Vivienda)
                .Include(r => r.Usuario)
                .Where(r => r.TN_Estado == EstadoReservaEnum.Activa)
                .ToListAsync();

            model.ReservasActivas = reservasActivas.Count;

            model.ReservasHoy = reservasActivas
                .Count(r => r.Disponibilidad.TF_Fecha.Date == hoy);

            model.ProximasReservas = reservasActivas
                .Where(r => r.Disponibilidad.TF_Fecha.Date.Add(r.Disponibilidad.TF_HoraInicio) >= ahora)
                .OrderBy(r => r.Disponibilidad.TF_Fecha.Date.Add(r.Disponibilidad.TF_HoraInicio))
                .Take(5)
                .Select(r => new PanelItemViewModel
                {
                    Titulo = r.Disponibilidad.AreaComun.TC_Nombre,
                    Detalle = "Vivienda " + r.Vivienda.TC_Numero + " · " + r.Usuario.TC_Nombre + " " + r.Usuario.TC_Apellido,
                    Etiqueta = r.Disponibilidad.TF_Fecha.Date == hoy
                        ? "Hoy"
                        : r.Disponibilidad.TF_Fecha.ToString("dd/MM"),
                    EtiquetaEstilo = r.Disponibilidad.TF_Fecha.Date == hoy ? "success" : "secondary",
                    Fecha = r.Disponibilidad.TF_Fecha.Date.Add(r.Disponibilidad.TF_HoraInicio)
                })
                .ToList();

            // ===================== ACCESOS =====================

            model.VisitantesDentro = await _context.Accesos
                .CountAsync(a => a.TF_FechaSalida == null && a.TB_Estado);

            model.AccesosHoy = await _context.Accesos
                .CountAsync(a => a.TF_FechaIngreso >= hoy && a.TF_FechaIngreso < manana);

            // ===================== INCIDENCIAS =====================

            model.IncidenciasPendientes = await _context.Incidencias
                .CountAsync(i => i.TN_Estado == EstadoIncidenciaEnum.Pendiente);

            // ===================== SOLICITUDES RECIENTES =====================

            var solicitudes = await _context.ViviendaUsuarios
                .Include(v => v.Usuario)
                .Include(v => v.Vivienda)
                .Where(v => v.TN_Estado == EstadoUsuarioEnum.Pendiente)
                .OrderByDescending(v => v.TF_FechaRegistro)
                .Take(5)
                .ToListAsync();

            model.SolicitudesRecientes = solicitudes
                .Select(v => new PanelItemViewModel
                {
                    Titulo = v.Usuario.TC_Nombre + " " + v.Usuario.TC_Apellido,
                    Detalle = "Vivienda " + v.Vivienda.TC_Numero + " · " + v.TN_TipoRelacion.ToString(),
                    Etiqueta = "Pendiente",
                    EtiquetaEstilo = "warning",
                    Fecha = v.TF_FechaRegistro
                })
                .ToList();

            return View(model);
        }
    }
}