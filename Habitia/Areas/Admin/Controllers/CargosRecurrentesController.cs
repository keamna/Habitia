using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Habitia.Data;
using Habitia.Models.Financiero;
using Habitia.ViewModels.Financiero.Admin;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CargosRecurrentesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CargosRecurrentesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var plantillas = await _context.CargosRecurrentes
                .Include(cr => cr.TipoCargo)
                .OrderByDescending(cr => cr.TB_Estado)
                .ThenBy(cr => cr.TN_Id)
                .ToListAsync();

            return View(plantillas);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new CargoRecurrenteCreateViewModel
            {
                TiposCargo = await ObtenerTiposCargo(),
                TiposRecargo = await ObtenerTiposRecargo(),
                Residentes = await ObtenerResidentesBusqueda()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CargoRecurrenteCreateViewModel vm)
        {
            if (!vm.TB_AplicarATodos && vm.ResidentesSeleccionados.Count == 0)
                ModelState.AddModelError(nameof(vm.ResidentesSeleccionados), "Debe seleccionar al menos un residente.");

            if (vm.TB_RecargoProgramado && (vm.TN_IdTipoRecargo == null || vm.TN_ValorRecargo == null))
                ModelState.AddModelError(nameof(vm.TN_ValorRecargo), "Complete el tipo y valor del recargo programado.");

            if (!ModelState.IsValid)
            {
                vm.TiposCargo = await ObtenerTiposCargo();
                vm.TiposRecargo = await ObtenerTiposRecargo();
                vm.Residentes = await ObtenerResidentesBusqueda();
                TempData["Error"] = "Debe completar todos los campos obligatorios.";
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
                    TF_FechaInicio = vm.TF_FechaInicio,
                    TB_Estado = true
                };

                if (!vm.TB_AplicarATodos)
                {
                    plantilla.Residentes = vm.ResidentesSeleccionados
                        .Select(id => new THBT_A_CargoRecurrenteResidente { TC_IdResidente = id })
                        .ToList();
                }

                _context.CargosRecurrentes.Add(plantilla);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Cargo recurrente programado correctamente";
            }
            catch
            {
                TempData["Error"] = "No fue posible completar la operación";
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

        private async Task<List<SelectListItem>> ObtenerTiposCargo() =>
            await _context.TiposCargo.OrderBy(t => t.TC_Nombre)
                .Select(t => new SelectListItem { Value = t.TC_Nombre, Text = t.TC_Nombre }).ToListAsync();

        private async Task<List<SelectListItem>> ObtenerTiposRecargo() =>
            await _context.TiposRecargo
                .Select(t => new SelectListItem { Value = t.TN_Id.ToString(), Text = t.TC_Nombre }).ToListAsync();

        private async Task<List<ResidenteBusquedaViewModel>> ObtenerResidentesBusqueda() =>
            await _context.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id &&
                    _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Residente")))
                .Select(u => new ResidenteBusquedaViewModel
                {
                    Id = u.Id,
                    Nombre = u.TC_Nombre + " " + u.TC_Apellido,
                    Identificacion = u.TC_Identificacion
                }).ToListAsync();


    // ============ EDIT ============

public async Task<IActionResult> Edit(int id)
        {
            var plantilla = await _context.CargosRecurrentes
                .Include(cr => cr.TipoCargo)
                .Include(cr => cr.Residentes)
                .FirstOrDefaultAsync(cr => cr.TN_Id == id);

            if (plantilla == null)
            {
                TempData["Error"] = "El cargo recurrente no existe.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new CargoRecurrenteEditViewModel
            {
                TN_Id = plantilla.TN_Id,
                TC_TipoCargoTexto = plantilla.TipoCargo.TC_Nombre,
                TN_MontoBase = plantilla.TN_MontoBase,
                TB_AplicaIva = plantilla.TB_AplicaIva,
                TC_Descripcion = plantilla.TC_Descripcion,
                TC_Frecuencia = plantilla.TC_Frecuencia,
                TB_AplicarATodos = plantilla.TB_AplicarATodos,
                ResidentesSeleccionados = plantilla.Residentes.Select(r => r.TC_IdResidente).ToList(),
                TB_RecargoProgramado = plantilla.TB_RecargoProgramado,
                TN_IdTipoRecargo = plantilla.TN_IdTipoRecargo,
                TN_ValorRecargo = plantilla.TN_ValorRecargo,
                TF_FechaInicio = plantilla.TF_FechaInicio,
                TF_UltimaGeneracion = plantilla.TF_UltimaGeneracion,
                TiposCargo = await ObtenerTiposCargo(),
                TiposRecargo = await ObtenerTiposRecargo(),
                Residentes = await ObtenerResidentesBusqueda()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CargoRecurrenteEditViewModel vm)
        {
            if (!vm.TB_AplicarATodos && vm.ResidentesSeleccionados.Count == 0)
                ModelState.AddModelError(nameof(vm.ResidentesSeleccionados), "Debe seleccionar al menos un residente.");

            if (vm.TB_RecargoProgramado && (vm.TN_IdTipoRecargo == null || vm.TN_ValorRecargo == null))
                ModelState.AddModelError(nameof(vm.TN_ValorRecargo), "Complete el tipo y valor del recargo programado.");

            if (!ModelState.IsValid)
            {
                vm.TiposCargo = await ObtenerTiposCargo();
                vm.TiposRecargo = await ObtenerTiposRecargo();
                vm.Residentes = await ObtenerResidentesBusqueda();
                TempData["Error"] = "Debe completar todos los campos obligatorios.";
                return View(vm);
            }

            var plantilla = await _context.CargosRecurrentes
                .Include(cr => cr.Residentes)
                .FirstOrDefaultAsync(cr => cr.TN_Id == vm.TN_Id);

            if (plantilla == null)
            {
                TempData["Error"] = "El cargo recurrente no existe.";
                return RedirectToAction(nameof(Index));
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

                plantilla.TN_IdTipoCargo = tipoCargo.TN_Id;
                plantilla.TN_MontoBase = vm.TN_MontoBase;
                plantilla.TB_AplicaIva = vm.TB_AplicaIva;
                plantilla.TC_Descripcion = vm.TC_Descripcion;
                plantilla.TC_Frecuencia = vm.TC_Frecuencia;
                plantilla.TB_AplicarATodos = vm.TB_AplicarATodos;
                plantilla.TB_RecargoProgramado = vm.TB_RecargoProgramado;
                plantilla.TN_IdTipoRecargo = vm.TB_RecargoProgramado ? vm.TN_IdTipoRecargo : null;
                plantilla.TN_ValorRecargo = vm.TB_RecargoProgramado ? vm.TN_ValorRecargo : null;

                // Reemplazar la lista de residentes destino
                _context.CargosRecurrentesResidentes.RemoveRange(plantilla.Residentes);
                if (!vm.TB_AplicarATodos)
                {
                    plantilla.Residentes = vm.ResidentesSeleccionados
                        .Select(id => new THBT_A_CargoRecurrenteResidente { TC_IdResidente = id })
                        .ToList();
                }
                else
                {
                    plantilla.Residentes = new List<THBT_A_CargoRecurrenteResidente>();
                }

                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Cargo recurrente actualizado correctamente";
            }
            catch
            {
                TempData["Error"] = "No fue posible completar la operación";
            }

            return RedirectToAction(nameof(Index));
        }

        // ============ DELETE ============

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
                // Los cargos ya generados (THBT_A_Cargo) no dependen de esta plantilla via FK,
                // así que eliminarla no afecta el historial de cargos ya cobrados.
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
    }
}