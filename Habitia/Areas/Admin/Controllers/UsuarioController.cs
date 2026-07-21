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

            if (model.Roles != null)
            {
                model.Roles =
                    model.Roles
                    .Where(x => x != "Admin" && x != "Residente")
                    .ToList();
            }

            if (model.Roles == null || !model.Roles.Any())
            {
                ModelState.AddModelError(
                    "",
                    "Debe seleccionar al menos un rol válido (Seguridad o Mantenimiento).");

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
                    model.TipoIdentificacion,


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







            if (model.Roles != null &&
               model.Roles.Any())
            {


                var rolesValidos =
                    model.Roles
                    .Where(r =>
                        _context.Roles
                        .Any(x => x.Name == r))
                    .ToList();



                if (rolesValidos.Any())
                {

                    await _userManager.AddToRolesAsync(
                        usuario,
                        rolesValidos);

                }


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

            model.Roles =
                model.Roles
                .Where(x => x != "Admin")
                .ToList();







            var rolesActuales =
                await _userManager.GetRolesAsync(user);






            var quitar =
                rolesActuales
                .Where(x =>
                    !model.Roles.Contains(x));





            if (quitar.Any())
            {

                await _userManager.RemoveFromRolesAsync(
                    user,
                    quitar);

            }






            var agregar =
                model.Roles
                .Where(x =>
                    !rolesActuales.Contains(x));





            if (agregar.Any())
            {

                await _userManager.AddToRolesAsync(
                    user,
                    agregar);

            }






            return Json(new
            {
                success = true
            });


        }









        // ==========================
        // APROBAR USUARIO
        // ==========================

        [HttpPost]
        public async Task<IActionResult> Aprobar([FromBody] IdUsuarioVM model)
        {

            var usuario =
                await _userManager.FindByIdAsync(model.Id);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            usuario.TN_Estado = EstadoUsuarioEnum.Activo;
            usuario.EmailConfirmed = true;

            await _userManager.UpdateAsync(usuario);

            var viviendaUsuario =
                await _context.ViviendaUsuarios
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

            var usuario =
                await _userManager.FindByIdAsync(model.Id);

            if (usuario == null)
            {
                return Json(new { success = false });
            }

            usuario.TN_Estado = EstadoUsuarioEnum.Rechazado;
            await _userManager.UpdateAsync(usuario);

            var viviendaUsuario =
                await _context.ViviendaUsuarios
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