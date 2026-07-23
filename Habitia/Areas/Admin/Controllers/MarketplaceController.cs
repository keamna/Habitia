using Habitia.Data;
using Habitia.Enums;
using Habitia.ViewModels.Marketplace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MarketplaceController : Controller
    {
        private readonly ApplicationDbContext _context;


        public MarketplaceController(ApplicationDbContext context)
        {
            _context = context;
        }


        // ===== LISTA =====
        public async Task<IActionResult> Index(
            string? busqueda,
            TipoPublicacionEnum? tipo,
            int? categoria)
        {
            var query = _context.Publicaciones
                .Include(p => p.Usuario)
                .Include(p => p.Categoria)
                .Include(p => p.Imagenes)
                .Where(p => p.TN_Estado == EstadoPublicacionEnum.Activa)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                query = query.Where(p =>
                    p.TC_Titulo.Contains(busqueda) ||
                    p.TC_Descripcion.Contains(busqueda) ||
                    (p.Usuario.TC_Nombre + " " + p.Usuario.TC_Apellido)
                        .Contains(busqueda));
            }

            if (tipo.HasValue)
                query = query.Where(p => p.TN_Tipo == tipo.Value);

            if (categoria.HasValue)
                query = query.Where(p => p.TN_IdCategoria == categoria.Value);

            var vm = new MarketplaceFiltroVM
            {
                Busqueda = busqueda,
                Tipo = tipo,
                IdCategoria = categoria,

                Publicaciones = await query
                    .OrderByDescending(p => p.TF_FechaPublicacion)
                    .ToListAsync()
            };

            ViewBag.Categorias =
                await _context.CategoriasPublicacion
                    .Where(c => c.TB_Estado)
                    .OrderBy(c => c.TC_Nombre)
                    .ToListAsync();

            return View(vm);
        }


        // ===== DETALLE =====
        public async Task<IActionResult> Detalle(int id)
        {
            var publicacion = await _context.Publicaciones
                .Include(p => p.Usuario)
                .Include(p => p.Categoria)
                .Include(p => p.Imagenes)
                .FirstOrDefaultAsync(p => p.TN_Id == id);

            if (publicacion == null)
                return NotFound();

            return View(publicacion);
        }


        // ===== ELIMINAR =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var publicacion = await _context.Publicaciones
                .FirstOrDefaultAsync(p => p.TN_Id == id);

            if (publicacion != null)
            {
                publicacion.TN_Estado = EstadoPublicacionEnum.Eliminada;
                await _context.SaveChangesAsync();
                TempData["Exito"] = "La publicación fue eliminada correctamente.";
            }
            else
            {
                TempData["Error"] = "No se encontró la publicación.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}