using Habitia.Data;
using Habitia.Enums;
using Habitia.Models;
using Habitia.ViewModels.Vivienda;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ViviendasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ViviendasController(ApplicationDbContext context)
        {
            _context = context;
        }


        // LISTADO DE VIVIENDAS

        public async Task<IActionResult> Index()
        {
            var viviendas = await _context.Viviendas
                .Include(v => v.Usuarios)
                .ThenInclude(vu => vu.Usuario)
                .ToListAsync();


            var lista = viviendas.Select(v => new ViviendaListaViewModel
            {
                Id = v.TN_Id,

                Numero = v.TC_Numero,

                Tipo = v.TN_Tipo,
                Estado = v.TN_Estado,

                CantidadInquilinos = v.TN_CantidadInquilinos,


                PropietarioNombre =
                    v.Usuarios
                    .Where(x =>
                        x.TN_TipoRelacion == TipoRelacionEnum.Propietario)
                    .Select(x =>
                        x.Usuario.TC_Nombre + " " + x.Usuario.TC_Apellido)
                    .FirstOrDefault()
                    ?? "Sin propietario",


                ViveAhi =
                    v.Usuarios
                    .Where(x =>
                        x.TN_TipoRelacion == TipoRelacionEnum.Propietario)
                    .Select(x => x.TB_ViveAhi)
                    .FirstOrDefault(),


                InquilinosActuales =
                    v.Usuarios
                    .Count(x =>
                        x.TN_TipoRelacion == TipoRelacionEnum.Inquilino &&
                        x.TN_Estado == EstadoUsuarioEnum.Activo)

            }).ToList();


            return View(lista);
        }



        // GET CREAR VIVIENDA

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }


        // POST CREAR VIVIENDA

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(ViviendaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var numeroNormalizado = model.Numero.Trim();

            var yaExiste = await _context.Viviendas
                .AnyAsync(v => v.TC_Numero.ToLower() == numeroNormalizado.ToLower());

            if (yaExiste)
            {
                ModelState.AddModelError(nameof(model.Numero), $"Ya existe una vivienda con el código '{numeroNormalizado}'.");
                return View(model);
            }

            var vivienda = new Vivienda
            {
                TC_Numero = numeroNormalizado,

                TN_Tipo = model.Tipo,

                TN_CantidadInquilinos = model.CantidadInquilinos,
                TN_Estado = EstadoViviendaEnum.Disponible,
                TF_FechaRegistro = DateTime.Now
            };

            _context.Viviendas.Add(vivienda);

            await _context.SaveChangesAsync();

            TempData["MensajeExito"] = "Vivienda creada correctamente.";

            return RedirectToAction(nameof(Index));
        }


        // OBTENER VIVIENDAS DISPONIBLES PARA REGISTRO

        [HttpGet]
        public async Task<IActionResult> Disponibles(
            TipoViviendaEnum tipo)
        {
            var viviendas = await _context.Viviendas
                .Include(v => v.Usuarios)
                .Where(v =>
                    v.TN_Tipo == tipo &&
                    v.TN_Estado == EstadoViviendaEnum.Disponible)
                .ToListAsync();


            var resultado = new List<object>();


            foreach (var vivienda in viviendas)
            {
                var propietario =
                    vivienda.Usuarios
                    .FirstOrDefault(x =>
                        x.TN_TipoRelacion == TipoRelacionEnum.Propietario &&
                        x.TN_Estado == EstadoUsuarioEnum.Activo);



                var inquilinos =
                    vivienda.Usuarios
                    .Count(x =>
                        x.TN_TipoRelacion == TipoRelacionEnum.Inquilino &&
                        x.TN_Estado == EstadoUsuarioEnum.Activo);



                bool disponible = false;



                if (propietario == null)
                {
                    disponible = true;
                }
                else if (
                    propietario.TB_ViveAhi == false &&
                    inquilinos < vivienda.TN_CantidadInquilinos)
                {
                    disponible = true;
                }



                if (disponible)
                {
                    resultado.Add(new
                    {
                        id = vivienda.TN_Id,
                        numero = vivienda.TC_Numero
                    });
                }
            }


            return Json(resultado);
        }

        // ELIMINAR VIVIENDA

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar([FromBody] IdViviendaVM model)
        {
            var vivienda = await _context.Viviendas
                .FirstOrDefaultAsync(v => v.TN_Id == model.Id);

            if (vivienda == null)
            {
                return Json(new { success = false, message = "Vivienda no encontrada." });
            }

            var relaciones = await _context.ViviendaUsuarios
                .Where(x => x.TN_IdVivienda == model.Id)
                .ToListAsync();

            var tienePendientes = relaciones
                .Any(x => x.TN_Estado == EstadoUsuarioEnum.Pendiente);

            if (tienePendientes)
            {
                return Json(new
                {
                    success = false,
                    message = "No se puede eliminar la vivienda porque tiene solicitudes pendientes de aprobación. Apruebe o rechace esas solicitudes primero."
                });
            }

            var tieneVinculoActivo = relaciones
                .Any(x => x.TN_Estado == EstadoUsuarioEnum.Activo || x.TN_Estado == EstadoUsuarioEnum.Suspendido);

            if (tieneVinculoActivo)
            {
                return Json(new
                {
                    success = false,
                    message = "No se puede eliminar la vivienda porque tiene usuarios asociados (activos o suspendidos). Elimine o reasigne esos usuarios primero."
                });
            }

            // Solo quedan relaciones Rechazadas (o ninguna): se limpian antes de
            // eliminar la vivienda, ya que igualmente referencian su Id por FK.
            if (relaciones.Any())
            {
                _context.ViviendaUsuarios.RemoveRange(relaciones);
            }

            _context.Viviendas.Remove(vivienda);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}