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
using Habitia.Data;

namespace Habitia.Areas.Residentes.Controllers
{
    [Area("Residente")]
    [Authorize(Roles = "Residente")]
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

        // GET: /Residentes/Incidencias
        public async Task<IActionResult> Index()
        {
            var idUsuario = _userManager.GetUserId(User);
            var incidencias = await _incidenciaService.ObtenerPorUsuarioAsync(idUsuario!);
            return View(incidencias);
        }

        // GET: /Residentes/Incidencias/Create
        public async Task<IActionResult> Create()
        {
            var model = new IncidenciaCreateViewModel();
            await CargarListasAsync(model);
            return View(model);
        }

        // POST: /Residentes/Incidencias/Create
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

        // GET: /Residentes/Incidencias/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var incidencia = await _incidenciaService.ObtenerPorIdAsync(id);

            if (incidencia == null)
                return NotFound();

            var idUsuario = _userManager.GetUserId(User);

            // Un residente solo puede ver sus propias incidencias
            if (incidencia.IdUsuario != idUsuario)
                return Forbid();

            return View(incidencia);
        }

        // Precarga solo las viviendas asociadas al usuario logueado (no todas las del condominio)
        private async Task CargarListasAsync(IncidenciaCreateViewModel model)
        {
            var idUsuario = _userManager.GetUserId(User);

            model.Viviendas = await _context.ViviendaUsuarios
                .Include(vu => vu.Vivienda)
                .Where(vu => vu.IdUsuario == idUsuario)
                .Select(vu => new SelectListItem
                {
                    Value = vu.Vivienda.Id.ToString(),
                    Text = vu.Vivienda.Numero
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