using Habitia.Data;
using Habitia.Enums;
using Habitia.Helpers;
using Habitia.Models;
using Habitia.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Residente.Controllers
{
    [Area("Residente")]
    [Authorize(Roles = "Residente")]
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
            ViewData["Title"] = "Panel Residente";

            await ReservaHelper.FinalizarReservasVencidasAsync(_context);

            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var userId = usuario.Id;
            var hoy = DateTime.Today;
            var ahora = DateTime.Now;

            var model = new PanelResidenteViewModel
            {
                NombreUsuario = usuario.TC_Nombre
            };

            // ===================== MIS VIVIENDAS =====================

            var viviendas = await _context.ViviendaUsuarios
                .Include(v => v.Vivienda)
                .Where(v => v.TC_IdUsuario == userId)
                .ToListAsync();

            model.MisViviendas = viviendas
                .Select(v => new PanelItemViewModel
                {
                    Titulo = v.Vivienda.TC_Numero,
                    Detalle = v.TN_TipoRelacion.ToString() + (v.TB_ViveAhi ? " · Vive ahí" : ""),
                    Etiqueta = v.TN_Estado.ToString(),
                    EtiquetaEstilo = v.TN_Estado == EstadoUsuarioEnum.Activo ? "success"
                                   : v.TN_Estado == EstadoUsuarioEnum.Pendiente ? "warning"
                                   : "secondary"
                })
                .ToList();

            // ===================== MIS RESERVAS =====================

            var reservas = await _context.Reservas
                .Include(r => r.Disponibilidad).ThenInclude(d => d.AreaComun)
                .Where(r => r.TC_IdUsuario == userId &&
                            r.TN_Estado == EstadoReservaEnum.Activa)
                .ToListAsync();

            model.ReservasActivas = reservas.Count;

            model.ProximasReservas = reservas
                .Where(r => r.Disponibilidad.TF_Fecha.Date.Add(r.Disponibilidad.TF_HoraInicio) >= ahora)
                .OrderBy(r => r.Disponibilidad.TF_Fecha.Date.Add(r.Disponibilidad.TF_HoraInicio))
                .Take(5)
                .Select(r => new PanelItemViewModel
                {
                    Titulo = r.Disponibilidad.AreaComun.TC_Nombre,
                    Detalle = DateTime.Today.Add(r.Disponibilidad.TF_HoraInicio).ToString("hh:mm tt") +
                              " a " +
                              DateTime.Today.Add(r.Disponibilidad.TF_HoraFin).ToString("hh:mm tt"),
                    Etiqueta = r.Disponibilidad.TF_Fecha.Date == hoy
                        ? "Hoy"
                        : r.Disponibilidad.TF_Fecha.ToString("dd/MM"),
                    EtiquetaEstilo = r.Disponibilidad.TF_Fecha.Date == hoy ? "success" : "secondary",
                    Fecha = r.Disponibilidad.TF_Fecha.Date.Add(r.Disponibilidad.TF_HoraInicio)
                })
                .ToList();

            // ===================== MIS VISITAS AUTORIZADAS =====================

            var autorizaciones = await _context.Autorizaciones
                .Include(a => a.Visitante)
                .Where(a => a.TC_IdUsuario == userId &&
                            (a.TN_Estado == EstadoAutorizacionEnum.Activa ||
                             a.TN_Estado == EstadoAutorizacionEnum.Pendiente) &&
                            a.TF_FechaVencimiento >= ahora)
                .OrderBy(a => a.TF_FechaVisita)
                .ToListAsync();

            model.AutorizacionesVigentes = autorizaciones.Count;

            model.ProximasVisitas = autorizaciones
                .Take(5)
                .Select(a => new PanelItemViewModel
                {
                    Titulo = a.Visitante.TC_Nombre,
                    Detalle = "Código " + a.TC_Codigo + " · " + a.TC_Motivo,
                    Etiqueta = a.TF_FechaVisita.Date == hoy
                        ? "Hoy"
                        : a.TF_FechaVisita.ToString("dd/MM"),
                    EtiquetaEstilo = a.TF_FechaVisita.Date == hoy ? "success" : "secondary",
                    Fecha = a.TF_FechaVisita
                })
                .ToList();

            // ===================== MIS INCIDENCIAS =====================

            model.IncidenciasAbiertas = await _context.Incidencias
                .CountAsync(i => i.TC_IdUsuario == userId &&
                                 (i.TN_Estado == EstadoIncidenciaEnum.Pendiente ||
                                  i.TN_Estado == EstadoIncidenciaEnum.EnProceso));

            return View(model);
        }
    }
}