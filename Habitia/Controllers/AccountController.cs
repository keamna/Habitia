using Habitia.Data;
using Habitia.Enums;
using Habitia.Helpers;
using Habitia.Models;
using Habitia.Services.Interfaces;
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
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AccountController> _logger;

        // Reglas del código de verificación (tabla CodigosVerificacion)
        private const int MINUTOS_EXPIRACION_CODIGO = 10;
        private const int MAX_INTENTOS_CODIGO = 5;

        // Array de cooldowns progresivos (en minutos)
        private static readonly int[] COOLDOWN_MINUTOS_POR_CICLO = { 1, 5, 15, 30 };


        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            IEmailSender emailSender,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailSender = emailSender;
            _logger = logger;
        }

        // =====================================================
        // HELPER: Obtener cooldown según ciclo de fallos
        // =====================================================
        private TimeSpan ObtenerCooldown(int ciclosFallidos)
        {
            int indice = Math.Min(ciclosFallidos, COOLDOWN_MINUTOS_POR_CICLO.Length - 1);
            return TimeSpan.FromMinutes(COOLDOWN_MINUTOS_POR_CICLO[indice]);
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

            // Validamos la contraseña manualmente
            var passwordValida =
                await _userManager.CheckPasswordAsync(usuario, model.Password);

            if (!passwordValida)
            {
                ModelState.AddModelError(
                    "",
                    "Correo o contraseña incorrectos."
                );

                return View(model);
            }

            var tiene2FA =
                await _userManager.GetTwoFactorEnabledAsync(usuario);

            if (tiene2FA)
            {
                try
                {
                    await GenerarYEnviarCodigoAsync(usuario);

                    TempData["EmailVerificacion"] = usuario.Email;
                    _logger.LogInformation($"Código de verificación generado para: {usuario.Email}");

                    return RedirectToAction(
                        "VerifyCode",
                        new { rememberMe = model.RememberMe }
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error al generar código: {ex.Message}");
                    ModelState.AddModelError(
                        "",
                        "Error al enviar el código de verificación. Por favor, intente más tarde."
                    );

                    return View(model);
                }
            }

            // Sin 2FA: inicia sesión directo
            await _signInManager.SignInAsync(usuario, model.RememberMe);

            return await RedirigirPorRolAsync(usuario);

        }


        // =====================================================
        // VERIFY CODE GET
        // =====================================================

        [HttpGet]
        public IActionResult VerifyCode(bool rememberMe)
        {
            if (TempData["EmailVerificacion"] == null)
            {
                return RedirectToAction("Login");
            }

            TempData.Keep("EmailVerificacion");

            var model = new VerifyCodeViewModel
            {
                RememberMe = rememberMe
            };

            ViewBag.Vencido = false;

            return View(model);
        }


        // =====================================================
        // VERIFY CODE POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {

            var email =
                TempData["EmailVerificacion"] as string;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            TempData.Keep("EmailVerificacion");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario =
                await _userManager.FindByEmailAsync(email);

            if (usuario == null)
            {
                return RedirectToAction("Login");
            }

            // Trae el último código no invalidado de este usuario
            var registro =
                await _context.CodigosVerificacion
                .Where(c =>
                    c.TC_IdUsuario == usuario.Id &&
                    !c.TB_Invalidado
                )
                .OrderByDescending(c => c.TF_FechaCreacion)
                .FirstOrDefaultAsync();

            if (registro == null || registro.TB_Usado)
            {
                ViewBag.Mensaje = "No hay un código activo. Solicitá uno nuevo.";
                ViewBag.Vencido = true;
                return View(model);
            }

            if (DateTime.UtcNow > registro.TF_FechaExpiracion)
            {
                ViewBag.Mensaje = "El código venció.";
                ViewBag.Vencido = true;
                return View(model);
            }

            if (registro.TN_Intentos >= MAX_INTENTOS_CODIGO)
            {
                var cooldown = ObtenerCooldown(registro.TN_CiclosFallidos);
                var tiempoRestante = registro.TF_UltimoReenvioUtc.HasValue
                    ? registro.TF_UltimoReenvioUtc.Value.Add(cooldown) - DateTime.UtcNow
                    : TimeSpan.Zero;

                if (tiempoRestante > TimeSpan.Zero)
                {
                    var minutosEspera = (int)Math.Ceiling(tiempoRestante.TotalSeconds / 60);
                    ViewBag.Mensaje = $"Superaste el límite de intentos. Debés esperar {minutosEspera} minuto(s) para solicitar un nuevo código.";
                    ViewBag.Vencido = true;
                    return View(model);
                }
                else
                {
                    ViewBag.Mensaje = "Superaste el límite de intentos. Solicitá un código nuevo.";
                    ViewBag.Vencido = true;
                    return View(model);
                }
            }

            if (registro.TC_Codigo != model.Codigo)
            {

                registro.TN_Intentos++;
                registro.TF_UltimoReenvioUtc = DateTime.UtcNow;

                if (registro.TN_Intentos >= MAX_INTENTOS_CODIGO)
                {
                    registro.TN_CiclosFallidos++;
                }

                await _context.SaveChangesAsync();

                int restantes =
                    MAX_INTENTOS_CODIGO - registro.TN_Intentos;

                ViewBag.Mensaje =
                    restantes > 0
                    ? $"Código incorrecto. Te quedan {restantes} intento(s)."
                    : "Código incorrecto. Superaste el límite de intentos, solicitá uno nuevo.";

                ViewBag.CodigoIncorrecto = true;
                ViewBag.Vencido = restantes <= 0;

                return View(model);
            }

            // Código correcto
            registro.TB_Usado = true;
            registro.TN_CiclosFallidos = 0;
            registro.TN_Intentos = 0;

            await _context.SaveChangesAsync();

            await _signInManager.SignInAsync(
                usuario,
                model.RememberMe
            );

            ViewBag.CodigoCorrecto = true;

            return await RedirigirPorRolAsync(usuario);

        }


        // =====================================================
        // REENVIAR CÓDIGO CON COOLDOWN PROGRESIVO
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReenviarCodigo(bool rememberMe)
        {

            var email =
                TempData["EmailVerificacion"] as string;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            var usuario =
                await _userManager.FindByEmailAsync(email);

            if (usuario == null)
            {
                return RedirectToAction("Login");
            }

            // Traer el código actual para checkear cooldown
            var codigoActual =
                await _context.CodigosVerificacion
                .Where(c =>
                    c.TC_IdUsuario == usuario.Id &&
                    !c.TB_Invalidado
                )
                .OrderByDescending(c => c.TF_FechaCreacion)
                .FirstOrDefaultAsync();

            // Si hay un código activo Y el usuario agotó intentos, checkear cooldown
            if (codigoActual != null && codigoActual.TN_Intentos >= MAX_INTENTOS_CODIGO)
            {
                var cooldown = ObtenerCooldown(codigoActual.TN_CiclosFallidos);
                var tiempoRestante = codigoActual.TF_UltimoReenvioUtc.HasValue
                    ? codigoActual.TF_UltimoReenvioUtc.Value.Add(cooldown) - DateTime.UtcNow
                    : TimeSpan.Zero;

                if (tiempoRestante > TimeSpan.Zero)
                {
                    var minutosEspera = (int)Math.Ceiling(tiempoRestante.TotalSeconds / 60);
                    TempData["Mensaje"] = $"Debés esperar {minutosEspera} minuto(s) para solicitar un nuevo código.";
                    TempData.Keep("EmailVerificacion");
                    return RedirectToAction("VerifyCode", new { rememberMe });
                }
            }

            // El cooldown expiró o no hay restricción
            try
            {
                await GenerarYEnviarCodigoAsync(usuario);

                TempData["EmailVerificacion"] = usuario.Email;
                TempData["MensajeReenvio"] = "Te enviamos un nuevo código a tu correo.";

                _logger.LogInformation($"Código reenviado para: {usuario.Email}");

                return RedirectToAction(
                    "VerifyCode",
                    new { rememberMe }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al reenviar código: {ex.Message}");
                TempData["Mensaje"] = "Error al enviar el código. Por favor, intente más tarde.";
                TempData.Keep("EmailVerificacion");
                return RedirectToAction("VerifyCode", new { rememberMe });
            }

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
            // ACTIVAR 2FA POR CORREO
            // ==========================================

            await _userManager.SetTwoFactorEnabledAsync(
                usuario,
                true
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
        // VERIFICAR EMAIL DISPONIBLE
        // =====================================================

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
        // OBTENER VIVIENDAS DISPONIBLES
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // =====================================================
        // VERIFICAR IDENTIFICACIÓN DISPONIBLE
        // =====================================================

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



        // =====================================================
        // HELPERS PRIVADOS - 2FA
        // =====================================================

        private async Task GenerarYEnviarCodigoAsync(ApplicationUser usuario)
        {
            try
            {
                _logger.LogInformation($"[INICIO] GenerarYEnviarCodigoAsync para: {usuario.Email}");

                // Invalidar códigos previos
                var codigosPrevios =
                    _context.CodigosVerificacion
                    .Where(c =>
                        c.TC_IdUsuario == usuario.Id &&
                        !c.TB_Usado &&
                        !c.TB_Invalidado
                    );

                await codigosPrevios.ForEachAsync(c => c.TB_Invalidado = true);

                // Generar nuevo código
                var codigoGenerado = Random.Shared.Next(0, 1000000).ToString("D6");

                var nuevoCodigo =
                    new CodigoVerificacion
                    {
                        TC_IdUsuario = usuario.Id,
                        TC_Codigo = codigoGenerado,
                        TF_FechaCreacion = DateTime.UtcNow,
                        TF_FechaExpiracion = DateTime.UtcNow.AddMinutes(MINUTOS_EXPIRACION_CODIGO),
                        TN_Intentos = 0,
                        TN_CiclosFallidos = 0,
                        TF_UltimoReenvioUtc = null,
                        TB_Usado = false,
                        TB_Invalidado = false
                    };

                _context.CodigosVerificacion.Add(nuevoCodigo);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"[DB] Código generado en BD - Usuario: {usuario.Email}, Código: {codigoGenerado}");

                // Generar plantilla de email
                _logger.LogInformation($"[EMAIL] Generando plantilla HTML...");
                var cuerpoCorreo =
                    EmailTemplateHelper.GenerarCorreoCodigoVerificacion(
                        usuario.TC_Nombre,
                        codigoGenerado
                    );

                _logger.LogInformation($"[EMAIL] Plantilla generada. Contenido vacío: {string.IsNullOrEmpty(cuerpoCorreo)}");

                if (string.IsNullOrEmpty(cuerpoCorreo))
                {
                    throw new InvalidOperationException("La plantilla de email retornó vacío");
                }

                // Enviar email
                _logger.LogInformation($"[EMAIL] Enviando email a: {usuario.Email}");
                await _emailSender.SendEmailAsync(
                    usuario.Email,
                    "Tu código de verificación - Habitia",
                    cuerpoCorreo
                );

                _logger.LogInformation($"✅ [EMAIL] Código enviado a {usuario.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ [ERROR] GenerarYEnviarCodigoAsync - {ex.Message} - {ex.StackTrace}");
                throw;
            }
        }



        private async Task<IActionResult> RedirigirPorRolAsync(ApplicationUser usuario)
        {

            var roles =
                await _userManager.GetRolesAsync(usuario);

            if (roles.Contains("Admin"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            if (roles.Contains("Residente"))
            {
                return RedirectToAction("Index", "Home", new { area = "Residente" });
            }

            if (roles.Contains("Seguridad"))
            {
                return RedirectToAction("Index", "Home", new { area = "Seguridad" });
            }

            if (roles.Contains("Mantenimiento"))
            {
                return RedirectToAction("Index", "Home", new { area = "Mantenimiento" });
            }

            return RedirectToAction("Login");

        }
    }
}