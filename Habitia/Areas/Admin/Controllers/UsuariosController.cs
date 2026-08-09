using Habitia.Data;
using Habitia.Enums;
using Habitia.Helpers;
using Habitia.Models;
using Habitia.ViewModels;
using Habitia.ViewModels.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            CargarTiposMantenimiento();

            var usuarios = await _userManager.Users.ToListAsync();

            var relaciones = await _context.ViviendaUsuarios
                .Include(v => v.Vivienda)
                .ToListAsync();

            var tiposPorUsuario = await _context.PersonalTipoMantenimiento
                .ToListAsync();

            var tiposMantenimientoTodos = await _context.TiposMantenimiento
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

                var tiposIdsUsuario = tiposPorUsuario
                    .Where(t => t.TC_IdPersonal == user.Id)
                    .Select(t => t.TN_IdTipo)
                    .ToList();

                var tiposNombresUsuario = tiposMantenimientoTodos
                    .Where(tm => tiposIdsUsuario.Contains(tm.TN_Id))
                    .Select(tm => tm.TC_Nombre)
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
                    Viviendas = viviendas,
                    TiposMantenimientoIds = tiposIdsUsuario,
                    TiposMantenimientoNombres = tiposNombresUsuario
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
            CargarRoles();
            CargarTiposMantenimiento();
            return View();
        }

        // ==========================
        // CREAR USUARIO POST
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CrearUsuarioViewModel model)
        {
            CargarRoles();
            CargarTiposMantenimiento();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Rol == "Admin" || model.Rol == "Residente")
            {
                ModelState.AddModelError(
                    nameof(model.Rol),
                    "Debe seleccionar un rol válido (Seguridad o Mantenimiento).");
                return View(model);
            }

            if (model.TipoIdentificacion == null)
            {
                ModelState.AddModelError(
                    nameof(model.TipoIdentificacion),
                    "Debe seleccionar el tipo de identificación.");
                return View(model);
            }

            if (model.Rol == "Mantenimiento" &&
                (model.TiposMantenimientoIds == null || !model.TiposMantenimientoIds.Any()))
            {
                ModelState.AddModelError(
                    nameof(model.TiposMantenimientoIds),
                    "Debe asignar al menos un tipo de mantenimiento.");
                return View(model);
            }

            var usuario = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                TC_Nombre = model.Nombre,
                TC_Apellido = model.Apellido,
                TN_TipoIdentificacion = model.TipoIdentificacion.Value,
                TC_Identificacion = model.Identificacion,
                TC_Telefono = model.Telefono,
                TN_Estado = EstadoUsuarioEnum.Activo,
                TF_FechaRegistro = DateTime.Now
            };

            var resultado = await _userManager.CreateAsync(usuario, model.Password);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            var rolValido = _context.Roles.Any(x => x.Name == model.Rol);

            if (rolValido)
            {
                await _userManager.AddToRoleAsync(usuario, model.Rol);
            }

            if (model.Rol == "Mantenimiento")
            {
                await AsignarTiposMantenimiento(usuario.Id, model.TiposMantenimientoIds!);
            }

            TempData["Success"] = "Usuario creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ==========================
        // CARGAR ROLES / TIPOS
        // ==========================
        private void CargarRoles()
        {
            ViewBag.Roles = new List<string> { "Seguridad", "Mantenimiento" };
        }

        private void CargarTiposMantenimiento()
        {
            ViewBag.TiposMantenimiento = _context.TiposMantenimiento
                .Where(t => t.TB_Estado)
                .OrderBy(t => t.TC_Nombre)
                .Select(t => new SelectListItem
                {
                    Value = t.TN_Id.ToString(),
                    Text = t.TC_Nombre
                })
                .ToList();
        }

        private async Task AsignarTiposMantenimiento(string idUsuario, List<int> tiposIds)
        {
            var actuales = _context.PersonalTipoMantenimiento
                .Where(x => x.TC_IdPersonal == idUsuario);

            _context.PersonalTipoMantenimiento.RemoveRange(actuales);

            foreach (var idTipo in tiposIds.Distinct())
            {
                _context.PersonalTipoMantenimiento.Add(new PersonalTipoMantenimiento
                {
                    TC_IdPersonal = idUsuario,
                    TN_IdTipo = idTipo
                });
            }

            await _context.SaveChangesAsync();
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

            user.TC_Nombre = model.Nombre;
            user.Email = model.Email;
            user.UserName = model.Email;

            var resultado = await _userManager.UpdateAsync(user);

            return Json(new { success = resultado.Succeeded });
        }

        // ==========================
        // EDITAR ROLES
        // ==========================
        [HttpPost]
        public async Task<IActionResult> EditarRoles([FromBody] EditarRolesVM model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            if (model.Rol == "Admin")
            {
                return Json(new { success = false, message = "No se puede asignar el rol Admin." });
            }

            if (string.IsNullOrWhiteSpace(model.Rol))
            {
                return Json(new { success = false, message = "Debe seleccionar un rol." });
            }

            if (model.Rol == "Mantenimiento" &&
                (model.TiposMantenimientoIds == null || !model.TiposMantenimientoIds.Any()))
            {
                return Json(new { success = false, message = "Debe asignar al menos un tipo de mantenimiento." });
            }

            var rolesActuales = await _userManager.GetRolesAsync(user);
            var quitar = rolesActuales.Where(x => x != "Admin");

            if (quitar.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, quitar);
            }

            await _userManager.AddToRoleAsync(user, model.Rol);

            if (model.Rol == "Mantenimiento")
            {
                await AsignarTiposMantenimiento(user.Id, model.TiposMantenimientoIds!);
            }
            else
            {
                var actuales = _context.PersonalTipoMantenimiento
                    .Where(x => x.TC_IdPersonal == user.Id);
                _context.PersonalTipoMantenimiento.RemoveRange(actuales);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        // ==========================
        // APROBAR USUARIO
        // ==========================
        [HttpPost]
        public async Task<IActionResult> Aprobar([FromBody] AprobarSolicitudVM model)
        {
            var usuario = await _userManager.FindByIdAsync(model.Id);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            var viviendaUsuario = await _context.ViviendaUsuarios
                .FirstOrDefaultAsync(x =>
                    x.TC_IdUsuario == model.Id &&
                    x.TN_IdVivienda == model.IdVivienda &&
                    x.TN_Estado == EstadoUsuarioEnum.Pendiente);

            if (viviendaUsuario == null)
            {
                return Json(new { success = false, message = "No se encontró una solicitud pendiente para esa vivienda." });
            }

            usuario.TN_Estado = EstadoUsuarioEnum.Activo;
            usuario.EmailConfirmed = true;
            await _userManager.UpdateAsync(usuario);

            viviendaUsuario.TN_Estado = EstadoUsuarioEnum.Activo;
            await _context.SaveChangesAsync();

            await RecalcularEstadoVivienda(viviendaUsuario.TN_IdVivienda);

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
        public async Task<IActionResult> Rechazar([FromBody] AprobarSolicitudVM model)
        {
            var viviendaUsuario = await _context.ViviendaUsuarios
                .FirstOrDefaultAsync(x =>
                    x.TC_IdUsuario == model.Id &&
                    x.TN_IdVivienda == model.IdVivienda &&
                    x.TN_Estado == EstadoUsuarioEnum.Pendiente);

            if (viviendaUsuario == null)
            {
                return Json(new { success = false, message = "No se encontró una solicitud pendiente para esa vivienda." });
            }

            viviendaUsuario.TN_Estado = EstadoUsuarioEnum.Rechazado;
            await _context.SaveChangesAsync();

            await RecalcularEstadoVivienda(viviendaUsuario.TN_IdVivienda);

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

            var viviendasAfectadas = relaciones
                .Select(x => x.TN_IdVivienda)
                .Distinct()
                .ToList();

            if (relaciones.Any())
            {
                _context.ViviendaUsuarios.RemoveRange(relaciones);
                await _context.SaveChangesAsync();

                foreach (var idVivienda in viviendasAfectadas)
                {
                    await RecalcularEstadoVivienda(idVivienda);
                }
            }

            var tiposMantenimiento = _context.PersonalTipoMantenimiento
                .Where(x => x.TC_IdPersonal == model.Id);
            _context.PersonalTipoMantenimiento.RemoveRange(tiposMantenimiento);
            await _context.SaveChangesAsync();

            var resultado = await _userManager.DeleteAsync(usuario);

            if (!resultado.Succeeded)
            {
                return Json(new { success = false, message = "No se pudo eliminar el usuario" });
            }

            return Json(new { success = true });
        }

        // ==========================
        // SUSPENDER USUARIO
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Suspender(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            if (await _userManager.IsInRoleAsync(usuario, "Admin"))
            {
                TempData["Error"] = "No se puede suspender a un administrador.";
                return RedirectToAction(nameof(Index));
            }

            usuario.TN_Estado = EstadoUsuarioEnum.Suspendido;
            await _userManager.UpdateAsync(usuario);

            // Invalida la sesión activa del usuario (si estaba conectado, se le cierra la sesión)
            await _userManager.UpdateSecurityStampAsync(usuario);

            TempData["Success"] = "Usuario suspendido correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ==========================
        // REACTIVAR USUARIO
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivar(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            usuario.TN_Estado = EstadoUsuarioEnum.Activo;
            await _userManager.UpdateAsync(usuario);

            TempData["Success"] = "Usuario reactivado correctamente.";
            return RedirectToAction(nameof(Index));
        }


        // ==========================
        // RECALCULAR ESTADO DE VIVIENDA
        // ==========================
        private async Task RecalcularEstadoVivienda(int idVivienda)
        {
            var vivienda = await _context.Viviendas
                .FirstOrDefaultAsync(v => v.TN_Id == idVivienda);

            if (vivienda == null)
            {
                return;
            }

            var cantidadActivos = await _context.ViviendaUsuarios
                .CountAsync(x =>
                    x.TN_IdVivienda == idVivienda &&
                    x.TN_Estado == EstadoUsuarioEnum.Activo);

            vivienda.TN_Estado = ViviendaHelper.RecalcularEstado(vivienda.TN_Estado, cantidadActivos);

            await _context.SaveChangesAsync();
        }
    }
}