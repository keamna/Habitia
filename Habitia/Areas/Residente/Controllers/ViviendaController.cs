using Habitia.Data;
using Habitia.Enums;
using Habitia.Helpers;
using Habitia.Models;
using Habitia.ViewModels.Vivienda;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Residente.Controllers
{
    [Area("Residente")]
    [Authorize(Roles = "Residente")]
    public class ViviendaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ViviendaController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Residente/Vivienda/Index
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var relaciones = await _context.ViviendaUsuarios
                .Include(vu => vu.Vivienda)
                    .ThenInclude(v => v.Usuarios)
                        .ThenInclude(vu => vu.Usuario)
                .Where(vu =>
                    vu.TC_IdUsuario == userId &&
                    vu.TN_Estado != EstadoUsuarioEnum.Rechazado)
                .ToListAsync();

            var viviendas = new List<ViviendaListaViewModel>();

            foreach (var relacion in relaciones)
            {
                var vivienda = relacion.Vivienda;

                var propietario = vivienda.Usuarios
                    .FirstOrDefault(x =>
                        x.TN_TipoRelacion == TipoRelacionEnum.Propietario &&
                        x.TN_Estado == EstadoUsuarioEnum.Activo);

                var inquilinosActuales = vivienda.Usuarios
                    .Count(x =>
                        x.TN_TipoRelacion == TipoRelacionEnum.Inquilino &&
                        (x.TN_Estado == EstadoUsuarioEnum.Activo ||
                         x.TN_Estado == EstadoUsuarioEnum.Pendiente));

                viviendas.Add(new ViviendaListaViewModel
                {
                    Id = vivienda.TN_Id,
                    Numero = vivienda.TC_Numero,
                    Tipo = vivienda.TN_Tipo,
                    Estado = vivienda.TN_Estado,
                    CantidadInquilinos = vivienda.TN_CantidadInquilinos,
                    PropietarioNombre = propietario != null
                        ? $"{propietario.Usuario.TC_Nombre} {propietario.Usuario.TC_Apellido}"
                        : null,
                    InquilinosActuales = inquilinosActuales,
                    ViveAhi = relacion.TB_ViveAhi,
                    EsPropietario = relacion.TN_TipoRelacion == TipoRelacionEnum.Propietario &&
                                     relacion.TN_Estado == EstadoUsuarioEnum.Activo,
                    EtiquetaInquilinos = ViviendaHelper.ObtenerEtiquetaOcupantes(propietario?.TB_ViveAhi)
                });
            }

            return View(viviendas);
        }

        // GET: /Residente/Vivienda/EditarCupo/5
        [HttpGet]
        public async Task<IActionResult> EditarCupo(int id)
        {
            var userId = _userManager.GetUserId(User);

            var relacionPropietario = await _context.ViviendaUsuarios
                .FirstOrDefaultAsync(x =>
                    x.TN_IdVivienda == id &&
                    x.TC_IdUsuario == userId &&
                    x.TN_TipoRelacion == TipoRelacionEnum.Propietario &&
                    x.TN_Estado == EstadoUsuarioEnum.Activo);

            if (relacionPropietario == null)
            {
                return Forbid();
            }

            var vivienda = await _context.Viviendas
                .Include(v => v.Usuarios)
                .FirstOrDefaultAsync(v => v.TN_Id == id);

            if (vivienda == null)
            {
                return NotFound();
            }

            var inquilinosActuales = vivienda.Usuarios
                .Count(x =>
                    x.TN_TipoRelacion == TipoRelacionEnum.Inquilino &&
                    (x.TN_Estado == EstadoUsuarioEnum.Activo || x.TN_Estado == EstadoUsuarioEnum.Pendiente));

            var model = new CupoViviendaViewModel
            {
                Id = vivienda.TN_Id,
                Numero = vivienda.TC_Numero,
                InquilinosActuales = inquilinosActuales,
                CantidadInquilinos = vivienda.TN_CantidadInquilinos,
                EtiquetaInquilinos = ViviendaHelper.ObtenerEtiquetaOcupantes(relacionPropietario.TB_ViveAhi)
            };

            return View(model);
        }

        // POST: /Residente/Vivienda/EditarCupo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarCupo(CupoViviendaViewModel model)
        {
            var userId = _userManager.GetUserId(User);

            var relacionPropietario = await _context.ViviendaUsuarios
                .FirstOrDefaultAsync(x =>
                    x.TN_IdVivienda == model.Id &&
                    x.TC_IdUsuario == userId &&
                    x.TN_TipoRelacion == TipoRelacionEnum.Propietario &&
                    x.TN_Estado == EstadoUsuarioEnum.Activo);

            if (relacionPropietario == null)
            {
                return Forbid();
            }

            var vivienda = await _context.Viviendas
                .Include(v => v.Usuarios)
                .FirstOrDefaultAsync(v => v.TN_Id == model.Id);

            if (vivienda == null)
            {
                return NotFound();
            }

            var inquilinosActuales = vivienda.Usuarios
                .Count(x =>
                    x.TN_TipoRelacion == TipoRelacionEnum.Inquilino &&
                    (x.TN_Estado == EstadoUsuarioEnum.Activo || x.TN_Estado == EstadoUsuarioEnum.Pendiente));

            if (model.CantidadInquilinos < inquilinosActuales)
            {
                ModelState.AddModelError(
                    nameof(model.CantidadInquilinos),
                    $"No puede fijar un cupo menor a los {inquilinosActuales} que ya están registrados o pendientes de aprobación."
                );
            }

            if (!ModelState.IsValid)
            {
                model.Numero = vivienda.TC_Numero;
                model.InquilinosActuales = inquilinosActuales;
                model.EtiquetaInquilinos = ViviendaHelper.ObtenerEtiquetaOcupantes(relacionPropietario.TB_ViveAhi);
                return View(model);
            }

            vivienda.TN_CantidadInquilinos = model.CantidadInquilinos;
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] = "Cupo actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // ============================
        // NUEVA LÓGICA REGISTRO
        // ============================

        // GET: /Residente/Vivienda/RegistrarVivienda
        [HttpGet]
        public IActionResult RegistrarVivienda()
        {
            return View(new RegistrarViviendaViewModel());
        }

        // AJAX: /Residente/Vivienda/ObtenerViviendasSinPropietario
        [HttpGet]
        public async Task<IActionResult> ObtenerViviendasSinPropietario(TipoViviendaEnum tipo)
        {
            var viviendas = await _context.Viviendas
                .Include(v => v.Usuarios)
                .Where(v =>
                    v.TN_Tipo == tipo &&
                    v.TN_Estado != EstadoViviendaEnum.Inactiva)
                .ToListAsync();

            var resultado = viviendas
                .Where(v => !v.Usuarios.Any(x =>
                    x.TN_TipoRelacion == TipoRelacionEnum.Propietario &&
                    (x.TN_Estado == EstadoUsuarioEnum.Activo || x.TN_Estado == EstadoUsuarioEnum.Pendiente)))
                .Select(v => new { id = v.TN_Id, numero = v.TC_Numero })
                .ToList();

            return Json(resultado);
        }

        // POST: /Residente/Vivienda/RegistrarVivienda
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarVivienda(RegistrarViviendaViewModel model)
        {
            var userId = _userManager.GetUserId(User);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var vivienda = await _context.Viviendas
                .Include(v => v.Usuarios)
                .FirstOrDefaultAsync(v => v.TN_Id == model.IdVivienda);

            if (vivienda == null || vivienda.TN_Estado == EstadoViviendaEnum.Inactiva)
            {
                ModelState.AddModelError(nameof(model.IdVivienda), "La vivienda seleccionada no está disponible.");
                return View(model);
            }

            // 🔒 Protección contra doble asignación
            var yaTienePropietario = vivienda.Usuarios.Any(x =>
                x.TN_TipoRelacion == TipoRelacionEnum.Propietario &&
                (x.TN_Estado == EstadoUsuarioEnum.Activo || x.TN_Estado == EstadoUsuarioEnum.Pendiente));

            if (yaTienePropietario)
            {
                ModelState.AddModelError(nameof(model.IdVivienda), "Esta vivienda ya tiene un propietario asignado.");
                return View(model);
            }

            _context.ViviendaUsuarios.Add(new ViviendaUsuario
            {
                TN_IdVivienda = model.IdVivienda,
                TC_IdUsuario = userId,
                TN_TipoRelacion = TipoRelacionEnum.Propietario,
                TN_Estado = EstadoUsuarioEnum.Pendiente,
                TB_ViveAhi = false,
                TF_FechaRegistro = DateTime.Now
            });

            await _context.SaveChangesAsync();

            TempData["MensajeExito"] = "Solicitud enviada. Un administrador debe aprobarla.";

            return RedirectToAction(nameof(Index));
        }
    }
}