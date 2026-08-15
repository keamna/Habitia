using Habitia.Data;
using Habitia.Enums;
using Habitia.Helpers;
using Habitia.Models;
using Habitia.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Seguridad.Controllers
{
    [Area("Seguridad")]
    [Authorize(Roles = "Seguridad")]
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
            ViewData["Title"] = "Panel Seguridad";

            await ReservaHelper.FinalizarReservasVencidasAsync(_context);

            var usuario = await _userManager.GetUserAsync(User);
            var hoy = DateTime.Today;
            var manana = hoy.AddDays(1);
            var ahora = DateTime.Now;

            var model = new PanelSeguridadViewModel
            {
                NombreUsuario = usuario?.TC_Nombre ?? "Seguridad"
            };

            // ===================== ACCESOS =====================

            var dentro = await _context.Accesos
                .Include(a => a.Visitante)
                .Include(a => a.Vivienda)
                .Include(a => a.Vehiculo)
                .Where(a => a.TF_FechaSalida == null && a.TB_Estado)
                .OrderByDescending(a => a.TF_FechaIngreso)
                .ToListAsync();

            model.VisitantesDentro = dentro.Count;

            model.DentroAhora = dentro
                .Take(6)
                .Select(a => new PanelItemViewModel
                {
                    Titulo = a.Visitante.TC_Nombre,
                    Detalle = "Vivienda " + a.Vivienda.TC_Numero +
                              (a.Vehiculo != null ? " · Placa " + a.Vehiculo.TC_Placa : ""),
                    Etiqueta = "Desde " + a.TF_FechaIngreso.ToString("hh:mm tt"),
                    EtiquetaEstilo = "info",
                    Fecha = a.TF_FechaIngreso
                })
                .ToList();

            model.IngresosHoy = await _context.Accesos
                .CountAsync(a => a.TF_FechaIngreso >= hoy && a.TF_FechaIngreso < manana);

            model.SalidasHoy = await _context.Accesos
                .CountAsync(a => a.TF_FechaSalida != null &&
                                 a.TF_FechaSalida >= hoy &&
                                 a.TF_FechaSalida < manana);

            // ===================== AUTORIZACIONES DE HOY =====================

            var autorizacionesHoy = await _context.Autorizaciones
                .Include(a => a.Visitante)
                .Include(a => a.Vivienda)
                .Where(a => a.TF_FechaVisita >= hoy &&
                            a.TF_FechaVisita < manana &&
                            (a.TN_Estado == EstadoAutorizacionEnum.Activa ||
                             a.TN_Estado == EstadoAutorizacionEnum.Pendiente))
                .OrderBy(a => a.TF_FechaVisita)
                .ToListAsync();

            model.AutorizacionesVigentesHoy = autorizacionesHoy.Count;

            model.AutorizacionesDeHoy = autorizacionesHoy
                .Take(6)
                .Select(a => new PanelItemViewModel
                {
                    Titulo = a.Visitante.TC_Nombre,
                    Detalle = "Vivienda " + a.Vivienda.TC_Numero + " · Código " + a.TC_Codigo,
                    Etiqueta = a.TN_Estado.ToString(),
                    EtiquetaEstilo = a.TN_Estado == EstadoAutorizacionEnum.Activa ? "success" : "warning",
                    Fecha = a.TF_FechaVisita
                })
                .ToList();

            // ===================== RESERVAS DE HOY =====================

            var reservasHoy = await _context.Reservas
                .Include(r => r.Disponibilidad)
                .Where(r => r.TN_Estado == EstadoReservaEnum.Activa)
                .ToListAsync();

            model.ReservasHoy = reservasHoy
                .Count(r => r.Disponibilidad.TF_Fecha.Date == hoy);

            return View(model);
        }
    }
}