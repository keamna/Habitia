using Habitia.Data;
using Habitia.Enums;
using Habitia.Models;
using Habitia.Services.Interfaces;
using Habitia.ViewModels.Mantenimiento;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MantenimientosController : Controller
    {
        private readonly IMantenimientoService _mantenimientoService;
        private readonly ITipoMantenimientoService _tipoMantenimientoService;
        private readonly IIncidenciaService _incidenciaService;
        private readonly UserManager<ApplicationUser> _userManager;

        public MantenimientosController(
            IMantenimientoService mantenimientoService,
            ITipoMantenimientoService tipoMantenimientoService,
            IIncidenciaService incidenciaService,
            UserManager<ApplicationUser> userManager)
        {
            _mantenimientoService = mantenimientoService;
            _tipoMantenimientoService = tipoMantenimientoService;
            _incidenciaService = incidenciaService;
            _userManager = userManager;
        }

        // GET: /Admin/Mantenimientos
        public async Task<IActionResult> Index()
        {
            var mantenimientos = await _mantenimientoService.ObtenerTodosAsync();
            return View(mantenimientos);
        }

        // GET: /Admin/Mantenimientos/Create?idIncidencia=5
        public async Task<IActionResult> Create(int idIncidencia)
        {
            var incidencia = await _incidenciaService.ObtenerPorIdAsync(idIncidencia);

            if (incidencia == null)
                return NotFound();

            // Reglas de negocio validadas también aquí, antes de mostrar el formulario
            if (incidencia.Responsabilidad == ResponsabilidadEnum.Privado)
            {
                TempData["Error"] = "No se puede generar mantenimiento para una incidencia Privada.";
                return RedirectToAction("Details", "Incidencias", new { id = idIncidencia });
            }

            if (await _incidenciaService.TieneMantenimientoAsociadoAsync(idIncidencia))
            {
                TempData["Error"] = "Esta incidencia ya tiene una tarea de mantenimiento asociada.";
                return RedirectToAction("Details", "Incidencias", new { id = idIncidencia });
            }

            var model = new MantenimientoCreateViewModel
            {
                IdIncidencia = idIncidencia,
                Descripcion = incidencia.Titulo // valor sugerido, el Admin puede editarlo
            };

            await CargarListasAsync(model);
            return View(model);
        }

        // POST: /Admin/Mantenimientos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MantenimientoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasAsync(model);
                return View(model);
            }

            try
            {
                var mantenimiento = await _mantenimientoService.ConvertirDesdeIncidenciaAsync(model);

                TempData["Success"] = "Tarea de mantenimiento generada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync(model);
                return View(model);
            }
        }

        // GET: /Admin/Mantenimientos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var mantenimiento = await _mantenimientoService.ObtenerPorIdAsync(id);

            if (mantenimiento == null)
                return NotFound();

            return View(mantenimiento);
        }

        private async Task CargarListasAsync(MantenimientoCreateViewModel model)
        {
            var tipos = await _tipoMantenimientoService.ObtenerActivosAsync();
            model.TiposMantenimiento = tipos.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Nombre
            }).ToList();

            var personalMantenimiento = await _userManager.GetUsersInRoleAsync("Mantenimiento");
            model.PersonalMantenimiento = personalMantenimiento
                .OrderBy(u => u.UserName)
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.UserName
                }).ToList();
        }
    }
}