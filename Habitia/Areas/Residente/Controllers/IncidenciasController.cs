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
        public async Task<IActionResult> Index(IncidenciaFiltroViewModel filtro)
        {
            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                filtro = new IncidenciaFiltroViewModel();
            }

            var idUsuario = _userManager.GetUserId(User);
            var incidencias = await _incidenciaService.ObtenerPorUsuarioAsync(idUsuario!, filtro);

            ViewBag.Filtro = filtro;
            ViewBag.TieneIncidencias = await _context.Incidencias.AnyAsync(i => i.TC_IdUsuario == idUsuario);

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

            if (incidencia.TC_IdUsuario != idUsuario)
                return Forbid();

            return View(incidencia);
        }

        // POST: /Residentes/Incidencias/MarcarComoResuelta/5
        // El residente solo puede resolver sus propias incidencias Privadas.
        // Las incidencias Comun/Mixto nunca pasan por aquí porque
        // MarcarComoResueltaAsync solo permite responsabilidad Privado.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarComoResuelta(int id)
        {
            var idUsuario = _userManager.GetUserId(User);

            try
            {
                await _incidenciaService.MarcarComoResueltaAsync(id, idUsuario!, esAdmin: false);
                TempData["Success"] = "Incidencia marcada como resuelta correctamente.";
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task CargarListasAsync(IncidenciaCreateViewModel model)
        {
            var idUsuario = _userManager.GetUserId(User);

            model.Viviendas = await _context.ViviendaUsuarios
                .Include(vu => vu.Vivienda)
                .Where(vu => vu.TC_IdUsuario == idUsuario)
                .Select(vu => new SelectListItem
                {
                    Value = vu.Vivienda.TN_Id.ToString(),
                    Text = vu.Vivienda.TC_Numero
                })
                .ToListAsync();

            model.AreasComunes = await _context.AreasComunes
                .OrderBy(a => a.TC_Nombre)
                .Select(a => new SelectListItem
                {
                    Value = a.TN_Id.ToString(),
                    Text = a.TC_Nombre
                })
                .ToListAsync();
        }
    }
}