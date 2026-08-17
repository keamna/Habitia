using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Habitia.Data;
using Habitia.Models.Financiero;
using Habitia.ViewModels.Financiero.Admin;
using Habitia.Services.Interfaces;
using System.Text.Json;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CargosRecurrentesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICargoRecurrenteService _cargoRecurrenteService;

        public CargosRecurrentesController(
            ApplicationDbContext context,
            ICargoRecurrenteService cargoRecurrenteService)
        {
            _context = context;
            _cargoRecurrenteService = cargoRecurrenteService;
        }

        public async Task<IActionResult> Index()
        {
            var plantillas = await _context.CargosRecurrentes
                .Include(cr => cr.TipoCargo)
                .Include(cr => cr.TipoRecargo)
                .Include(cr => cr.Residentes)
                    .ThenInclude(r => r.Vivienda)
                .Include(cr => cr.Residentes)
                    .ThenInclude(r => r.Residente)
                .OrderByDescending(cr => cr.TB_Estado)
                .ThenBy(cr => cr.TN_Id)
                .ToListAsync();

            // Solo se consulta si al menos una plantilla aplica a todos,
            // para no hacer la query innecesariamente.
            if (plantillas.Any(p => p.TB_AplicarATodos))
            {
                var destinatariosTodos = await ObtenerDestinatariosTodosResidentesAsync();
                ViewData["JsonTodosResidentes"] = JsonSerializer.Serialize(destinatariosTodos);
                ViewData["CountTodosResidentes"] = destinatariosTodos.Count;
            }
            else
            {
                ViewData["JsonTodosResidentes"] = "[]";
                ViewData["CountTodosResidentes"] = 0;
            }

            return View(plantillas);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new CargoRecurrenteCreateViewModel
            {
                TiposCargo = await ObtenerTiposCargo(),
                TiposRecargo = await ObtenerTiposRecargo(),
                ViviendasResidentes = await ObtenerViviendasResidentesActivas()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CargoRecurrenteCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.TiposCargo = await ObtenerTiposCargo();
                vm.TiposRecargo = await ObtenerTiposRecargo();
                vm.ViviendasResidentes = await ObtenerViviendasResidentesActivas();
                return View(vm);
            }

            try
            {
                var tipoCargo = await _context.TiposCargo
                    .FirstOrDefaultAsync(t => t.TC_Nombre.ToLower() == vm.TC_TipoCargoTexto.Trim().ToLower());

                if (tipoCargo == null)
                {
                    tipoCargo = new THBT_CAT_TipoCargo { TC_Nombre = vm.TC_TipoCargoTexto.Trim() };
                    _context.TiposCargo.Add(tipoCargo);
                    await _context.SaveChangesAsync();
                }

                var plantilla = new THBT_A_CargoRecurrente
                {
                    TN_IdTipoCargo = tipoCargo.TN_Id,
                    TN_MontoBase = vm.TN_MontoBase,
                    TB_AplicaIva = vm.TB_AplicaIva,
                    TC_Descripcion = vm.TC_Descripcion,
                    TC_Frecuencia = vm.TC_Frecuencia,
                    TB_AplicarATodos = vm.TB_AplicarATodos,
                    TB_RecargoProgramado = vm.TB_RecargoProgramado,
                    TN_IdTipoRecargo = vm.TB_RecargoProgramado ? vm.TN_IdTipoRecargo : null,
                    TN_ValorRecargo = vm.TB_RecargoProgramado ? vm.TN_ValorRecargo : null,
                    TC_FrecuenciaRecargo = vm.TB_RecargoProgramado ? vm.TC_FrecuenciaRecargo : null,
                    TF_FechaInicio = vm.TF_FechaInicio,
                    TB_Estado = true
                };

                if (!vm.TB_AplicarATodos)
                {
                    var vinculos = await _context.ViviendaUsuarios
                        .Where(vu => vm.ViviendaUsuarioSeleccionados.Contains(vu.TN_Id))
                        .Select(vu => new { vu.TN_Id, vu.TC_IdUsuario, vu.TN_IdVivienda })
                        .ToListAsync();

                    if (vinculos.Count != vm.ViviendaUsuarioSeleccionados.Distinct().Count())
                    {
                        ModelState.AddModelError(nameof(vm.ViviendaUsuarioSeleccionados),
                            "Una o más viviendas seleccionadas ya no están disponibles. Vuelva a seleccionar los destinatarios.");

                        vm.TiposCargo = await ObtenerTiposCargo();
                        vm.TiposRecargo = await ObtenerTiposRecargo();
                        vm.ViviendasResidentes = await ObtenerViviendasResidentesActivas();
                        return View(vm);
                    }

                    plantilla.Residentes = vinculos
                        .Select(v => new THBT_A_CargoRecurrenteResidente
                        {
                            TC_IdResidente = v.TC_IdUsuario,
                            TN_IdVivienda = v.TN_IdVivienda
                        })
                        .ToList();
                }

                _context.CargosRecurrentes.Add(plantilla);
                await _context.SaveChangesAsync();

                // Genera el primer cargo hoy mismo, sin esperar al job periódico
                // de medianoche, sin importar si la frecuencia es Diario, Semanal
                // o Mensual.
                await _cargoRecurrenteService.GenerarCargoInmediatoAsync(plantilla.TN_Id);

                TempData["Mensaje"] = "Cargo recurrente programado y primer cargo generado correctamente";
            }
            catch (Exception ex)
            {
                // TEMPORAL PARA DIAGNÓSTICO
                var mensajeDetalle = ex.InnerException?.Message ?? ex.Message;
                TempData["Error"] = $"DEBUG — {ex.GetType().Name}: {mensajeDetalle}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pausar(int id)
        {
            var plantilla = await _context.CargosRecurrentes.FindAsync(id);
            if (plantilla != null)
            {
                plantilla.TB_Estado = !plantilla.TB_Estado;
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = plantilla.TB_Estado ? "Cargo recurrente reactivado" : "Cargo recurrente pausado";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var plantilla = await _context.CargosRecurrentes
                .Include(cr => cr.Residentes)
                .FirstOrDefaultAsync(cr => cr.TN_Id == id);

            if (plantilla == null)
            {
                TempData["Error"] = "El cargo recurrente no existe.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.CargosRecurrentesResidentes.RemoveRange(plantilla.Residentes);
                _context.CargosRecurrentes.Remove(plantilla);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Cargo recurrente eliminado correctamente";
            }
            catch
            {
                TempData["Error"] = "No fue posible eliminar el cargo recurrente";
            }

            return RedirectToAction(nameof(Index));
        }

        // ============ Helpers ============

        private async Task<List<SelectListItem>> ObtenerTiposCargo() =>
            await _context.TiposCargo.OrderBy(t => t.TC_Nombre)
                .Select(t => new SelectListItem { Value = t.TC_Nombre, Text = t.TC_Nombre }).ToListAsync();

        private async Task<List<SelectListItem>> ObtenerTiposRecargo() =>
            await _context.TiposRecargo
                .Select(t => new SelectListItem { Value = t.TN_Id.ToString(), Text = t.TC_Nombre }).ToListAsync();

        private async Task<List<ViviendaResidenteViewModel>> ObtenerViviendasResidentesActivas()
        {
            return await _context.ViviendaUsuarios
                .Include(vu => vu.Vivienda)
                .Include(vu => vu.Usuario)
                .Where(vu => vu.TN_Estado == EstadoUsuarioEnum.Activo
                             && _context.UserRoles.Any(ur => ur.UserId == vu.TC_IdUsuario &&
                                 _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Residente")))
                .OrderBy(vu => vu.Vivienda.TC_Numero)
                .Select(vu => new ViviendaResidenteViewModel
                {
                    TN_IdViviendaUsuario = vu.TN_Id,
                    TN_IdVivienda = vu.TN_IdVivienda,
                    NumeroVivienda = vu.Vivienda.TC_Numero,
                    TC_IdResidente = vu.TC_IdUsuario,
                    NombreResidente = vu.Usuario.TC_Nombre + " " + vu.Usuario.TC_Apellido,
                    IdentificacionResidente = vu.Usuario.TC_Identificacion,
                    TipoRelacion = vu.TN_TipoRelacion.ToString()
                })
                .ToListAsync();
        }

        private async Task<List<object>> ObtenerDestinatariosTodosResidentesAsync()
        {
            var vinculos = await _context.ViviendaUsuarios
                .Include(vu => vu.Vivienda)
                .Include(vu => vu.Usuario)
                .Where(vu => vu.TN_Estado == EstadoUsuarioEnum.Activo
                             && _context.UserRoles.Any(ur => ur.UserId == vu.TC_IdUsuario &&
                                 _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Residente")))
                .OrderBy(vu => vu.Vivienda.TC_Numero)
                .ToListAsync();

            return vinculos
                .Select(vu => (object)new
                {
                    identificacion = vu.Usuario.TC_Identificacion,
                    nombre = vu.Usuario.TC_Nombre + " " + vu.Usuario.TC_Apellido,
                    telefono = string.IsNullOrWhiteSpace(vu.Usuario.TC_Telefono) ? "No registrado" : vu.Usuario.TC_Telefono,
                    correo = vu.Usuario.Email,
                    vivienda = vu.Vivienda.TC_Numero
                })
                .ToList();
        }
    }
}