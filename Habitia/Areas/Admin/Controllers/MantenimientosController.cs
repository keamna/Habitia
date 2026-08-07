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

        public async Task<IActionResult> Create(int? idIncidencia)
        {
            var model = new MantenimientoCreateViewModel();

            if (idIncidencia.HasValue)
            {
                var incidencia = await _incidenciaService.ObtenerPorIdAsync(idIncidencia.Value);

                if (incidencia == null)
                    return NotFound();

                if (incidencia.TN_Responsabilidad == ResponsabilidadEnum.Privado)
                {
                    TempData["Error"] = "Las incidencias privadas no requieren tarea de mantenimiento.";
                    return RedirectToAction("Details", "Incidencias", new { id = idIncidencia.Value });
                }

                if (await _incidenciaService.TieneMantenimientoAsociadoAsync(idIncidencia.Value))
                {
                    TempData["Error"] = "Esta incidencia ya tiene una tarea de mantenimiento asociada.";
                    return RedirectToAction("Details", "Incidencias", new { id = idIncidencia.Value });
                }

                model.IdIncidencia = idIncidencia.Value;
                model.Descripcion = incidencia.TC_Titulo;
            }

            await CargarListasAsync(model, idIncidencia.HasValue);
            return View(model);
        }

        // POST: /Admin/Mantenimientos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MantenimientoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasAsync(model, false);
                return View(model);
            }

            try
            {
                await _mantenimientoService.ConvertirDesdeIncidenciaAsync(model);

                TempData["Success"] = "Tarea de mantenimiento creada y asignada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync(model, false);
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

        // GET: /Admin/Mantenimientos/PersonalPorTipo?idTipo=3
        // Devuelve el personal ACTIVO configurado para atender ese tipo de mantenimiento.
        // Usado por el JS de Create.cshtml para filtrar el <select> dinámicamente.
        [HttpGet]
        public async Task<IActionResult> PersonalPorTipo(int idTipo)
        {
            var personal = await _mantenimientoService.ObtenerPersonalActivoPorTipoAsync(idTipo);

            var resultado = personal.Select(u => new
            {
                id = u.Id,
                nombre = u.UserName
            });

            return Json(resultado);
        }

        private async Task CargarListasAsync(MantenimientoCreateViewModel model, bool incidenciaFija)
        {
            var tipos = await _tipoMantenimientoService.ObtenerActivosAsync();
            model.TiposMantenimiento = tipos.Select(t => new SelectListItem
            {
                Value = t.TN_Id.ToString(),
                Text = t.TC_Nombre
            }).ToList();

            // Carga inicial completa (personal activo); el JS la reemplaza al elegir un tipo existente.
            var personalMantenimiento = await _userManager.GetUsersInRoleAsync("Mantenimiento");
            model.PersonalMantenimiento = personalMantenimiento
                .Where(u => u.TN_Estado == EstadoUsuarioEnum.Activo)
                .OrderBy(u => u.UserName)
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.UserName
                }).ToList();

            if (!incidenciaFija)
            {
                var elegibles = await _incidenciaService.ObtenerElegiblesParaMantenimientoAsync();
                model.IncidenciasElegibles = elegibles.Select(i => new SelectListItem
                {
                    Value = i.TN_Id.ToString(),
                    Text = $"{i.TC_Titulo} ({i.TN_Responsabilidad})"
                }).ToList();
            }
        }
    }
}