using Habitia.Data;
using Habitia.Models.Catalogos;
using Habitia.ViewModels.Documento;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriasDocumentoController : Controller
    {
        private readonly ApplicationDbContext _context;


        public CategoriasDocumentoController(ApplicationDbContext context)
        {
            _context = context;
        }


        // ===== LISTA =====
        public async Task<IActionResult> Index()
        {
            var categorias = await _context.CategoriasDocumento
                .OrderBy(c => c.TC_Nombre)
                .ToListAsync();

            // Cantidad de documentos por categoría, para mostrarla en la tabla
            ViewBag.Conteos = await _context.Documentos
                .GroupBy(d => d.TN_IdCategoria)
                .Select(g => new { Id = g.Key, Total = g.Count() })
                .ToDictionaryAsync(x => x.Id, x => x.Total);

            return View(categorias);
        }


        // ===== CREAR (GET) =====
        public IActionResult Crear()
        {
            return View(new CategoriaDocumentoFormVM());
        }


        // ===== CREAR (POST) =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CategoriaDocumentoFormVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var nombre = vm.Nombre.Trim();

            // Regla de negocio: el sistema evitará duplicidad de categorías
            var existe = await _context.CategoriasDocumento
                .AnyAsync(c => c.TC_Nombre.ToLower() == nombre.ToLower());

            if (existe)
            {
                ModelState.AddModelError(nameof(vm.Nombre),
                    "Ya existe una categoría con ese nombre.");
                return View(vm);
            }

            _context.CategoriasDocumento.Add(new CategoriaDocumento
            {
                TC_Nombre = nombre,
                TB_Estado = true
            });

            await _context.SaveChangesAsync();

            TempData["Exito"] = "Categoría creada correctamente.";
            return RedirectToAction(nameof(Index));
        }


        // ===== EDITAR (GET) =====
        public async Task<IActionResult> Editar(int id)
        {
            var categoria = await _context.CategoriasDocumento
                .FirstOrDefaultAsync(c => c.TN_Id == id);

            if (categoria == null)
                return NotFound();

            return View(new CategoriaDocumentoFormVM
            {
                Id = categoria.TN_Id,
                Nombre = categoria.TC_Nombre
            });
        }


        // ===== EDITAR (POST) =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(CategoriaDocumentoFormVM vm)
        {
            var categoria = await _context.CategoriasDocumento
                .FirstOrDefaultAsync(c => c.TN_Id == vm.Id);

            if (categoria == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(vm);

            var nombre = vm.Nombre.Trim();

            // Duplicado, excluyendo la categoría que se está editando
            var existe = await _context.CategoriasDocumento
                .AnyAsync(c => c.TN_Id != vm.Id &&
                               c.TC_Nombre.ToLower() == nombre.ToLower());

            if (existe)
            {
                ModelState.AddModelError(nameof(vm.Nombre),
                    "Ya existe otra categoría con ese nombre.");
                return View(vm);
            }

            categoria.TC_Nombre = nombre;
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Categoría actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }


        // ===== ACTIVAR / DESACTIVAR =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            var categoria = await _context.CategoriasDocumento
                .FirstOrDefaultAsync(c => c.TN_Id == id);

            if (categoria == null)
                return RedirectToAction(nameof(Index));

            // Si va a desactivarse, verificar que no tenga documentos activos
            if (categoria.TB_Estado)
            {
                var tieneDocumentos = await _context.Documentos
                    .AnyAsync(d => d.TN_IdCategoria == id && d.TB_Estado);

                if (tieneDocumentos)
                {
                    TempData["Error"] =
                        "No se puede desactivar: la categoría tiene documentos activos.";

                    return RedirectToAction(nameof(Index));
                }
            }

            categoria.TB_Estado = !categoria.TB_Estado;
            await _context.SaveChangesAsync();

            TempData["Exito"] = categoria.TB_Estado
                ? "Categoría activada."
                : "Categoría desactivada.";

            return RedirectToAction(nameof(Index));
        }
    }
}