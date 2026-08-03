using Habitia.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Residente.Controllers
{
    [Area("Residente")]
    [Authorize(Roles = "Residente")]
    public class DocumentosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;


        public DocumentosController(
            ApplicationDbContext context,
            IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }


        // ===== LISTA (solo documentos activos) =====
        public async Task<IActionResult> Index(int? categoria, string? busqueda)
        {
            var query = _context.Documentos
                .Include(d => d.Categoria)
                .Where(d => d.TB_Estado)          // el residente solo ve los activos
                .AsQueryable();

            if (categoria.HasValue)
                query = query.Where(d => d.TN_IdCategoria == categoria.Value);

            if (!string.IsNullOrWhiteSpace(busqueda))
                query = query.Where(d =>
                    d.TC_Nombre.Contains(busqueda) ||
                    d.TC_Descripcion.Contains(busqueda));

            // Solo categorías que tengan al menos un documento activo
            ViewBag.Categorias = await _context.CategoriasDocumento
                .Where(c => _context.Documentos
                    .Any(d => d.TN_IdCategoria == c.TN_Id && d.TB_Estado))
                .OrderBy(c => c.TC_Nombre)
                .ToListAsync();

            ViewBag.FiltroCategoria = categoria;
            ViewBag.Busqueda = busqueda;

            return View(await query
                .OrderByDescending(d => d.TF_FechaCarga)
                .ToListAsync());
        }


        // ===== DESCARGAR =====
        public async Task<IActionResult> Descargar(int id)
        {
            // El filtro por estado es obligatorio: sin él, un residente podría
            // descargar un documento desactivado escribiendo el id en la URL.
            var doc = await _context.Documentos
                .FirstOrDefaultAsync(d => d.TN_Id == id && d.TB_Estado);

            if (doc == null)
                return NotFound();

            var rutaFisica = Path.Combine(
                _env.WebRootPath,
                doc.TC_Archivo.TrimStart('/')
                              .Replace('/', Path.DirectorySeparatorChar));

            if (!System.IO.File.Exists(rutaFisica))
            {
                TempData["Error"] = "El archivo ya no está disponible.";
                return RedirectToAction(nameof(Index));
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(rutaFisica);

            return File(bytes, "application/octet-stream", doc.TC_NombreOriginal);
        }
    }
}