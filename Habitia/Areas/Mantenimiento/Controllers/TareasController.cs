using Habitia.Models;
using Habitia.Services.Interfaces;
using Habitia.ViewModels.Mantenimiento;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Habitia.Areas.Mantenimiento.Controllers
{
    [Area("Mantenimiento")]
    [Authorize(Roles = "Mantenimiento")]
    public class TareasController : Controller
    {
        private readonly IMantenimientoService _mantenimientoService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TareasController(
            IMantenimientoService mantenimientoService,
            UserManager<ApplicationUser> userManager)
        {
            _mantenimientoService = mantenimientoService;
            _userManager = userManager;
        }

        // GET: /Mantenimiento/Tareas
        // Bandeja de trabajo: solo las tareas asignadas a este usuario
        public async Task<IActionResult> Index()
        {
            var idUsuario = _userManager.GetUserId(User);
            var tareas = await _mantenimientoService.ObtenerPorPersonalAsignadoAsync(idUsuario!);
            return View(tareas);
        }

        // GET: /Mantenimiento/Tareas/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var idUsuario = _userManager.GetUserId(User);
            var tarea = await _mantenimientoService.ObtenerPorIdAsync(id);

            if (tarea == null)
                return NotFound();

            if (tarea.TC_IdPersonalAsignado != idUsuario)
                return Forbid();

            return View(tarea);
        }

        // GET: /Mantenimiento/Tareas/Actualizar/5
        public async Task<IActionResult> Actualizar(int id)
        {
            var idUsuario = _userManager.GetUserId(User);
            var tarea = await _mantenimientoService.ObtenerPorIdAsync(id);

            if (tarea == null)
                return NotFound();

            if (tarea.TC_IdPersonalAsignado != idUsuario)
                return Forbid();

            if (tarea.TN_Estado == Habitia.Enums.EstadoMantenimientoEnum.Completado)
            {
                TempData["Error"] = "No es posible modificar una tarea que ya fue completada.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var model = new MantenimientoEstadoUpdateViewModel
            {
                Id = tarea.TN_Id,
                Estado = tarea.TN_Estado,
                Observaciones = tarea.TC_Observaciones
            };

            ViewBag.Descripcion = tarea.TC_Descripcion;
            ViewBag.Titulo = tarea.Incidencia?.TC_Titulo;

            return View(model);
        }

        // POST: /Mantenimiento/Tareas/Actualizar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Actualizar(MantenimientoEstadoUpdateViewModel model)
        {
            var idUsuario = _userManager.GetUserId(User);

            if (!ModelState.IsValid)
            {
                var tarea = await _mantenimientoService.ObtenerPorIdAsync(model.Id);
                ViewBag.Descripcion = tarea?.TC_Descripcion;
                ViewBag.Titulo = tarea?.Incidencia?.TC_Titulo;
                return View(model);
            }

            try
            {
                await _mantenimientoService.ActualizarEstadoAsync(model, idUsuario!);
                TempData["Success"] = "Información guardada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}