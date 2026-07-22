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
        public async Task<IActionResult> Index()
        {
            var idPersonal = _userManager.GetUserId(User);
            var tareas = await _mantenimientoService.ObtenerPorPersonalAsignadoAsync(idPersonal!);
            return View(tareas);
        }

        // GET: /Mantenimiento/Tareas/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var tarea = await _mantenimientoService.ObtenerPorIdAsync(id);

            if (tarea == null)
                return NotFound();

            var idPersonal = _userManager.GetUserId(User);
            if (tarea.TC_IdPersonalAsignado != idPersonal)
                return Forbid();

            return View(tarea);
        }

        // GET: /Mantenimiento/Tareas/ActualizarEstado/5
        public async Task<IActionResult> ActualizarEstado(int id)
        {
            var tarea = await _mantenimientoService.ObtenerPorIdAsync(id);

            if (tarea == null)
                return NotFound();

            var idPersonal = _userManager.GetUserId(User);
            if (tarea.TC_IdPersonalAsignado != idPersonal)
                return Forbid();

            var model = new MantenimientoEstadoUpdateViewModel
            {
                Id = tarea.TN_Id,
                Estado = tarea.TN_Estado,
                Observaciones = tarea.TC_Observaciones
            };

            ViewBag.Descripcion = tarea.TC_Descripcion;
            return View(model);
        }

        // POST: /Mantenimiento/Tareas/ActualizarEstado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarEstado(MantenimientoEstadoUpdateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var idPersonal = _userManager.GetUserId(User);

            try
            {
                await _mantenimientoService.ActualizarEstadoAsync(model, idPersonal!);
                TempData["Success"] = "Estado de la tarea actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }
    }
}