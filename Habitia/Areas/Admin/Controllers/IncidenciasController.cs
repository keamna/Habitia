using Habitia.Data;
using Habitia.Enums;
using Habitia.Helpers;
using Habitia.Models;
using Habitia.Services.Interfaces;
using Habitia.ViewModels.Incidencias;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class IncidenciasController : Controller
    {
        private readonly IIncidenciaService _incidenciaService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public IncidenciasController(
            IIncidenciaService incidenciaService,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            _incidenciaService = incidenciaService;
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: /Admin/Incidencias
        public async Task<IActionResult> Index(IncidenciaFiltroViewModel filtro)
        {
            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                filtro = new IncidenciaFiltroViewModel();
            }

            var incidencias = await _incidenciaService.ObtenerTodasAsync(filtro);

            ViewBag.Filtro = filtro;
            return View(incidencias);
        }

        // GET: /Admin/Incidencias/Create
        // El Admin reporta con el MISMO formulario que Residente/Seguridad
        public async Task<IActionResult> Create()
        {
            var model = new IncidenciaCreateViewModel();
            await CargarListasAsync(model);
            return View(model);
        }

        // POST: /Admin/Incidencias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IncidenciaCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasAsync(model);
                return View(model);
            }

            var idAdmin = _userManager.GetUserId(User);

            try
            {
                var imagenUrl = await ArchivoHelper.GuardarEvidenciaAsync(model.Evidencia, _env.WebRootPath);
                await _incidenciaService.CrearAsync(model, idAdmin!, imagenUrl);

                TempData["Success"] = "Incidencia reportada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync(model);
                return View(model);
            }
        }

        // GET: /Admin/Incidencias/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var incidencia = await _incidenciaService.ObtenerPorIdAsync(id);

            if (incidencia == null)
                return NotFound();

            return View(incidencia);
        }

        // GET: /Admin/Incidencias/AsignarPrioridad/5
        public async Task<IActionResult> AsignarPrioridad(int id)
        {
            var incidencia = await _incidenciaService.ObtenerPorIdAsync(id);

            if (incidencia == null)
                return NotFound();

            var model = ConstruirModeloPrioridad(incidencia);

            return View(model);
        }

        // POST: /Admin/Incidencias/AsignarPrioridad
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarPrioridad(IncidenciaPrioridadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Solo Id y Prioridad viajan desde el form; el resto de los datos
                // de contexto (título, descripción, reportante, etc.) hay que
                // recargarlos desde la BD para poder re-renderizar la vista.
                var incidenciaContexto = await _incidenciaService.ObtenerPorIdAsync(model.Id);
                if (incidenciaContexto == null)
                    return NotFound();

                model = ConstruirModeloPrioridad(incidenciaContexto, model.Prioridad);
                return View(model);
            }

            try
            {
                await _incidenciaService.AsignarPrioridadAsync(model);
                TempData["Success"] = "Prioridad asignada correctamente.";
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                var incidenciaContexto = await _incidenciaService.ObtenerPorIdAsync(model.Id);
                if (incidenciaContexto != null)
                    model = ConstruirModeloPrioridad(incidenciaContexto, model.Prioridad);

                return View(model);
            }
        }

        // POST: /Admin/Incidencias/MarcarComoResuelta/5
        // Cierre manual de incidencias Privadas (el Admin puede cerrarla si el residente no lo hizo)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarComoResuelta(int id)
        {
            var idAdmin = _userManager.GetUserId(User);

            try
            {
                await _incidenciaService.MarcarComoResueltaAsync(id, idAdmin!, esAdmin: true);
                TempData["Success"] = "Incidencia marcada como resuelta correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        private static IncidenciaPrioridadViewModel ConstruirModeloPrioridad(Incidencia incidencia, PrioridadEnum? prioridadSeleccionada = null)
        {
            return new IncidenciaPrioridadViewModel
            {
                Id = incidencia.TN_Id,
                Titulo = incidencia.TC_Titulo,
                Descripcion = incidencia.TC_Descripcion,
                NombreCompletoReporta = incidencia.Usuario != null
                    ? $"{incidencia.Usuario.TC_Nombre} {incidencia.Usuario.TC_Apellido}"
                    : "N/D",
                IdentificacionReporta = incidencia.Usuario?.TC_Identificacion,
                FechaRegistro = incidencia.TF_FechaRegistro,
                Responsabilidad = incidencia.TN_Responsabilidad,
                Prioridad = prioridadSeleccionada
            };
        }

        private async Task CargarListasAsync(IncidenciaCreateViewModel model)
        {
            model.Viviendas = await _context.Viviendas
                .OrderBy(v => v.TC_Numero)
                .Select(v => new SelectListItem { Value = v.TN_Id.ToString(), Text = v.TC_Numero })
                .ToListAsync();

            model.AreasComunes = await _context.AreasComunes
                .OrderBy(a => a.TC_Nombre)
                .Select(a => new SelectListItem { Value = a.TN_Id.ToString(), Text = a.TC_Nombre })
                .ToListAsync();
        }
    }
}