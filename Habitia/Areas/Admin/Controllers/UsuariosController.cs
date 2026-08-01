using Habitia.Data;
using Habitia.Enums;
using Habitia.Models;
using Habitia.ViewModels;
using Habitia.ViewModels.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsuariosController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UsuariosController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // ==========================
        // LISTADO DE USUARIOS
        // ==========================

        public async Task<IActionResult> Index()
        {
            var usuarios = await _userManager.Users.ToListAsync();

            var relaciones = await _context.ViviendaUsuarios
                .Include(v => v.Vivienda)
                .ToListAsync();

            var lista = new List<UsuarioListItemViewModel>();

            foreach (var user in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var viviendas = relaciones
                    .Where(x => x.TC_IdUsuario == user.Id)
                    .Select(x => new ViviendaResumenViewModel
                    {
                        ViviendaId = x.TN_IdVivienda,
                        Codigo = x.Vivienda.TC_Numero,
                        TipoVivienda = x.Vivienda.TN_Tipo.ToString(),
                        TipoRelacion = x.TN_TipoRelacion.ToString(),
                        Estado = x.TN_Estado.ToString(),
                        ViveAhi = x.TB_ViveAhi
                    })
                    .ToList();

                lista.Add(new UsuarioListItemViewModel
                {
                    Id = user.Id,
                    TipoIdentificacion = user.TN_TipoIdentificacion.ToString(),
                    NumeroIdentificacion = user.TC_Identificacion,
                    NombreCompleto = user.TC_Nombre + " " + user.TC_Apellido,
                    Email = user.Email,
                    Telefono = user.TC_Telefono,
                    Estado = user.TN_Estado.ToString(),
                    Roles = roles.ToList(),
                    Viviendas = viviendas
                });
            }

            return View(lista);
        }

        // ==========================
        // CREAR USUARIO GET
        // ==========================

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        // ==========================
        // CREAR USUARIO POST
        // ==========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CrearUsuarioViewModel model)
        {
            // Seguridad: solo se permite crear Seguridad o Mantenimiento desde este formulario.
            // Admin nunca se crea aquí; Residente se autoregistra y se aprueba con Aprobar().
            if (model.Rol != "Seguridad" && model.Rol != "Mantenimiento")
            {
                ModelState.AddModelError(nameof(model.Rol), "Debe seleccionar un rol válido (Seguridad o Mantenimiento).");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var identificacionNormalizada = model.Identificacion.Trim();

            var yaExiste = await _userManager.Users
                .AnyAsync(u => u.TC_Identificacion == identificacionNormalizada);

            if (yaExiste)
            {
                ModelState.AddModelError(nameof(model.Identificacion), "Ya existe un usuario registrado con esta identificación.");
                return View(model);
            }

            var emailYaExiste = await _userManager.Users
                .AnyAsync(u => u.Email == model.Email);

            if (emailYaExiste)
            {
                ModelState.AddModelError(nameof(model.Email), "Ya existe un usuario registrado con este correo.");
                return View(model);
            }

            var usuario = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                TC_Nombre = model.Nombre.Trim(),
                TC_Apellido = model.Apellido.Trim(),
                TN_TipoIdentificacion = model.TipoIdentificacion!.Value,
                TC_Identificacion = identificacionNormalizada,
                TC_Telefono = model.Telefono.Trim(),
                TN_Estado = EstadoUsuarioEnum.Activo,
                TF_FechaRegistro = DateTime.Now
            };

            var resultado = await _userManager.CreateAsync(usuario, model.Password);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _userManager.AddToRoleAsync(usuario, model.Rol);

            TempData["Success"] = "Usuario registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ==========================
        // EDITAR DATOS
        // ==========================

        [HttpPost]
        public async Task<IActionResult> Editar([FromBody] EditarUsuarioVM model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            if (string.IsNullOrWhiteSpace(model.Nombre) || string.IsNullOrWhiteSpace(model.Email))
            {
                return Json(new { success = false, message = "Debe completar todos los campos obligatorios." });
            }

            user.TC_Nombre = model.Nombre.Trim();
            user.Email = model.Email.Trim();
            user.UserName = model.Email.Trim();

            var resultado = await _userManager.UpdateAsync(user);

            return Json(new { success = resultado.Succeeded });
        }

        // ==========================
        // EDITAR ROLES (un solo rol, solo Seguridad/Mantenimiento)
        // ==========================

        [HttpPost]
        public async Task<IActionResult> EditarRoles([FromBody] EditarRolesVM model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            // Los residentes no pueden cambiar de rol desde acá
            if (await _userManager.IsInRoleAsync(user, "Residente"))
            {
                return Json(new { success = false, message = "No se puede modificar el rol de un residente." });
            }

            // Nunca permitir asignar Admin, y solo se permite un único rol
            if (model.Rol != "Seguridad" && model.Rol != "Mantenimiento")
            {
                return Json(new { success = false, message = "Debe seleccionar un rol válido (Seguridad o Mantenimiento)." });
            }

            var rolesActuales = await _userManager.GetRolesAsync(user);

            if (rolesActuales.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, rolesActuales);
            }

            await _userManager.AddToRoleAsync(user, model.Rol);

            return Json(new { success = true });
        }

        // ==========================
        // APROBAR USUARIO
        // ==========================

        [HttpPost]
        public async Task<IActionResult> Aprobar([FromBody] IdUsuarioVM model)
        {
            var usuario = await _userManager.FindByIdAsync(model.Id);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            usuario.TN_Estado = EstadoUsuarioEnum.Activo;
            usuario.EmailConfirmed = true;

            await _userManager.UpdateAsync(usuario);

            var viviendaUsuario = await _context.ViviendaUsuarios
                .FirstOrDefaultAsync(x => x.TC_IdUsuario == model.Id);

            if (viviendaUsuario != null)
            {
                viviendaUsuario.TN_Estado = EstadoUsuarioEnum.Activo;
                _context.ViviendaUsuarios.Update(viviendaUsuario);
                await _context.SaveChangesAsync();
            }

            if (!await _userManager.IsInRoleAsync(usuario, "Residente"))
            {
                await _userManager.AddToRoleAsync(usuario, "Residente");
            }

            return Json(new { success = true });
        }

        // ==========================
        // RECHAZAR USUARIO
        // ==========================

        [HttpPost]
        public async Task<IActionResult> Rechazar([FromBody] IdUsuarioVM model)
        {
            var usuario = await _userManager.FindByIdAsync(model.Id);

            if (usuario == null)
            {
                return Json(new { success = false });
            }

            usuario.TN_Estado = EstadoUsuarioEnum.Rechazado;
            await _userManager.UpdateAsync(usuario);

            var viviendaUsuario = await _context.ViviendaUsuarios
                .FirstOrDefaultAsync(x => x.TC_IdUsuario == model.Id);

            if (viviendaUsuario != null)
            {
                viviendaUsuario.TN_Estado = EstadoUsuarioEnum.Rechazado;
                _context.ViviendaUsuarios.Update(viviendaUsuario);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        // ==========================
        // SUSPENDER USUARIO
        // ==========================

        [HttpPost]
        public async Task<IActionResult> Suspender([FromBody] IdUsuarioVM model)
        {
            var usuario = await _userManager.FindByIdAsync(model.Id);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            if (await _userManager.IsInRoleAsync(usuario, "Admin"))
            {
                return Json(new { success = false, message = "No se puede suspender a un administrador." });
            }

            usuario.TN_Estado = EstadoUsuarioEnum.Suspendido;

            var resultado = await _userManager.UpdateAsync(usuario);

            if (!resultado.Succeeded)
            {
                return Json(new { success = false, message = "No se pudo suspender el usuario." });
            }

            // Invalida cualquier sesión activa del usuario suspendido
            await _userManager.UpdateSecurityStampAsync(usuario);

            return Json(new { success = true });
        }

        // ==========================
        // REACTIVAR USUARIO
        // ==========================

        [HttpPost]
        public async Task<IActionResult> Reactivar([FromBody] IdUsuarioVM model)
        {
            var usuario = await _userManager.FindByIdAsync(model.Id);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            usuario.TN_Estado = EstadoUsuarioEnum.Activo;

            var resultado = await _userManager.UpdateAsync(usuario);

            if (!resultado.Succeeded)
            {
                return Json(new { success = false, message = "No se pudo reactivar el usuario." });
            }

            return Json(new { success = true });
        }

        // ==========================
        // ELIMINAR USUARIO
        // ==========================

        [HttpPost]
        public async Task<IActionResult> Eliminar([FromBody] IdUsuarioVM model)
        {
            var usuario = await _userManager.FindByIdAsync(model.Id);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            if (await _userManager.IsInRoleAsync(usuario, "Admin"))
            {
                return Json(new { success = false, message = "No se puede eliminar a un administrador" });
            }

            var relaciones = await _context.ViviendaUsuarios
                .Where(x => x.TC_IdUsuario == model.Id)
                .ToListAsync();

            if (relaciones.Any())
            {
                _context.ViviendaUsuarios.RemoveRange(relaciones);
                await _context.SaveChangesAsync();
            }

            var resultado = await _userManager.DeleteAsync(usuario);

            if (!resultado.Succeeded)
            {
                return Json(new { success = false, message = "No se pudo eliminar el usuario" });
            }

            return Json(new { success = true });
        }
    }
}