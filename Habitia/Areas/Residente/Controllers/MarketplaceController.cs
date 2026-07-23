using System.Security.Claims;
using Habitia.Data;
using Habitia.Enums;
using Habitia.Models;
using Habitia.ViewModels.Marketplace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Residente.Controllers
{
    [Area("Residente")]
    [Authorize(Roles = "Residente")]
    public class MarketplaceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;


        public MarketplaceController(
            ApplicationDbContext context,
            IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }


        private string UsuarioActualId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";


        // ===== LISTA (activas + finalizadas) =====
        public async Task<IActionResult> Index(
            string? busqueda,
            TipoPublicacionEnum? tipo,
            int? categoria,
            string? vendedor,
            bool mias = false)
        {
            var query = _context.Publicaciones
                .Include(p => p.Usuario)
                .Include(p => p.Categoria)
                .Include(p => p.Imagenes)
                .Include(p => p.Resenas)
                .Where(p => p.TN_Estado == EstadoPublicacionEnum.Activa ||
                            p.TN_Estado == EstadoPublicacionEnum.Finalizada)
                .AsQueryable();


            // Busca en título, descripción, especificaciones,
            // categoría y nombre del residente
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                query = query.Where(p =>
                    p.TC_Titulo.Contains(busqueda) ||
                    p.TC_Descripcion.Contains(busqueda) ||
                    p.TC_Especificaciones.Contains(busqueda) ||
                    p.Categoria.TC_Nombre.Contains(busqueda) ||
                    (p.Usuario.TC_Nombre + " " + p.Usuario.TC_Apellido)
                        .Contains(busqueda));
            }


            if (!string.IsNullOrWhiteSpace(vendedor))
                query = query.Where(p => p.TC_IdUsuario == vendedor);


            if (tipo.HasValue)
                query = query.Where(p => p.TN_Tipo == tipo.Value);

            if (categoria.HasValue)
                query = query.Where(p => p.TN_IdCategoria == categoria.Value);

            if (mias)
                query = query.Where(p => p.TC_IdUsuario == UsuarioActualId);


            var vm = new MarketplaceFiltroVM
            {
                Busqueda = busqueda,
                Tipo = tipo,
                IdCategoria = categoria,
                IdVendedor = vendedor,
                SoloMias = mias,

                Publicaciones = await query
                    .OrderBy(p => p.TN_Estado)
                    .ThenByDescending(p => p.TF_FechaPublicacion)
                    .ToListAsync(),

                MiPerfil = await ObtenerPerfil(UsuarioActualId)
            };


            if (!string.IsNullOrWhiteSpace(vendedor))
                vm.VendedorFiltrado = await ObtenerPerfil(vendedor);


            if (!string.IsNullOrWhiteSpace(busqueda))
                vm.Vendedores = await BuscarVendedores(busqueda);


            await CargarCategorias();

            return View(vm);
        }

        // ===== SUGERENCIAS DEL BUSCADOR =====
        [HttpGet]
        public async Task<IActionResult> Sugerencias(string? q)
        {
            var lista = new List<SugerenciaVM>();

            if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2)
                return Json(lista);

            q = q.Trim();


            // Publicaciones que coinciden
            var publicaciones = await _context.Publicaciones
                .Include(p => p.Categoria)
                .Where(p => p.TN_Estado == EstadoPublicacionEnum.Activa &&
                            (p.TC_Titulo.Contains(q) ||
                             p.TC_Descripcion.Contains(q)))
                .OrderBy(p => p.TC_Titulo)
                .Take(6)
                .ToListAsync();

            foreach (var p in publicaciones)
            {
                lista.Add(new SugerenciaVM
                {
                    Tipo = "publicacion",
                    Id = p.TN_Id.ToString(),
                    Texto = p.TC_Titulo,
                    Extra = p.Categoria != null ? p.Categoria.TC_Nombre : ""
                });
            }


            // Categorías que coinciden
            var categorias = await _context.CategoriasPublicacion
                .Where(c => c.TB_Estado && c.TC_Nombre.Contains(q))
                .OrderBy(c => c.TC_Nombre)
                .Take(4)
                .ToListAsync();

            foreach (var c in categorias)
            {
                lista.Add(new SugerenciaVM
                {
                    Tipo = "categoria",
                    Id = c.TN_Id.ToString(),
                    Texto = c.TC_Nombre,
                    Extra = "Ver categoría"
                });
            }


            // Residentes que coinciden
            var idsConPublicaciones = await _context.Publicaciones
                .Where(p => p.TN_Estado == EstadoPublicacionEnum.Activa)
                .Select(p => p.TC_IdUsuario)
                .Distinct()
                .ToListAsync();

            var residentes = await _context.Users
                .Where(u => idsConPublicaciones.Contains(u.Id) &&
                            (u.TC_Nombre + " " + u.TC_Apellido).Contains(q))
                .OrderBy(u => u.TC_Nombre)
                .Take(4)
                .ToListAsync();

            foreach (var u in residentes)
            {
                lista.Add(new SugerenciaVM
                {
                    Tipo = "residente",
                    Id = u.Id,
                    Texto = u.TC_Nombre + " " + u.TC_Apellido,
                    Extra = "Residente",
                    Foto = u.TC_FotoPerfil
                });
            }


            return Json(lista);
        }

        // ===== DETALLE =====
        public async Task<IActionResult> Detalle(int id)
        {
            var publicacion = await _context.Publicaciones
                .Include(p => p.Usuario)
                .Include(p => p.Categoria)
                .Include(p => p.Imagenes)
                .FirstOrDefaultAsync(p =>
                    p.TN_Id == id &&
                    p.TN_Estado != EstadoPublicacionEnum.Eliminada);

            if (publicacion == null)
                return NotFound();


            var esPropia = publicacion.TC_IdUsuario == UsuarioActualId;


            if (publicacion.TN_Estado == EstadoPublicacionEnum.Finalizada && !esPropia)
            {
                TempData["Error"] =
                    "Esta publicación ya no está disponible.";

                return RedirectToAction(nameof(Index));
            }


            var resenas = await _context.ResenasPublicacion
                .Include(r => r.Usuario)
                .Where(r => r.TN_IdPublicacion == id)
                .OrderByDescending(r => r.TF_FechaResena)
                .ToListAsync();


            var vm = new DetallePublicacionVM
            {
                Publicacion = publicacion,
                EsPropia = esPropia,
                Vendedor = await ObtenerPerfil(publicacion.TC_IdUsuario),
                Resenas = resenas,

                PromedioPublicacion = resenas.Any()
                    ? resenas.Average(r => r.TN_Calificacion)
                    : 0,

                MiResena = resenas
                    .FirstOrDefault(r => r.TC_IdUsuario == UsuarioActualId)
            };


            return View(vm);
        }


        // ===== GUARDAR RESEÑA =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resenar(ResenaFormVM vm)
        {
            var publicacion = await _context.Publicaciones
                .FirstOrDefaultAsync(p =>
                    p.TN_Id == vm.IdPublicacion &&
                    p.TN_Estado == EstadoPublicacionEnum.Activa);

            if (publicacion == null)
            {
                TempData["Error"] = "Esta publicación ya no está disponible.";
                return RedirectToAction(nameof(Index));
            }


            if (publicacion.TC_IdUsuario == UsuarioActualId)
            {
                TempData["Error"] = "No puedes calificar tu propia publicación.";
                return RedirectToAction(nameof(Detalle), new { id = vm.IdPublicacion });
            }


            if (vm.Calificacion < 1 || vm.Calificacion > 5)
            {
                TempData["Error"] = "Seleccione una calificación válida.";
                return RedirectToAction(nameof(Detalle), new { id = vm.IdPublicacion });
            }


            var existente = await _context.ResenasPublicacion
                .FirstOrDefaultAsync(r =>
                    r.TN_IdPublicacion == vm.IdPublicacion &&
                    r.TC_IdUsuario == UsuarioActualId);


            if (existente != null)
            {
                existente.TN_Calificacion = vm.Calificacion;
                existente.TC_Comentario = vm.Comentario;
                existente.TF_FechaResena = DateTime.Now;

                TempData["Exito"] = "Tu reseña fue actualizada.";
            }
            else
            {
                _context.ResenasPublicacion.Add(new ResenaPublicacion
                {
                    TN_IdPublicacion = vm.IdPublicacion,
                    TC_IdUsuario = UsuarioActualId,
                    TN_Calificacion = vm.Calificacion,
                    TC_Comentario = vm.Comentario,
                    TF_FechaResena = DateTime.Now
                });

                TempData["Exito"] = "¡Gracias por tu reseña!";
            }


            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Detalle), new { id = vm.IdPublicacion });
        }


        // ===== ELIMINAR MI RESEÑA =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarResena(int id, int publicacionId)
        {
            var resena = await _context.ResenasPublicacion
                .FirstOrDefaultAsync(r =>
                    r.TN_Id == id &&
                    r.TC_IdUsuario == UsuarioActualId);

            if (resena != null)
            {
                _context.ResenasPublicacion.Remove(resena);
                await _context.SaveChangesAsync();

                TempData["Exito"] = "Reseña eliminada.";
            }

            return RedirectToAction(nameof(Detalle), new { id = publicacionId });
        }


        // ===== SUBIR FOTO DE PERFIL =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubirFoto(IFormFile foto, string? returnUrl)
        {
            if (foto == null || foto.Length == 0)
            {
                TempData["Error"] = "Seleccione una imagen.";
                return RedirectToAction(nameof(Index));
            }


            var extension = Path.GetExtension(foto.FileName).ToLowerInvariant();

            if (extension != ".jpg" && extension != ".jpeg" &&
                extension != ".png" && extension != ".webp")
            {
                TempData["Error"] = "Formato no válido. Use JPG, PNG o WEBP.";
                return RedirectToAction(nameof(Index));
            }


            var carpeta = Path.Combine(_env.WebRootPath, "uploads", "perfiles");

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);


            var nombre = Guid.NewGuid() + extension;
            var ruta = Path.Combine(carpeta, nombre);

            using (var stream = new FileStream(ruta, FileMode.Create))
            {
                await foto.CopyToAsync(stream);
            }


            var usuario = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == UsuarioActualId);

            if (usuario != null)
            {
                usuario.TC_FotoPerfil = "/uploads/perfiles/" + nombre;
                await _context.SaveChangesAsync();

                TempData["Exito"] = "Foto de perfil actualizada.";
            }


            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Index));
        }


        // ===== QUITAR FOTO DE PERFIL =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuitarFoto()
        {
            var usuario = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == UsuarioActualId);

            if (usuario != null)
            {
                usuario.TC_FotoPerfil = null;
                await _context.SaveChangesAsync();

                TempData["Exito"] = "Foto de perfil eliminada.";
            }

            return RedirectToAction(nameof(Index));
        }


        // ===== CREAR GET =====
        public async Task<IActionResult> Crear()
        {
            await CargarCategorias();
            return View(new PublicacionFormVM());
        }


        // ===== CREAR POST =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(PublicacionFormVM vm)
        {
            vm.ArmarHoras();

            ValidarSegunTipo(vm);

            if (!ModelState.IsValid)
            {
                await CargarCategorias();
                return View(vm);
            }

            var publicacion = new Publicacion
            {
                TC_IdUsuario = UsuarioActualId,
                TN_IdCategoria = vm.IdCategoria,
                TC_Titulo = vm.Titulo,
                TC_Descripcion = vm.Descripcion,
                TN_Precio = vm.Precio ?? 0,
                TN_Tipo = vm.Tipo,
                TN_Estado = EstadoPublicacionEnum.Activa,
                TF_FechaPublicacion = DateTime.Now,

                TC_Contacto = vm.Contacto,

                TC_Especificaciones =
                    vm.Tipo == TipoPublicacionEnum.Producto
                        ? vm.Especificaciones : null,

                TF_FechaServicio =
                    vm.Tipo == TipoPublicacionEnum.Servicio
                        ? vm.FechaServicio : null,

                TT_HoraInicioServicio =
                    vm.Tipo == TipoPublicacionEnum.Servicio
                        ? vm.HoraInicioServicio : null,

                TT_HoraFinServicio =
                    vm.Tipo == TipoPublicacionEnum.Servicio
                        ? vm.HoraFinServicio : null
            };

            _context.Publicaciones.Add(publicacion);
            await _context.SaveChangesAsync();

            await GuardarImagenes(vm.Imagenes, publicacion.TN_Id);

            TempData["Exito"] = "Publicación creada correctamente.";

            return RedirectToAction(nameof(Index));
        }


        // ===== EDITAR GET =====
        public async Task<IActionResult> Editar(int id)
        {
            var publicacion = await _context.Publicaciones
                .Include(p => p.Imagenes)
                .FirstOrDefaultAsync(p =>
                    p.TN_Id == id &&
                    p.TC_IdUsuario == UsuarioActualId &&
                    p.TN_Estado != EstadoPublicacionEnum.Eliminada);

            if (publicacion == null)
                return NotFound();

            var vm = new PublicacionFormVM
            {
                Id = publicacion.TN_Id,
                Titulo = publicacion.TC_Titulo,
                Descripcion = publicacion.TC_Descripcion,
                Tipo = publicacion.TN_Tipo,
                IdCategoria = publicacion.TN_IdCategoria,
                Precio = publicacion.TN_Precio,
                Especificaciones = publicacion.TC_Especificaciones,
                Contacto = publicacion.TC_Contacto,
                FechaServicio = publicacion.TF_FechaServicio,
                HoraInicioServicio = publicacion.TT_HoraInicioServicio,
                HoraFinServicio = publicacion.TT_HoraFinServicio,

                ImagenesExistentes = publicacion.Imagenes.ToList()
            };

            vm.DesarmarHoras();

            await CargarCategorias();

            return View(vm);
        }


        // ===== EDITAR POST =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(PublicacionFormVM vm)
        {
            var publicacion = await _context.Publicaciones
                .FirstOrDefaultAsync(p =>
                    p.TN_Id == vm.Id &&
                    p.TC_IdUsuario == UsuarioActualId &&
                    p.TN_Estado != EstadoPublicacionEnum.Eliminada);

            if (publicacion == null)
                return NotFound();

            vm.ArmarHoras();

            ValidarSegunTipo(vm);

            if (!ModelState.IsValid)
            {
                await CargarCategorias();

                vm.ImagenesExistentes =
                    await _context.ImagenesPublicacion
                        .Where(i => i.TN_IdPublicacion == vm.Id)
                        .ToListAsync();

                return View(vm);
            }

            publicacion.TC_Titulo = vm.Titulo;
            publicacion.TC_Descripcion = vm.Descripcion;
            publicacion.TN_Tipo = vm.Tipo;
            publicacion.TN_IdCategoria = vm.IdCategoria;
            publicacion.TN_Precio = vm.Precio ?? 0;
            publicacion.TC_Contacto = vm.Contacto;

            publicacion.TC_Especificaciones =
                vm.Tipo == TipoPublicacionEnum.Producto
                    ? vm.Especificaciones : null;

            publicacion.TF_FechaServicio =
                vm.Tipo == TipoPublicacionEnum.Servicio
                    ? vm.FechaServicio : null;

            publicacion.TT_HoraInicioServicio =
                vm.Tipo == TipoPublicacionEnum.Servicio
                    ? vm.HoraInicioServicio : null;

            publicacion.TT_HoraFinServicio =
                vm.Tipo == TipoPublicacionEnum.Servicio
                    ? vm.HoraFinServicio : null;

            await GuardarImagenes(vm.Imagenes, publicacion.TN_Id);

            await _context.SaveChangesAsync();

            TempData["Exito"] = "Publicación actualizada correctamente.";

            return RedirectToAction(nameof(Detalle), new { id = publicacion.TN_Id });
        }


        // ===== FINALIZAR =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(int id)
        {
            var publicacion = await _context.Publicaciones
                .FirstOrDefaultAsync(p =>
                    p.TN_Id == id &&
                    p.TC_IdUsuario == UsuarioActualId);

            if (publicacion != null)
            {
                publicacion.TN_Estado = EstadoPublicacionEnum.Finalizada;
                await _context.SaveChangesAsync();

                TempData["Exito"] =
                    "La publicación se marcó como no disponible.";
            }

            return RedirectToAction(nameof(Detalle), new { id });
        }


        // ===== REACTIVAR =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivar(int id)
        {
            var publicacion = await _context.Publicaciones
                .FirstOrDefaultAsync(p =>
                    p.TN_Id == id &&
                    p.TC_IdUsuario == UsuarioActualId &&
                    p.TN_Estado == EstadoPublicacionEnum.Finalizada);

            if (publicacion != null)
            {
                publicacion.TN_Estado = EstadoPublicacionEnum.Activa;
                await _context.SaveChangesAsync();

                TempData["Exito"] =
                    "La publicación está disponible de nuevo.";
            }

            return RedirectToAction(nameof(Detalle), new { id });
        }


        // ===== ELIMINAR =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var publicacion = await _context.Publicaciones
                .FirstOrDefaultAsync(p =>
                    p.TN_Id == id &&
                    p.TC_IdUsuario == UsuarioActualId);

            if (publicacion != null)
            {
                publicacion.TN_Estado = EstadoPublicacionEnum.Eliminada;
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Publicación eliminada.";
            }

            return RedirectToAction(nameof(Index));
        }


        // ===== ELIMINAR IMAGEN =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarImagen(int imagenId, int publicacionId)
        {
            var imagen = await _context.ImagenesPublicacion
                .FirstOrDefaultAsync(i => i.TN_Id == imagenId);

            if (imagen != null)
            {
                var publicacion = await _context.Publicaciones
                    .FirstOrDefaultAsync(p =>
                        p.TN_Id == imagen.TN_IdPublicacion &&
                        p.TC_IdUsuario == UsuarioActualId);

                if (publicacion != null)
                {
                    _context.ImagenesPublicacion.Remove(imagen);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Editar), new { id = publicacionId });
        }


        // ===== HELPERS =====

        private async Task<List<PerfilVendedorVM>> BuscarVendedores(string busqueda)
        {
            var idsConPublicaciones = await _context.Publicaciones
                .Where(p => p.TN_Estado == EstadoPublicacionEnum.Activa)
                .Select(p => p.TC_IdUsuario)
                .Distinct()
                .ToListAsync();


            var usuarios = await _context.Users
                .Where(u => idsConPublicaciones.Contains(u.Id) &&
                            (u.TC_Nombre + " " + u.TC_Apellido).Contains(busqueda))
                .OrderBy(u => u.TC_Nombre)
                .Take(8)
                .ToListAsync();


            var lista = new List<PerfilVendedorVM>();

            foreach (var u in usuarios)
                lista.Add(await ObtenerPerfil(u.Id));


            return lista;
        }


        private async Task<PerfilVendedorVM> ObtenerPerfil(string idUsuario)
        {
            var usuario = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == idUsuario);


            var idsPublicaciones = await _context.Publicaciones
                .Where(p => p.TC_IdUsuario == idUsuario &&
                            p.TN_Estado == EstadoPublicacionEnum.Activa)
                .Select(p => p.TN_Id)
                .ToListAsync();


            var calificaciones = await _context.ResenasPublicacion
                .Where(r => idsPublicaciones.Contains(r.TN_IdPublicacion))
                .Select(r => r.TN_Calificacion)
                .ToListAsync();


            return new PerfilVendedorVM
            {
                Id = idUsuario,

                NombreCompleto = usuario != null
                    ? usuario.TC_Nombre + " " + usuario.TC_Apellido
                    : "Residente",

                FotoPerfil = usuario?.TC_FotoPerfil,

                TotalPublicaciones = idsPublicaciones.Count,

                TotalResenas = calificaciones.Count,

                PromedioEstrellas = calificaciones.Any()
                    ? calificaciones.Average()
                    : 0
            };
        }


        private void ValidarSegunTipo(PublicacionFormVM vm)
        {
            if (vm.Tipo == TipoPublicacionEnum.Producto)
            {
                if (!vm.Precio.HasValue)
                    ModelState.AddModelError(nameof(vm.Precio),
                        "El precio es obligatorio para un producto.");
            }
            else if (vm.Tipo == TipoPublicacionEnum.Servicio)
            {
                if (!vm.FechaServicio.HasValue)
                    ModelState.AddModelError(nameof(vm.FechaServicio),
                        "Indique la fecha en que el servicio está disponible.");

                if (!vm.HoraInicioServicio.HasValue)
                    ModelState.AddModelError(nameof(vm.HoraInicioH),
                        "Indique la hora de inicio.");

                if (!vm.HoraFinServicio.HasValue)
                    ModelState.AddModelError(nameof(vm.HoraFinH),
                        "Indique la hora de fin.");

                if (vm.HoraInicioServicio.HasValue &&
                    vm.HoraFinServicio.HasValue &&
                    vm.HoraFinServicio <= vm.HoraInicioServicio)
                    ModelState.AddModelError(nameof(vm.HoraFinH),
                        "La hora de fin debe ser mayor a la de inicio.");
            }
        }


        private async Task CargarCategorias()
        {
            ViewBag.Categorias =
                await _context.CategoriasPublicacion
                    .Where(c => c.TB_Estado)
                    .OrderBy(c => c.TC_Nombre)
                    .ToListAsync();
        }


        private async Task GuardarImagenes(List<IFormFile>? imagenes, int publicacionId)
        {
            if (imagenes == null || imagenes.Count == 0)
                return;

            var carpeta = Path.Combine(_env.WebRootPath, "uploads", "marketplace");

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            foreach (var imagen in imagenes)
            {
                if (imagen.Length == 0)
                    continue;

                var extension = Path.GetExtension(imagen.FileName).ToLowerInvariant();

                if (extension != ".jpg" && extension != ".jpeg" &&
                    extension != ".png" && extension != ".webp")
                    continue;

                var nombre = Guid.NewGuid() + extension;
                var ruta = Path.Combine(carpeta, nombre);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }

                _context.ImagenesPublicacion.Add(
                    new ImagenPublicacion
                    {
                        TN_IdPublicacion = publicacionId,
                        TC_Url = "/uploads/marketplace/" + nombre
                    });
            }

            await _context.SaveChangesAsync();
        }
    }
}