using Habitia.Data;
using Habitia.Enums;
using Habitia.Helpers;
using Habitia.Models;
using Habitia.Models.Acceso;
using Habitia.ViewModels.Autorizacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Residente.Controllers
{
    [Area("Residente")]
    [Authorize(Roles = "Residente")]
    public class AutorizacionesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AutorizacionesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ================= Historial de autorizaciones del residente =================
        public async Task<IActionResult> Index()
        {
            await AccesoHelper.ExpirarAutorizacionesVencidasAsync(_context);

            var userId = _userManager.GetUserId(User);

            var autorizaciones = await _context.Autorizaciones
                .Include(a => a.Visitante)
                .Include(a => a.Usuario)
                .Include(a => a.Vivienda)
                .Where(a => a.TC_IdUsuario == userId)
                .OrderByDescending(a => a.TF_FechaRegistro)
                .ToListAsync();

            var vm = autorizaciones
                .Select(a => AccesoHelper.MapAutorizacionToVM(a, mostrarDatosResidente: false, puedeInvalidar: false))
                .ToList();

            if (!vm.Any())
                TempData["Info"] = "No se encontraron visitas registradas";

            return View(vm);
        }

        // ================= Formulario de nueva autorización =================
        public IActionResult Create()
        {
            var vm = new AutorizacionFormVM
            {
                FechaVisita = DateTime.Today
            };

            return View(vm);
        }

        // ================= Búsqueda de visitantes para reutilizar (AJAX) =================
        [HttpGet]
        public async Task<IActionResult> BuscarVisitantes(string termino)
        {
            var resultado = await AccesoHelper.BuscarVisitantesAsync(_context, termino);
            return Json(resultado);
        }

        // ================= Confirmar registro: genera la autorización + QR =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AutorizacionFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Debe completar todos los campos requeridos";
                return View(vm);
            }

            var userId = _userManager.GetUserId(User);

            var viviendaUsuario = await _context.ViviendaUsuarios
                .FirstOrDefaultAsync(v => v.TC_IdUsuario == userId && v.TB_ViveAhi);

            if (viviendaUsuario == null)
            {
                TempData["Error"] = "No tiene una vivienda asignada para generar autorizaciones.";
                return View(vm);
            }

            int idVisitante;

            if (vm.IdVisitanteExistente.HasValue)
            {
                var existente = await _context.Visitantes
                    .FirstOrDefaultAsync(v => v.TN_Id == vm.IdVisitanteExistente.Value && v.TB_Estado);

                if (existente == null)
                {
                    TempData["Error"] = "El visitante seleccionado ya no está disponible.";
                    return View(vm);
                }

                idVisitante = existente.TN_Id;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(vm.NombreVisitante) ||
                    string.IsNullOrWhiteSpace(vm.IdentificacionVisitante) ||
                    vm.TipoIdentificacionVisitante == null)
                {
                    TempData["Error"] = "Debe completar todos los campos requeridos";
                    return View(vm);
                }

                var visitante = new Visitante
                {
                    TC_Nombre = vm.NombreVisitante,
                    TN_TipoIdentificacion = vm.TipoIdentificacionVisitante.Value,
                    TC_Identificacion = vm.IdentificacionVisitante,
                    TC_Telefono = vm.TelefonoVisitante,
                    TB_Estado = true
                };

                _context.Visitantes.Add(visitante);
                await _context.SaveChangesAsync();

                idVisitante = visitante.TN_Id;
            }

            var codigo = await AccesoHelper.GenerarCodigoUnicoAsync(_context);

            var ahora = DateTime.Now;

            var autorizacion = new Autorizacion
            {
                TN_IdVisitante = idVisitante,
                TC_IdUsuario = userId,
                TN_IdVivienda = viviendaUsuario.TN_IdVivienda,
                TC_Codigo = codigo,
                TF_FechaVisita = vm.FechaVisita,
                TF_FechaVencimiento = ahora.AddHours(6),
                TC_Motivo = vm.Motivo,
                TF_FechaRegistro = ahora,
                TN_Estado = EstadoAutorizacionEnum.Pendiente
            };

            _context.Autorizaciones.Add(autorizacion);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Autorización generada correctamente";

            return RedirectToAction(nameof(Detalle), new { id = autorizacion.TN_Id });
        }

        // ================= Detalle: muestra el código generado =================
        public async Task<IActionResult> Detalle(int id)
        {
            var userId = _userManager.GetUserId(User);

            var autorizacion = await _context.Autorizaciones
                .Include(a => a.Visitante)
                .Include(a => a.Usuario)
                .Include(a => a.Vivienda)
                .FirstOrDefaultAsync(a => a.TN_Id == id && a.TC_IdUsuario == userId);

            if (autorizacion == null)
                return NotFound();

            var vm = AccesoHelper.MapAutorizacionToVM(autorizacion, mostrarDatosResidente: false, puedeInvalidar: false);

            return View(vm);
        }

        // ================= Mis visitantes (para el listado inicial del formulario) =================
        [HttpGet]
        public async Task<IActionResult> MisVisitantes()
        {
            var userId = _userManager.GetUserId(User);

            var resultado = await _context.Autorizaciones
                .Where(a => a.TC_IdUsuario == userId && a.Visitante.TB_Estado)
                .Select(a => new
                {
                    id = a.Visitante.TN_Id,
                    nombre = a.Visitante.TC_Nombre,
                    identificacion = a.Visitante.TC_Identificacion
                })
                .Distinct()
                .OrderBy(v => v.nombre)
                .ToListAsync();

            return Json(resultado);
        }
    }
}