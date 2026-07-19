using Habitia.Data;
using Habitia.Enums;
using Habitia.Models;
using Habitia.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;


        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }





        // =====================================================
        // LOGIN GET
        // =====================================================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }





        // =====================================================
        // LOGIN POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }



            var usuario =
                await _userManager.FindByEmailAsync(model.Email);



            if (usuario == null)
            {

                ModelState.AddModelError(
                    "",
                    "Credenciales incorrectas."
                );

                return View(model);
            }




            if (usuario.TN_Estado == EstadoUsuarioEnum.Pendiente)
            {

                ModelState.AddModelError(
                    "",
                    "Su cuenta todavía está pendiente de aprobación."
                );

                return View(model);

            }




            if (usuario.TN_Estado == EstadoUsuarioEnum.Rechazado)
            {

                ModelState.AddModelError(
                    "",
                    "Su solicitud fue rechazada."
                );

                return View(model);

            }






            var resultado =
                await _signInManager.PasswordSignInAsync(
                    usuario,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false
                );






            if (!resultado.Succeeded)
            {

                ModelState.AddModelError(
                    "",
                    "Correo o contraseña incorrectos."
                );

                return View(model);

            }






            var roles =
                await _userManager.GetRolesAsync(usuario);





            if (roles.Contains("Admin"))
            {

                return RedirectToAction(
                    "Index",
                    "Dashboard",
                    new
                    {
                        area = "Admin"
                    });

            }







            if (roles.Contains("Residente"))
            {

                return RedirectToAction(
                    "Index",
                    "Home",
                    new
                    {
                        area = "Residente"
                    });

            }







            if (roles.Contains("Seguridad"))
            {

                return RedirectToAction(
                    "Index",
                    "Home",
                    new
                    {
                        area = "Seguridad"
                    });

            }







            if (roles.Contains("Mantenimiento"))
            {

                return RedirectToAction(
                    "Index",
                    "Home",
                    new
                    {
                        area = "Mantenimiento"
                    });

            }







            return RedirectToAction("Login");

        }









        // =====================================================
        // REGISTER GET
        // =====================================================

        [HttpGet]
        public IActionResult Register()
        {

            return View(
                new RegisterViewModel()
            );

        }








        // =====================================================
        // REGISTER POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }






            var vivienda =
                await _context.Viviendas
                .FirstOrDefaultAsync(x =>
                    x.TN_Id == model.TN_ViviendaId
                );






            if (vivienda == null)
            {

                ModelState.AddModelError(
                    "",
                    "Debe seleccionar una vivienda."
                );


                return View(model);

            }








            // ==========================================
            // VALIDAR PROPIETARIO
            // ==========================================


            if (model.TC_TipoRelacion == TipoRelacionEnum.Propietario)
            {


                bool existePropietario =
                    await _context.ViviendaUsuarios
                    .AnyAsync(x =>
                        x.TN_IdVivienda == vivienda.TN_Id &&
                        x.TN_TipoRelacion == TipoRelacionEnum.Propietario &&
                        (
                            x.TN_Estado == EstadoUsuarioEnum.Activo ||
                            x.TN_Estado == EstadoUsuarioEnum.Pendiente
                        )
                    );



                if (existePropietario)
                {

                    ModelState.AddModelError(
                        "",
                        "Esta vivienda ya tiene propietario."
                    );


                    return View(model);

                }


            }









            // ==========================================
            // VALIDAR INQUILINO
            // ==========================================


            if (model.TC_TipoRelacion == TipoRelacionEnum.Inquilino)
            {


                var propietario =
                    await _context.ViviendaUsuarios
                    .FirstOrDefaultAsync(x =>
                        x.TN_IdVivienda == vivienda.TN_Id &&
                        x.TN_TipoRelacion == TipoRelacionEnum.Propietario &&
                        x.TN_Estado == EstadoUsuarioEnum.Activo
                    );



                if (propietario == null)
                {

                    ModelState.AddModelError(
                        "",
                        "La vivienda no tiene propietario aprobado."
                    );


                    return View(model);

                }



            }

            // ==========================================
            // CREAR USUARIO
            // ==========================================


            var partesNombre =
                model.TC_NombreCompleto
                .Trim()
                .Split(" ");



            var nombre =
                partesNombre[0];



            var apellido =
                partesNombre.Length > 1
                ? string.Join(" ", partesNombre.Skip(1))
                : "";





            var usuario =
                new ApplicationUser
                {

                    UserName = model.Email,

                    Email = model.Email,

                    EmailConfirmed = true,


                    TC_Nombre = nombre,

                    TC_Apellido = apellido,


                    TN_TipoIdentificacion =
                         model.TC_TipoIdentificacion.Value,


                    TC_Identificacion =
                        model.TC_NumeroIdentificacion,


                    TC_Telefono =
                        model.TC_Telefono,


                    TN_Estado =
                        EstadoUsuarioEnum.Pendiente,


                    TF_FechaRegistro =
                        DateTime.Now

                };







            var resultado =
                await _userManager.CreateAsync(
                    usuario,
                    model.Password
                );






            if (!resultado.Succeeded)
            {


                foreach (var error in resultado.Errors)
                {

                    ModelState.AddModelError(
                        "",
                        error.Description
                    );

                }


                return View(model);

            }







            // ==========================================
            // ASIGNAR ROL RESIDENTE
            // ==========================================


            await _userManager.AddToRoleAsync(
                usuario,
                "Residente"
            );









            // ==========================================
            // RELACIONAR VIVIENDA
            // ==========================================


            var viviendaUsuario =
                new ViviendaUsuario
                {

                    TC_IdUsuario =
                        usuario.Id,


                    TN_IdVivienda =
                        vivienda.TN_Id,


                    TN_TipoRelacion =
                        model.TC_TipoRelacion.Value,


                    TN_Estado =
                        EstadoUsuarioEnum.Pendiente,


                    TB_ViveAhi =
                        model.TB_ViveAhi,


                    TF_FechaRegistro =
                        DateTime.Now

                };






            _context.ViviendaUsuarios.Add(
                viviendaUsuario
            );



            await _context.SaveChangesAsync();







            TempData["RegistroPendiente"] = true;



            return RedirectToAction(
                "Register"
            );


        }









        // =====================================================
        // OBTENER VIVIENDAS
        // =====================================================


        [HttpGet]
        public async Task<IActionResult> ObtenerViviendas(
            TipoViviendaEnum tipo)
        {


            var viviendas =
                await _context.Viviendas
                .Where(x =>
                    x.TN_Tipo == tipo &&
                    x.TN_Estado == EstadoViviendaEnum.Disponible
                )
                .Select(x => new
                {

                    id = x.TN_Id,

                    numero = x.TC_Numero

                })
                .ToListAsync();




            return Json(viviendas);


        }









        // =====================================================
        // LOGOUT
        // =====================================================


        [HttpPost]
        public async Task<IActionResult> Logout()
        {

            await _signInManager.SignOutAsync();


            return RedirectToAction(
                "Login"
            );

        }


    }

}