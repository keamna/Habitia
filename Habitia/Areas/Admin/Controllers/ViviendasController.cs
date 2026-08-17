using Habitia.Data;
using Habitia.Enums;
using Habitia.Helpers;
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


            var lista = viviendas.Select(v =>
            {
                var relacionPropietario = v.Usuarios
                    .FirstOrDefault(x => x.TN_TipoRelacion == TipoRelacionEnum.Propietario);

                return new ViviendaListaViewModel
                {
                    Id = v.TN_Id,

                    Numero = v.TC_Numero,

                    Tipo = v.TN_Tipo,
                    Estado = v.TN_Estado,

                    CantidadInquilinos = v.TN_CantidadInquilinos,

                    PropietarioNombre = relacionPropietario != null
                        ? relacionPropietario.Usuario.TC_Nombre + " " + relacionPropietario.Usuario.TC_Apellido
                        : "Sin propietario",

                    ViveAhi = relacionPropietario?.TB_ViveAhi ?? false,

                    InquilinosActuales =
                        v.Usuarios
                        .Count(x =>
                            x.TN_TipoRelacion == TipoRelacionEnum.Inquilino &&
                            x.TN_Estado == EstadoUsuarioEnum.Activo),

                    // "Familiares" si el propietario vive ahí, "Inquilinos" si no.
                    EtiquetaInquilinos = ViviendaHelper.ObtenerEtiquetaOcupantes(relacionPropietario?.TB_ViveAhi)
                };
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
                TempData["MensajeError"] = "La vivienda ingresada ya se encuentra registrada.";
                return View(model);
            }

            var vivienda = new Vivienda
            {
                TC_Numero = numeroNormalizado,

                TN_Tipo = model.Tipo,

                // El cupo lo define únicamente el propietario, desde su panel,
                // una vez que quede registrado en la vivienda.
                TN_CantidadInquilinos = 0,

                // Sin usuarios asociados al crearla => Disponible.
                TN_Estado = EstadoViviendaEnum.Disponible,
                TF_FechaRegistro = DateTime.Now
            };

            _context.Viviendas.Add(vivienda);

            await _context.SaveChangesAsync();

            TempData["MensajeExito"] = "Vivienda registrada correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // GET EDITAR VIVIENDA

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var vivienda = await _context.Viviendas.FindAsync(id);

            if (vivienda == null)
            {
                TempData["MensajeError"] = "No se encontraron viviendas con los datos ingresados.";
                return RedirectToAction(nameof(Index));
            }

            var model = new ViviendaViewModel
            {
                Id = vivienda.TN_Id,
                Numero = vivienda.TC_Numero,
                Tipo = vivienda.TN_Tipo
            };

            return View(model);
        }


        // POST EDITAR VIVIENDA

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(ViviendaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var vivienda = await _context.Viviendas.FindAsync(model.Id);

            if (vivienda == null)
            {
                TempData["MensajeError"] = "No se encontraron viviendas con los datos ingresados.";
                return RedirectToAction(nameof(Index));
            }

            var numeroNormalizado = model.Numero.Trim();

            var yaExiste = await _context.Viviendas
                .AnyAsync(v => v.TC_Numero.ToLower() == numeroNormalizado.ToLower() && v.TN_Id != model.Id);

            if (yaExiste)
            {
                ModelState.AddModelError(nameof(model.Numero), "La vivienda ingresada ya se encuentra registrada.");
                return View(model);
            }

            try
            {
                vivienda.TC_Numero = numeroNormalizado;
                vivienda.TN_Tipo = model.Tipo;
                // Nota: TN_CantidadInquilinos no se toca aquí. Solo el propietario
                // (residente) puede modificarlo, desde EditarCupo.
                // Nota: TN_Estado tampoco se toca aquí. Disponible/Ocupada se
                // recalculan solos según usuarios activos (ver UsuariosController),
                // e Inactiva se maneja aparte con CambiarEstadoInactiva.

                await _context.SaveChangesAsync();

                TempData["MensajeExito"] = "Vivienda actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["MensajeError"] = "No fue posible completar la operación. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }


        // DETALLE DE VIVIENDA (consulta + usuarios asociados)

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var vivienda = await _context.Viviendas
                .Include(v => v.Usuarios)
                    .ThenInclude(vu => vu.Usuario)
                .FirstOrDefaultAsync(v => v.TN_Id == id);

            if (vivienda == null)
            {
                TempData["MensajeError"] = "No se encontraron viviendas con los datos ingresados.";
                return RedirectToAction(nameof(Index));
            }

            var relacionPropietario = vivienda.Usuarios
                .FirstOrDefault(x => x.TN_TipoRelacion == TipoRelacionEnum.Propietario);

            UsuarioDetalleViewModel MapearUsuario(ViviendaUsuario x) => new()
            {
                Id = x.TC_IdUsuario,
                Identificacion = x.Usuario.TC_Identificacion,
                Nombre = x.Usuario.TC_Nombre + " " + x.Usuario.TC_Apellido,
                Email = x.Usuario.Email,
                Telefono = x.Usuario.TC_Telefono,
                FotoPerfil = x.Usuario.TC_FotoPerfil,
                TipoRelacion = x.TN_TipoRelacion,
                Estado = x.TN_Estado,
                ViveAhi = x.TB_ViveAhi
            };

            var model = new ViviendaDetalleViewModel
            {
                Id = vivienda.TN_Id,
                Numero = vivienda.TC_Numero,
                Tipo = vivienda.TN_Tipo,
                Estado = vivienda.TN_Estado,
                CantidadInquilinos = vivienda.TN_CantidadInquilinos,
                EtiquetaInquilinos = ViviendaHelper.ObtenerEtiquetaOcupantes(relacionPropietario?.TB_ViveAhi),
                Propietario = relacionPropietario != null ? MapearUsuario(relacionPropietario) : null,
                UsuariosAsociados = vivienda.Usuarios.Select(MapearUsuario).ToList()
            };

            return View(model);
        }


        // OBTENER VIVIENDAS DISPONIBLES PARA REGISTRO
        //
        // Una vivienda es seleccionable para registrarse si:
        //  - No tiene propietario todavía (estará en Disponible), o
        //  - Tiene propietario que NO vive ahí y aún hay cupo de inquilinos
        //    libre (en ese caso la vivienda ya está en Ocupada, pero igual
        //    debe poder recibir más inquilinos hasta llenar el cupo).
        // Nunca se incluyen viviendas Inactiva (las excluye el Admin a mano).

        [HttpGet]
        public async Task<IActionResult> Disponibles(
            TipoViviendaEnum tipo)
        {
            var viviendas = await _context.Viviendas
                .Include(v => v.Usuarios)
                .Where(v =>
                    v.TN_Tipo == tipo &&
                    v.TN_Estado != EstadoViviendaEnum.Inactiva)
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

        // VERIFICAR SI SE PUEDE ELIMINAR (usado antes de mostrar el modal)

        [HttpGet]
        public async Task<IActionResult> VerificarEliminacion(int id)
        {
            var relaciones = await _context.ViviendaUsuarios
                .Where(x => x.TN_IdVivienda == id)
                .ToListAsync();

            var tienePendientes = relaciones
                .Any(x => x.TN_Estado == EstadoUsuarioEnum.Pendiente);

            if (tienePendientes)
            {
                return Json(new
                {
                    puedeEliminar = false,
                    message = "No se puede eliminar la vivienda porque tiene solicitudes pendientes de aprobación. Apruebe o rechace esas solicitudes primero."
                });
            }

            var tieneVinculoActivo = relaciones
                .Any(x => x.TN_Estado == EstadoUsuarioEnum.Activo || x.TN_Estado == EstadoUsuarioEnum.Suspendido);

            if (tieneVinculoActivo)
            {
                return Json(new
                {
                    puedeEliminar = false,
                    message = "No se puede eliminar la vivienda porque tiene usuarios asociados (activos o suspendidos). Elimine o reasigne esos usuarios primero."
                });
            }

            // La vivienda tampoco se puede eliminar si tiene reservas de áreas
            // comunes activas (todavía por realizarse) hechas a su nombre.
            var mensajeReservas = await ValidarReservasAsync(id);

            if (mensajeReservas != null)
            {
                return Json(new
                {
                    puedeEliminar = false,
                    message = mensajeReservas
                });
            }

            return Json(new { puedeEliminar = true });
        }


        // VALIDAR RESERVAS ACTIVAS O PENDIENTES DE LA VIVIENDA
        //
        // Devuelve el mensaje de error si la vivienda no se puede eliminar,
        // o null si no tiene reservas que lo impidan. Se usa tanto en
        // VerificarEliminacion (antes de mostrar el modal) como en Eliminar
        // (por si el estado cambió entre que se abrió el modal y se confirmó).

        private async Task<string?> ValidarReservasAsync(int idVivienda)
        {
            var ahora = DateTime.Now;

            var reservas = await _context.Reservas
                .Include(r => r.Disponibilidad)
                .Where(r => r.TN_IdVivienda == idVivienda &&
                            r.TN_Estado == EstadoReservaEnum.Activa)
                .ToListAsync();

            // Una reserva sigue "viva" si su horario todavía no terminó.
            var cantidadVigentes = reservas
                .Count(r => r.Disponibilidad != null &&
                            r.Disponibilidad.TF_Fecha.Date.Add(r.Disponibilidad.TF_HoraFin) > ahora);

            if (cantidadVigentes > 0)
            {
                return cantidadVigentes == 1
                    ? "No se puede eliminar la vivienda porque tiene 1 reserva de área común activa. Cancele esa reserva primero."
                    : $"No se puede eliminar la vivienda porque tiene {cantidadVigentes} reservas de áreas comunes activas. Cancele esas reservas primero.";
            }

            return null;
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

            // Se revalida acá también: entre que se abrió el modal y se confirmó,
            // pudo haberse creado una reserva nueva para esta vivienda.
            var mensajeReservas = await ValidarReservasAsync(model.Id);

            if (mensajeReservas != null)
            {
                return Json(new
                {
                    success = false,
                    message = mensajeReservas
                });
            }

            // Se guarda antes de borrar, porque una vez eliminada la vivienda
            // ya no se puede consultar su estado.
            var estabaDisponible = vivienda.TN_Estado == EstadoViviendaEnum.Disponible;

            // Reservas ya finalizadas o canceladas: no bloquean la eliminación,
            // pero referencian la vivienda por FK, así que se limpian antes.
            var reservasHistoricas = await _context.Reservas
                .Where(r => r.TN_IdVivienda == model.Id)
                .ToListAsync();

            if (reservasHistoricas.Any())
            {
                _context.Reservas.RemoveRange(reservasHistoricas);
            }

            // Solo quedan relaciones Rechazadas (o ninguna): se limpian antes de
            // eliminar la vivienda, ya que igualmente referencian su Id por FK.
            if (relaciones.Any())
            {
                _context.ViviendaUsuarios.RemoveRange(relaciones);
            }

            _context.Viviendas.Remove(vivienda);
            await _context.SaveChangesAsync();

            // El mensaje de éxito solo aparece si la vivienda estaba Disponible
            // (sin usuarios asignados) al momento de eliminarla.
            if (estabaDisponible)
            {
                TempData["MensajeExito"] = "Vivienda eliminada correctamente.";
            }

            return Json(new { success = true });
        }


        // CAMBIAR A INACTIVA / REACTIVAR (toggle manual del Admin)
        //
        // Al reactivar, se recalcula automáticamente entre Disponible/Ocupada
        // según si la vivienda tiene usuarios activos en ese momento.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstadoInactiva([FromBody] IdViviendaVM model)
        {
            var vivienda = await _context.Viviendas
                .Include(v => v.Usuarios)
                .FirstOrDefaultAsync(v => v.TN_Id == model.Id);

            if (vivienda == null)
            {
                return Json(new { success = false, message = "Vivienda no encontrada." });
            }

            if (vivienda.TN_Estado == EstadoViviendaEnum.Inactiva)
            {
                var cantidadActivos = vivienda.Usuarios
                    .Count(x => x.TN_Estado == EstadoUsuarioEnum.Activo);

                vivienda.TN_Estado = cantidadActivos > 0
                    ? EstadoViviendaEnum.Ocupada
                    : EstadoViviendaEnum.Disponible;
            }
            else
            {
                vivienda.TN_Estado = EstadoViviendaEnum.Inactiva;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, nuevoEstado = vivienda.TN_Estado.ToString() });
        }
    }
}