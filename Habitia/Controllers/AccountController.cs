using Habitia.Data;
using Habitia.Enums;
using Habitia.Helpers;
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
                    "Su cuenta no ha sido aceptada. Inténtelo de nuevo."
                );

                return View(model);

            }

            if (usuario.TN_Estado == EstadoUsuarioEnum.Suspendido)
            {
                ModelState.AddModelError("", "Su cuenta ha sido suspendida. Contacte al administrador para más información.");
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

            bool esAjax =
                Request.Headers["X-Requested-With"] == "XMLHttpRequest";



            if (!ModelState.IsValid)
            {

                if (esAjax)
                {

                    var erroresModelo =
                        ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new
                    {
                        success = false,
                        errors = erroresModelo
                    });

                }

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


                if (esAjax)
                {
                    return Json(new
                    {
                        success = false,
                        errors = new[] { "Debe seleccionar una vivienda." }
                    });
                }


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


                    if (esAjax)
                    {
                        return Json(new
                        {
                            success = false,
                            errors = new[] { "Esta vivienda ya tiene propietario." }
                        });
                    }


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


                    if (esAjax)
                    {
                        return Json(new
                        {
                            success = false,
                            errors = new[] { "La vivienda no tiene propietario aprobado." }
                        });
                    }


                    return View(model);

                }



                // Respetar el cupo de inquilinos definido para la vivienda
                // (el mismo campo sirve tanto para arrendatarios como para
                // personas que conviven con el propietario).
                var inquilinosOcupados =
                    await _context.ViviendaUsuarios
                    .CountAsync(x =>
                        x.TN_IdVivienda == vivienda.TN_Id &&
                        x.TN_TipoRelacion == TipoRelacionEnum.Inquilino &&
                        (
                            x.TN_Estado == EstadoUsuarioEnum.Activo ||
                            x.TN_Estado == EstadoUsuarioEnum.Pendiente
                        )
                    );



                if (inquilinosOcupados >= vivienda.TN_CantidadInquilinos)
                {

                    ModelState.AddModelError(
                        "",
                        "Esta vivienda ya alcanzó el cupo máximo de inquilinos."
                    );


                    if (esAjax)
                    {
                        return Json(new
                        {
                            success = false,
                            errors = new[] { "Esta vivienda ya alcanzó el cupo máximo de inquilinos." }
                        });
                    }


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


                if (esAjax)
                {
                    return Json(new
                    {
                        success = false,
                        errors = resultado.Errors.Select(e => e.Description).ToList()
                    });
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







            // ==========================================
            // RESPUESTA FINAL
            // ==========================================


            if (esAjax)
            {

                return Json(new
                {
                    success = true
                });

            }



            TempData["RegistroPendiente"] = true;



            return RedirectToAction(
                "Register"
            );


        }









        // =====================================================
        // VERIFICAR EMAIL DISPONIBLE (paso 1 del registro)
        // =====================================================
        //
        // Se llama desde el JS al presionar "Siguiente" en el paso 1,
        // para avisar de inmediato si el correo ya está registrado,
        // en vez de que el error aparezca hasta el final del paso 3.

        [HttpGet]
        public async Task<IActionResult> VerificarEmailDisponible(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Json(new { disponible = false });
            }

            var usuarioExistente =
                await _userManager.FindByEmailAsync(email);

            return Json(new { disponible = usuarioExistente == null });
        }









        // =====================================================
        // OBTENER VIVIENDAS DISPONIBLES (según tipo y relación)
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> ObtenerViviendas(
            TipoViviendaEnum tipo,
            TipoRelacionEnum relacion)
        {

            var query =
                _context.Viviendas
                .Include(v => v.Usuarios)
                .Where(x => x.TN_Tipo == tipo)
                .AsQueryable();


            // Solo para Propietario nos interesa que la vivienda esté
            // marcada como Disponible (todavía sin dueño reclamándola).
            // Para Inquilino la vivienda YA tiene dueño activo (por eso
            // su TN_Estado suele cambiar a Ocupada al aprobarlo), así
            // que acá NO filtramos por TN_Estado.
            if (relacion == TipoRelacionEnum.Propietario)
            {
                query =
                    query.Where(x =>
                        x.TN_Estado == EstadoViviendaEnum.Disponible
                    );
            }


            var viviendas =
                await query.ToListAsync();


            var resultado = new List<object>();


            foreach (var vivienda in viviendas)
            {

                bool disponible = false;
                string etiquetaRelacion = null;


                if (relacion == TipoRelacionEnum.Propietario)
                {
                    // Solo se puede elegir como Propietario si nadie más
                    // la tiene reclamada (ni Activo ni Pendiente de aprobación)
                    var propietario =
                        vivienda.Usuarios
                        .FirstOrDefault(x =>
                            x.TN_TipoRelacion == TipoRelacionEnum.Propietario &&
                            (
                                x.TN_Estado == EstadoUsuarioEnum.Activo ||
                                x.TN_Estado == EstadoUsuarioEnum.Pendiente
                            ));

                    disponible = propietario == null;
                }
                else if (relacion == TipoRelacionEnum.Inquilino)
                {
                    // Para Inquilino solo se necesita un propietario Activo,
                    // sin importar si vive ahí o no (el mismo cupo
                    // TN_CantidadInquilinos sirve tanto para gente que
                    // alquila como para gente que convive con el dueño).
                    // Se cuentan también las solicitudes Pendientes para
                    // no sobrevender el cupo mientras se aprueban.
                    var propietarioActivo =
                        vivienda.Usuarios
                        .FirstOrDefault(x =>
                            x.TN_TipoRelacion == TipoRelacionEnum.Propietario &&
                            x.TN_Estado == EstadoUsuarioEnum.Activo);

                    if (propietarioActivo != null)
                    {
                        var inquilinosOcupados =
                            vivienda.Usuarios
                            .Count(x =>
                                x.TN_TipoRelacion == TipoRelacionEnum.Inquilino &&
                                (
                                    x.TN_Estado == EstadoUsuarioEnum.Activo ||
                                    x.TN_Estado == EstadoUsuarioEnum.Pendiente
                                ));

                        disponible = inquilinosOcupados < vivienda.TN_CantidadInquilinos;

                        // Aclara con qué etiqueta va a quedar registrado en
                        // esa vivienda específica, según si el propietario
                        // vive ahí o no (US: "Ocupante" en la tarjeta,
                        // pero se muestra como Familiar o Inquilino aquí).
                        if (disponible)
                        {
                            etiquetaRelacion =
                                ViviendaHelper.ObtenerEtiquetaOcupantes(propietarioActivo.TB_ViveAhi);
                        }
                    }
                }


                if (disponible)
                {
                    resultado.Add(new
                    {
                        id = vivienda.TN_Id,
                        numero = vivienda.TC_Numero,
                        etiqueta = etiquetaRelacion
                    });
                }
            }


            return Json(resultado);

        }









        // =====================================================
        // LOGOUT
        // =====================================================


        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // =====================================================
        // VERIFICAR IDENTIFICACIÓN DISPONIBLE (paso 1 del registro)
        // =====================================================
        //
        // Misma idea que VerificarEmailDisponible: se llama al presionar
        // "Siguiente" en el paso 1, para avisar de inmediato si la cédula
        // ya está registrada, sin esperar hasta el final del paso 3.

        [HttpGet]
        public async Task<IActionResult> VerificarIdentificacionDisponible(string identificacion)
        {
            if (string.IsNullOrWhiteSpace(identificacion))
            {
                return Json(new { disponible = false });
            }

            var yaExiste =
                await _userManager.Users
                .AnyAsync(u => u.TC_Identificacion == identificacion.Trim());

            return Json(new { disponible = !yaExiste });
        }


    }

}