using Habitia.Services.Interfaces;
using Habitia.ViewModels.Catalogos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TiposMantenimientoController : Controller
    {
        private readonly ITipoMantenimientoService _tipoMantenimientoService;

        public TiposMantenimientoController(ITipoMantenimientoService tipoMantenimientoService)
        {
            _tipoMantenimientoService = tipoMantenimientoService;
        }

        // GET: /Admin/TiposMantenimiento
        public async Task<IActionResult> Index()
        {
            var tipos = await _tipoMantenimientoService.ObtenerActivosAsync();
            return View(tipos);
        }

        // GET: /Admin/TiposMantenimiento/Create
        public IActionResult Create()
        {
            return View(new TipoMantenimientoCreateViewModel());
        }

        // POST: /Admin/TiposMantenimiento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoMantenimientoCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _tipoMantenimientoService.ObtenerOCrearAsync(null, model.Nombre);
                TempData["Success"] = "Tipo de mantenimiento creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(model.Nombre), ex.Message);
                return View(model);
            }
        }

        // Validación AJAX en tiempo real desde el formulario (opcional, ver nota abajo)
        [HttpGet]
        public async Task<IActionResult> ValidarNombre(string nombre)
        {
            var existe = await _tipoMantenimientoService.ExisteNombreAsync(nombre);
            return Json(!existe); // true = válido (no existe), false = inválido (duplicado)
        }
    }
}