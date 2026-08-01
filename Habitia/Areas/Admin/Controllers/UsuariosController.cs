using Habitia.Data;
using Habitia.Enums;
using Habitia.Helpers;
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

            var usuarios = await _userManager.Users
                .ToListAsync();



            var relaciones = await _context.ViviendaUsuarios
                .Include(v => v.Vivienda)
                .ToListAsync();



            var lista = new List<UsuarioListItemViewModel>();



            foreach (var user in usuarios)
            {

                var roles =
                    await _userManager.GetRolesAsync(user);



                var viviendas = relaciones
                    .Where(x => x.TC_IdUsuario == user.Id)
                    .Select(x => new ViviendaResumenViewModel
                    {

                        ViviendaId = x.TN_IdVivienda,
                        Codigo = x.Vivienda.TC_Numero,

                        TipoVivienda =
                            x.Vivienda.TN_Tipo.ToString(),
                        TipoRelacion =
                            x.TN_TipoRelacion.ToString(),

                        Estado =
                            x.TN_Estado.ToString(),

                        ViveAhi =
                            x.TB_ViveAhi

                    })
                    .ToList();



                lista.Add(new UsuarioListItemViewModel
                {

                    Id = user.Id,

                    TipoIdentificacion =
                        user.TN_TipoIdentificacion.ToString(),

                    NumeroIdentificacion =
                        user.TC_Identificacion,

                    NombreCompleto =
                        user.TC_Nombre + " " + user.TC_Apellido,

                    Email =
                        user.Email,

                    Telefono =
                        user.TC_Telefono,

                    Estado =
                        user.TN_Estado.ToString(),

                    Roles =
                        roles.ToList(),

                    Viviendas =
                        viviendas

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

            return View();

        }








        // ==========================
        // CREAR USUARIO POST
        // ==========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            CrearUsuarioViewModel model)
        {


            CargarRoles();



            if (!ModelState.IsValid)
            {
                return View(model);
            }






            // Seguridad:
            // nunca permitir crear Admin ni Residente desde este formulario.
            // Residente se asigna solo mediante el flujo de Aprobar() vinculado
            // a una vivienda; Admin nunca se crea desde aquí.

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







            var usuario = new ApplicationUser
            {

                UserName = model.Email,

                Email = model.Email,


                EmailConfirmed = true,


                TC_Nombre = model.Nombre,


                TC_Apellido = model.Apellido,

                TN_TipoIdentificacion =
                    model.TipoIdentificacion.Value,


                TC_Identificacion =
                    model.Identificacion,


                TC_Telefono =
                    model.Telefono,


                TN_Estado =
                    EstadoUsuarioEnum.Activo,


                TF_FechaRegistro =
                    DateTime.Now

            };








            var resultado =
                await _userManager.CreateAsync(
                    usuario,
                    model.Password);






            if (!resultado.Succeeded)
            {

                foreach (var error in resultado.Errors)
                {

                    ModelState.AddModelError(
                        "",
                        error.Description);

                }


                return View(model);

            }







            var rolValido =
                _context.Roles
                .Any(x => x.Name == model.Rol);

            if (rolValido)
            {

                await _userManager.AddToRoleAsync(
                    usuario,
                    model.Rol);

            }







            return RedirectToAction(nameof(Index));

        }










        // ==========================
        // CARGAR ROLES
        // ==========================

        private void CargarRoles()
        {

            // "Residente" se excluye a propósito: ese rol se asigna
            // automáticamente al aprobar una solicitud de vivienda,
            // no se crea manualmente desde este formulario.

            ViewBag.Roles =
                new List<string>
                {
                    "Seguridad",
                    "Mantenimiento"
                };

        }









        // ==========================
        // EDITAR DATOS
        // ==========================

        [HttpPost]
        public async Task<IActionResult> Editar(
            [FromBody] EditarUsuarioVM model)
        {


            var user =
                await _userManager.FindByIdAsync(model.Id);



            if (user == null)
            {

                return Json(new
                {
                    success = false,
                    message = "Usuario no encontrado"
                });

            }






            user.TC_Nombre = model.Nombre;

            user.Email = model.Email;

            user.UserName = model.Email;





            var resultado =
                await _userManager.UpdateAsync(user);




            return Json(new
            {
                success = resultado.Succeeded
            });


        }









        // ==========================
        // EDITAR ROLES
        // ==========================
        //
        // El usuario tiene un único rol operativo (Seguridad o Mantenimiento).
        // Se quitan todos los roles no-Admin actuales y se asigna el nuevo.

        [HttpPost]
        public async Task<IActionResult> EditarRoles(
            [FromBody] EditarRolesVM model)
        {


            var user =
                await _userManager.FindByIdAsync(model.Id);



            if (user == null)
            {

                return Json(new
                {
                    success = false,
                    message = "Usuario no encontrado"
                });

            }






            // Nunca permitir asignar Admin

            if (model.Rol == "Admin")
            {
                return Json(new
                {
                    success = false,
                    message = "No se puede asignar el rol Admin."
                });
            }

            if (string.IsNullOrWhiteSpace(model.Rol))
            {
                return Json(new
                {
                    success = false,
                    message = "Debe seleccionar un rol."
                });
            }







            var rolesActuales =
                await _userManager.GetRolesAsync(user);




            var quitar =
                rolesActuales
                .Where(x => x != "Admin");


            if (quitar.Any())
            {

                await _userManager.RemoveFromRolesAsync(
                    user,
                    quitar);

            }






            await _userManager.AddToRoleAsync(
                user,
                model.Rol);






            return Json(new
            {
                success = true
            });


        }









        // ==========================
        // APROBAR USUARIO
        // ==========================
        //
        // Filtra por usuario + vivienda + estado Pendiente, porque un mismo
        // usuario puede tener más de una relación (ej. es propietario en una
        // vivienda y tiene una solicitud pendiente en otra). Sin este filtro
        // se podría aprobar la relación equivocada.

        [HttpPost]
        public async Task<IActionResult> Aprobar([FromBody] AprobarSolicitudVM model)
        {

            var usuario =
                await _userManager.FindByIdAsync(model.Id);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            var viviendaUsuario =
                await _context.ViviendaUsuarios
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

            // Recalcula Disponible/Ocupada según usuarios activos.
            // Inactiva nunca se pisa acá, la controla el Admin manualmente.
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
        //
        // Nota: el estado Rechazado se aplica solo a la relación ViviendaUsuario
        // específica, no a la cuenta del usuario (usuario.TN_Estado), porque el
        // mismo usuario podría estar activo en otra vivienda como propietario.

        [HttpPost]
        public async Task<IActionResult> Rechazar([FromBody] AprobarSolicitudVM model)
        {

            var viviendaUsuario =
                await _context.ViviendaUsuarios
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

            var usuario =
                await _userManager.FindByIdAsync(model.Id);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            if (await _userManager.IsInRoleAsync(usuario, "Admin"))
            {
                return Json(new { success = false, message = "No se puede eliminar a un administrador" });
            }

            var relaciones =
                await _context.ViviendaUsuarios
                .Where(x => x.TC_IdUsuario == model.Id)
                .ToListAsync();

            // Se guardan antes del RemoveRange, para poder recalcular
            // el estado de cada vivienda afectada después de borrar.
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

            var resultado = await _userManager.DeleteAsync(usuario);

            if (!resultado.Succeeded)
            {
                return Json(new { success = false, message = "No se pudo eliminar el usuario" });
            }

            return Json(new { success = true });

        }









        // ==========================
        // RECALCULAR ESTADO DE VIVIENDA
        // ==========================
        //
        // Se llama después de cualquier cambio que active, rechace o elimine
        // una relación ViviendaUsuario. Disponible <-> Ocupada se ajustan
        // solos; Inactiva nunca se toca aquí (es manual, del Admin).

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