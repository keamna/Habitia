using Habitia.Helpers;
using Habitia.Models;
using Habitia.Services.Interfaces;
using Habitia.ViewModels.Incidencias;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Habitia.Data;

namespace Habitia.Areas.Seguridad.Controllers
{
    [Area("Seguridad")]
    [Authorize(Roles = "Seguridad")]
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

        // GET: /Seguridad/Incidencias
        public async Task<IActionResult> Index()
        {
            var idUsuario = _userManager.GetUserId(User);
            var incidencias = await _incidenciaService.ObtenerPorUsuarioAsync(idUsuario!);
            return View(incidencias);
        }

        // GET: /Seguridad/Incidencias/Create
        public async Task<IActionResult> Create()
        {
            var model = new IncidenciaCreateViewModel();
            await CargarListasAsync(model);
            return View(model);
        }

        // POST: /Seguridad/Incidencias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IncidenciaCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasAsync(model);
                return View(model);
            }

            var idUsuario = _userManager.GetUserId(User);

            try
            {
                var imagenUrl = await ArchivoHelper.GuardarEvidenciaAsync(model.Evidencia, _env.WebRootPath);
                await _incidenciaService.CrearAsync(model, idUsuario!, imagenUrl);

                TempData["Success"] = "Incidencia registrada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync(model);
                return View(model);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var incidencia = await _incidenciaService.ObtenerPorIdAsync(id);

            if (incidencia == null)
                return NotFound();

            return View(incidencia);
        }

        // Seguridad puede reportar sobre cualquier vivienda o área común del condominio
        private async Task CargarListasAsync(IncidenciaCreateViewModel model)
        {
            model.Viviendas = await _context.Viviendas
                .OrderBy(v => v.Numero)
                .Select(v => new SelectListItem
                {
                    Value = v.Id.ToString(),
                    Text = v.Numero
                })
                .ToListAsync();

            model.AreasComunes = await _context.AreasComunes
                .OrderBy(a => a.Nombre)
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Nombre
                })
                .ToListAsync();
        }
    }
}