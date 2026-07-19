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
        private readonly ITipoMantenimientoService _tipoMantenimientoService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public IncidenciasController(
            IIncidenciaService incidenciaService,
            ITipoMantenimientoService tipoMantenimientoService,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            _incidenciaService = incidenciaService;
            _tipoMantenimientoService = tipoMantenimientoService;
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
        public async Task<IActionResult> Create()
        {
            var model = new IncidenciaAdminCreateViewModel();
            await CargarListasAsync(model);
            return View(model);
        }

        // POST: /Admin/Incidencias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IncidenciaAdminCreateViewModel model)
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
                var resultado = await _incidenciaService.CrearPorAdminAsync(model, idAdmin!, imagenUrl);

                TempData["Success"] = resultado.Mantenimiento != null
                    ? "Incidencia registrada y tarea de mantenimiento generada correctamente."
                    : "Incidencia registrada correctamente (responsabilidad Privada, sin tarea asociada).";

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

        // GET: /Admin/Incidencias/Clasificar/5
        public async Task<IActionResult> Clasificar(int id)
        {
            var incidencia = await _incidenciaService.ObtenerPorIdAsync(id);

            if (incidencia == null)
                return NotFound();

            var model = new IncidenciaClasificarViewModel
            {
                Id = incidencia.Id,
                Titulo = incidencia.Titulo,
                Descripcion = incidencia.Descripcion,
                NombreUsuarioReporta = incidencia.Usuario?.UserName ?? "N/D",
                FechaRegistro = incidencia.FechaRegistro,
                Responsabilidad = incidencia.Responsabilidad
            };

            return View(model);
        }

        // POST: /Admin/Incidencias/Clasificar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clasificar(IncidenciaClasificarViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _incidenciaService.ClasificarAsync(model);
                TempData["Success"] = "Incidencia clasificada correctamente.";

                if (model.Responsabilidad != ResponsabilidadEnum.Privado)
                {
                    return RedirectToAction("Create", "Mantenimientos", new { idIncidencia = model.Id });
                }

                return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // El Admin ve todas las viviendas/áreas comunes del condominio, más
        // los catálogos de mantenimiento, porque su formulario es combinado.
        private async Task CargarListasAsync(IncidenciaAdminCreateViewModel model)
        {
            model.Viviendas = await _context.Viviendas
                .OrderBy(v => v.Numero)
                .Select(v => new SelectListItem { Value = v.Id.ToString(), Text = v.Numero })
                .ToListAsync();

            model.AreasComunes = await _context.AreasComunes
                .OrderBy(a => a.Nombre)
                .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Nombre })
                .ToListAsync();

            var tipos = await _tipoMantenimientoService.ObtenerActivosAsync();
            model.TiposMantenimiento = tipos
                .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Nombre })
                .ToList();

            var personal = await _userManager.GetUsersInRoleAsync("Mantenimiento");
            model.PersonalMantenimiento = personal
                .OrderBy(u => u.UserName)
                .Select(u => new SelectListItem { Value = u.Id, Text = u.UserName })
                .ToList();
        }
    }
}