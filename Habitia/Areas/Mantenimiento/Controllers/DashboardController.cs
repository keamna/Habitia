using Habitia.Data;
using Habitia.Enums;
using Habitia.Models;
using Habitia.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Mantenimiento.Controllers
{
    [Area("Mantenimiento")]
    [Authorize(Roles = "Mantenimiento")]
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
            ViewData["Title"] = "Panel Mantenimiento";

            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var userId = usuario.Id;
            var inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            var model = new PanelMantenimientoViewModel
            {
                NombreUsuario = usuario.TC_Nombre
            };

            var tareas = await _context.Mantenimientos
                .Include(m => m.Tipo)
                .Include(m => m.Incidencia)
                .Include(m => m.AreaComun)
                .Where(m => m.TC_IdPersonalAsignado == userId)
                .ToListAsync();

            model.TareasProgramadas = tareas
                .Count(m => m.TN_Estado == EstadoMantenimientoEnum.Programado);

            model.TareasEnProceso = tareas
                .Count(m => m.TN_Estado == EstadoMantenimientoEnum.EnProceso);

            model.TareasCompletadasMes = tareas
                .Count(m => m.TN_Estado == EstadoMantenimientoEnum.Completado &&
                            m.TF_FechaFin.HasValue &&
                            m.TF_FechaFin.Value >= inicioMes);

            model.TareasPendientes = tareas
                .Where(m => m.TN_Estado == EstadoMantenimientoEnum.Programado ||
                            m.TN_Estado == EstadoMantenimientoEnum.EnProceso)
                .OrderBy(m => m.TF_FechaRegistro)
                .Take(6)
                .Select(m => new PanelItemViewModel
                {
                    Titulo = m.Incidencia != null ? m.Incidencia.TC_Titulo : m.TC_Descripcion,
                    Detalle = (m.Tipo != null ? m.Tipo.TC_Nombre : "Sin tipo") +
                              (m.AreaComun != null ? " · " + m.AreaComun.TC_Nombre : ""),

                    // "EnProceso" se muestra separado y con solo la primera en mayúscula.
                    Etiqueta = m.TN_Estado == EstadoMantenimientoEnum.EnProceso
                        ? "En proceso"
                        : m.TN_Estado.ToString(),

                    EtiquetaEstilo = m.TN_Estado == EstadoMantenimientoEnum.EnProceso ? "info" : "warning",
                    Fecha = m.TF_FechaRegistro
                })
                .ToList();

            return View(model);
        }
    }
}