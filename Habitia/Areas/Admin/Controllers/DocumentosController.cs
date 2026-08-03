using System.Security.Claims;
using Habitia.Data;
using Habitia.Helpers;
using Habitia.Models.Documentos;
using Habitia.ViewModels.Documento;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
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


        private string UsuarioActualId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";


        // ===== LISTA =====
        public async Task<IActionResult> Index(
            int? categoria, bool? estado, string? busqueda)
        {
            var query = _context.Documentos
                .Include(d => d.Categoria)
                .Include(d => d.Usuario)
                .AsQueryable();

            if (categoria.HasValue)
                query = query.Where(d => d.TN_IdCategoria == categoria.Value);

            if (estado.HasValue)
                query = query.Where(d => d.TB_Estado == estado.Value);

            if (!string.IsNullOrWhiteSpace(busqueda))
                query = query.Where(d =>
                    d.TC_Nombre.Contains(busqueda) ||
                    d.TC_Descripcion.Contains(busqueda));

            ViewBag.FiltroCategoria = categoria;
            ViewBag.FiltroEstado = estado;
            ViewBag.Busqueda = busqueda;

            await CargarCategorias();

            return View(await query
                .OrderByDescending(d => d.TF_FechaCarga)
                .ToListAsync());
        }


        // ===== CREAR (GET) =====
        public async Task<IActionResult> Crear()
        {
            await CargarCategorias();
            return View(new DocumentoFormVM());
        }


        // ===== CREAR (POST) =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(DocumentoFormVM vm)
        {
            if (vm.Archivo == null || vm.Archivo.Length == 0)
                ModelState.AddModelError(nameof(vm.Archivo),
                    "Debe adjuntar un archivo.");

            if (!ModelState.IsValid)
            {
                await CargarCategorias();
                return View(vm);
            }

            try
            {
                var ruta = await ArchivoHelper.GuardarDocumentoAsync(
                    vm.Archivo!, _env.WebRootPath);

                _context.Documentos.Add(new Documento
                {
                    TC_Nombre = vm.Nombre.Trim(),
                    TC_Descripcion = vm.Descripcion?.Trim(),
                    TN_IdCategoria = vm.IdCategoria,
                    TC_Archivo = ruta,
                    TC_NombreOriginal = Path.GetFileName(vm.Archivo!.FileName),
                    TN_Tamano = vm.Archivo.Length,
                    TC_IdUsuario = UsuarioActualId,
                    TB_Estado = true,
                    TF_FechaCarga = DateTime.Now
                });

                await _context.SaveChangesAsync();

                TempData["Exito"] = "Documento cargado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(vm.Archivo), ex.Message);
                await CargarCategorias();
                return View(vm);
            }
        }


        // ===== EDITAR (GET) =====
        public async Task<IActionResult> Editar(int id)
        {
            var doc = await _context.Documentos
                .FirstOrDefaultAsync(d => d.TN_Id == id);

            if (doc == null)
                return NotFound();

            await CargarCategorias();

            return View(new DocumentoFormVM
            {
                Id = doc.TN_Id,
                Nombre = doc.TC_Nombre,
                Descripcion = doc.TC_Descripcion,
                IdCategoria = doc.TN_IdCategoria,
                ArchivoActual = doc.TC_Archivo,
                NombreOriginalActual = doc.TC_NombreOriginal
            });
        }


        // ===== EDITAR (POST) =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(DocumentoFormVM vm)
        {
            var doc = await _context.Documentos
                .FirstOrDefaultAsync(d => d.TN_Id == vm.Id);

            if (doc == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await CargarCategorias();
                vm.ArchivoActual = doc.TC_Archivo;
                vm.NombreOriginalActual = doc.TC_NombreOriginal;
                return View(vm);
            }

            doc.TC_Nombre = vm.Nombre.Trim();
            doc.TC_Descripcion = vm.Descripcion?.Trim();
            doc.TN_IdCategoria = vm.IdCategoria;

            // El archivo solo se reemplaza si subió uno nuevo
            if (vm.Archivo != null && vm.Archivo.Length > 0)
            {
                try
                {
                    var rutaAnterior = doc.TC_Archivo;

                    doc.TC_Archivo = await ArchivoHelper.GuardarDocumentoAsync(
                        vm.Archivo, _env.WebRootPath);

                    doc.TC_NombreOriginal = Path.GetFileName(vm.Archivo.FileName);
                    doc.TN_Tamano = vm.Archivo.Length;

                    ArchivoHelper.EliminarArchivo(rutaAnterior, _env.WebRootPath);
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(nameof(vm.Archivo), ex.Message);
                    await CargarCategorias();
                    vm.ArchivoActual = doc.TC_Archivo;
                    vm.NombreOriginalActual = doc.TC_NombreOriginal;
                    return View(vm);
                }
            }

            await _context.SaveChangesAsync();

            TempData["Exito"] = "Documento actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }


        // ===== ACTIVAR / DESACTIVAR =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            var doc = await _context.Documentos
                .FirstOrDefaultAsync(d => d.TN_Id == id);

            if (doc != null)
            {
                doc.TB_Estado = !doc.TB_Estado;
                await _context.SaveChangesAsync();

                TempData["Exito"] = doc.TB_Estado
                    ? "El documento ahora es visible para los residentes."
                    : "El documento se ocultó para los residentes.";
            }

            return RedirectToAction(nameof(Index));
        }


        // ===== ELIMINAR =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var doc = await _context.Documentos
                .FirstOrDefaultAsync(d => d.TN_Id == id);

            if (doc == null)
                return RedirectToAction(nameof(Index));

            // Regla de negocio: no se elimina si está asociado a una asamblea
            var enUso = await _context.DocumentosAsamblea
                .AnyAsync(da => da.TN_IdDocumento == id);

            if (enUso)
            {
                TempData["Error"] =
                    "No se puede eliminar: el documento está asociado a una asamblea. " +
                    "Puede desactivarlo en su lugar.";

                return RedirectToAction(nameof(Index));
            }

            var ruta = doc.TC_Archivo;

            _context.Documentos.Remove(doc);
            await _context.SaveChangesAsync();

            ArchivoHelper.EliminarArchivo(ruta, _env.WebRootPath);

            TempData["Exito"] = "Documento eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }


        // ===== DESCARGAR =====
        public async Task<IActionResult> Descargar(int id)
        {
            var doc = await _context.Documentos
                .FirstOrDefaultAsync(d => d.TN_Id == id);

            if (doc == null)
                return NotFound();

            var rutaFisica = Path.Combine(
                _env.WebRootPath,
                doc.TC_Archivo.TrimStart('/')
                              .Replace('/', Path.DirectorySeparatorChar));

            if (!System.IO.File.Exists(rutaFisica))
            {
                TempData["Error"] = "El archivo ya no está disponible en el servidor.";
                return RedirectToAction(nameof(Index));
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(rutaFisica);

            return File(bytes, "application/octet-stream", doc.TC_NombreOriginal);
        }


        // ===== HELPERS =====
        private async Task CargarCategorias()
        {
            ViewBag.Categorias = await _context.CategoriasDocumento
                .Where(c => c.TB_Estado)
                .OrderBy(c => c.TC_Nombre)
                .ToListAsync();
        }
    }
}