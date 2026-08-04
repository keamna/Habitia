using Habitia.Data;
using Habitia.Models;
using Habitia.ViewModels;
using Habitia.ViewModels.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Controllers
{
    [Authorize]
    public class MiPerfilController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public MiPerfilController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: /MiPerfil
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "" });

            var model = await ConstruirModeloAsync(user);
            return View(model);
        }

        // POST: /MiPerfil/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(MiPerfilViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "" });

            if (!ModelState.IsValid)
            {
                var modeloConDatos = await ConstruirModeloAsync(user);
                CopiarCamposEditables(modeloConDatos, model);
                return View("Index", modeloConDatos);
            }

            var emailNormalizado = model.Email.Trim();

            var emailYaExiste = await _userManager.Users
                .AnyAsync(u => u.Email == emailNormalizado && u.Id != user.Id);

            if (emailYaExiste)
            {
                ModelState.AddModelError(nameof(model.Email), "Ya existe un usuario registrado con este correo.");
                var modeloConDatos = await ConstruirModeloAsync(user);
                CopiarCamposEditables(modeloConDatos, model);
                return View("Index", modeloConDatos);
            }

            user.TC_Nombre = model.Nombre.Trim();
            user.TC_Apellido = model.Apellido.Trim();
            user.Email = emailNormalizado;
            user.UserName = emailNormalizado;
            user.TC_Telefono = model.Telefono.Trim();

            var resultado = await _userManager.UpdateAsync(user);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                var modeloConDatos = await ConstruirModeloAsync(user);
                CopiarCamposEditables(modeloConDatos, model);
                return View("Index", modeloConDatos);
            }

            // Refresca la cookie de sesión, ya que el correo/username cambió
            await _signInManager.RefreshSignInAsync(user);

            TempData["Success"] = "Perfil actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<MiPerfilViewModel> ConstruirModeloAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var rol = roles.Contains("Admin") ? "Admin"
                    : roles.Contains("Seguridad") ? "Seguridad"
                    : roles.Contains("Mantenimiento") ? "Mantenimiento"
                    : roles.Contains("Residente") ? "Residente"
                    : "Sin rol asignado";

            var viviendas = await _context.ViviendaUsuarios
                .Include(v => v.Vivienda)
                .Where(v => v.TC_IdUsuario == user.Id)
                .Select(v => new ViviendaResumenViewModel
                {
                    ViviendaId = v.TN_IdVivienda,
                    Codigo = v.Vivienda.TC_Numero,
                    TipoVivienda = v.Vivienda.TN_Tipo.ToString(),
                    TipoRelacion = v.TN_TipoRelacion.ToString(),
                    Estado = v.TN_Estado.ToString(),
                    ViveAhi = v.TB_ViveAhi
                })
                .ToListAsync();

            return new MiPerfilViewModel
            {
                TipoIdentificacion = user.TN_TipoIdentificacion.ToString(),
                NumeroIdentificacion = user.TC_Identificacion,
                Rol = rol,
                FotoPerfil = user.TC_FotoPerfil,
                Viviendas = viviendas,
                Nombre = user.TC_Nombre,
                Apellido = user.TC_Apellido,
                Email = user.Email,
                Telefono = user.TC_Telefono
            };
        }

        private static void CopiarCamposEditables(MiPerfilViewModel destino, MiPerfilViewModel origen)
        {
            destino.Nombre = origen.Nombre;
            destino.Apellido = origen.Apellido;
            destino.Email = origen.Email;
            destino.Telefono = origen.Telefono;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubirFoto(IFormFile foto, string? returnUrl)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "" });

            if (foto == null || foto.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar una imagen.";
                return Redirect(returnUrl ?? Url.Action(nameof(Index))!);
            }

            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(foto.FileName).ToLowerInvariant();

            if (!extensionesPermitidas.Contains(extension))
            {
                TempData["Error"] = "Solo se permiten imágenes JPG, PNG o WEBP.";
                return Redirect(returnUrl ?? Url.Action(nameof(Index))!);
            }

            const int maxSizeBytes = 5 * 1024 * 1024;
            if (foto.Length > maxSizeBytes)
            {
                TempData["Error"] = "El tamaño máximo permitido es 5 MB.";
                return Redirect(returnUrl ?? Url.Action(nameof(Index))!);
            }

            var carpeta = Path.Combine("wwwroot", "uploads", "perfiles");
            Directory.CreateDirectory(carpeta);

            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await foto.CopyToAsync(stream);
            }

            user.TC_FotoPerfil = $"/uploads/perfiles/{nombreArchivo}";
            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Foto de perfil actualizada correctamente.";
            return Redirect(returnUrl ?? Url.Action(nameof(Index))!);
        }
    }
}