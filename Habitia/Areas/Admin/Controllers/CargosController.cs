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
    public class CargosController : Controller
    {
        private const decimal PORCENTAJE_IVA = 0.13m;
        private const string ESTADO_PENDIENTE = "Pendiente";
        private readonly ApplicationDbContext _context;

        public CargosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string estado)
        {
            var cargos = await _context.Cargos
                .Include(c => c.Residente)
                .Include(c => c.TipoCargo)
                .Include(c => c.EstadoCargo)
                .Where(c => c.TB_Estado) // solo cargos activos (no eliminados lógicamente)
                .OrderByDescending(c => c.TF_FechaEmision)
                .Select(c => new CargoListItemViewModel
                {
                    TN_Id = c.TN_Id,
                    ResidenteId = c.Residente.Id,
                    NombreResidente = c.Residente.TC_Nombre + " " + c.Residente.TC_Apellido,
                    IdentificacionResidente = c.Residente.TC_Identificacion,
                    CorreoResidente = c.Residente.Email,
                    TelefonoResidente = c.Residente.TC_Telefono,
                    TipoCargo = c.TipoCargo.TC_Nombre,
                    TN_MontoBase = c.TN_MontoBase,
                    TB_AplicaIva = c.TB_AplicaIva,
                    TN_MontoIva = c.TN_MontoIva,
                    TN_MontoTotal = c.TN_MontoTotal,
                    TF_FechaEmision = c.TF_FechaEmision,
                    TF_FechaVencimiento = c.TF_FechaVencimiento,
                    EstadoCargo = c.EstadoCargo.TC_Nombre,
                    TB_RecargoAplicado = c.TB_RecargoAplicado // <-- NUEVO
                })
                .ToListAsync();

            // Filtro inicial (llega, por ejemplo, al volver de aplicar un recargo)
            ViewData["EstadoInicial"] = estado;

            return View(cargos);
        }

        // ============ CREATE ============

        public async Task<IActionResult> Create()
        {
            var vm = new CargoCreateViewModel
            {
                TiposCargo = await ObtenerTiposCargo(),
                Residentes = await ObtenerResidentesBusqueda()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CargoCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.TiposCargo = await ObtenerTiposCargo();
                vm.Residentes = await ObtenerResidentesBusqueda();
                TempData["Error"] = "Debe completar todos los campos obligatorios.";
                return View(vm);
            }

            try
            {
                var tipoCargo = await ObtenerOCrearTipoCargo(vm.TC_TipoCargoTexto);

                var montoBase = vm.TN_MontoBase!.Value;
                var montoIva = vm.TB_AplicaIva ? Math.Round(montoBase * PORCENTAJE_IVA, 2) : 0m;
                var montoTotal = montoBase + montoIva;

                var estadoPendiente = await _context.EstadosCargo
                    .FirstAsync(e => e.TC_Nombre == ESTADO_PENDIENTE);

                // --- NUEVO: lista de residentes destino según el modo (único o masivo) ---
                var residentesDestino = vm.TB_AplicarATodos
                    ? vm.ResidentesSeleccionados
                    : new List<string> { vm.TC_IdResidente! };

                foreach (var idResidente in residentesDestino)
                {
                    var cargo = new THBT_A_Cargo
                    {
                        TC_IdResidente = idResidente,
                        TN_IdTipoCargo = tipoCargo.TN_Id,
                        TN_IdEstadoCargo = estadoPendiente.TN_Id,
                        TC_Descripcion = vm.TC_Descripcion,
                        TN_MontoBase = montoBase,
                        TB_AplicaIva = vm.TB_AplicaIva,
                        TN_MontoIva = montoIva,
                        TN_MontoTotal = montoTotal,
                        TF_FechaEmision = DateTime.Now,
                        TF_FechaVencimiento = vm.TF_FechaVencimiento!.Value,
                        TB_Estado = true
                    };

                    _context.Cargos.Add(cargo);
                }

                await _context.SaveChangesAsync();

                TempData["Mensaje"] = residentesDestino.Count > 1
                    ? $"Se crearon {residentesDestino.Count} cargos correctamente"
                    : "Cargo creado correctamente";
            }
            catch
            {
                TempData["Error"] = "No fue posible completar la operación";
            }

            return RedirectToAction(nameof(Index));
        }

        // ============ EDIT ============

        public async Task<IActionResult> Edit(int id)
        {
            var cargo = await _context.Cargos
                .Include(c => c.Residente)
                .Include(c => c.TipoCargo)
                .Include(c => c.EstadoCargo)
                .FirstOrDefaultAsync(c => c.TN_Id == id && c.TB_Estado);

            if (cargo == null)
            {
                TempData["Error"] = "El cargo no existe.";
                return RedirectToAction(nameof(Index));
            }

            if (cargo.EstadoCargo.TC_Nombre != ESTADO_PENDIENTE)
            {
                TempData["Error"] = $"No se puede editar un cargo en estado \"{cargo.EstadoCargo.TC_Nombre}\".";
                return RedirectToAction(nameof(Index));
            }

            var vm = new CargoEditViewModel
            {
                TN_Id = cargo.TN_Id,
                TC_IdResidente = cargo.TC_IdResidente,
                ResidenteDisplay = $"{cargo.Residente.TC_Identificacion} - {cargo.Residente.TC_Nombre} {cargo.Residente.TC_Apellido}",
                TC_TipoCargoTexto = cargo.TipoCargo.TC_Nombre,
                TN_MontoBase = cargo.TN_MontoBase,
                TB_AplicaIva = cargo.TB_AplicaIva,
                TF_FechaVencimiento = cargo.TF_FechaVencimiento,
                TC_Descripcion = cargo.TC_Descripcion,
                EstadoActual = cargo.EstadoCargo.TC_Nombre,
                TiposCargo = await ObtenerTiposCargo(),
                Residentes = await ObtenerResidentesBusqueda()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CargoEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.TiposCargo = await ObtenerTiposCargo();
                vm.Residentes = await ObtenerResidentesBusqueda();
                TempData["Error"] = "Debe completar todos los campos obligatorios.";
                return View(vm);
            }

            var cargo = await _context.Cargos
                .Include(c => c.EstadoCargo)
                .FirstOrDefaultAsync(c => c.TN_Id == vm.TN_Id && c.TB_Estado);

            if (cargo == null)
            {
                TempData["Error"] = "El cargo no existe.";
                return RedirectToAction(nameof(Index));
            }

            if (cargo.EstadoCargo.TC_Nombre != ESTADO_PENDIENTE)
            {
                TempData["Error"] = $"No se puede editar un cargo en estado \"{cargo.EstadoCargo.TC_Nombre}\".";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var tipoCargo = await ObtenerOCrearTipoCargo(vm.TC_TipoCargoTexto);

                var montoBase = vm.TN_MontoBase!.Value;
                var montoIva = vm.TB_AplicaIva ? Math.Round(montoBase * PORCENTAJE_IVA, 2) : 0m;
                var montoTotal = montoBase + montoIva;

                cargo.TC_IdResidente = vm.TC_IdResidente;
                cargo.TN_IdTipoCargo = tipoCargo.TN_Id;
                cargo.TC_Descripcion = vm.TC_Descripcion;
                cargo.TN_MontoBase = montoBase;
                cargo.TB_AplicaIva = vm.TB_AplicaIva;
                cargo.TN_MontoIva = montoIva;
                cargo.TN_MontoTotal = montoTotal;
                cargo.TF_FechaVencimiento = vm.TF_FechaVencimiento!.Value;

                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Cargo actualizado correctamente";
            }
            catch
            {
                TempData["Error"] = "No fue posible completar la operación";
            }

            return RedirectToAction(nameof(Index));
        }

        // ============ DELETE (lógico) ============
        // Solo si está en estado "Pendiente", se puede eliminar. De lo contrario, se muestra un mensaje de error.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var cargo = await _context.Cargos
                .Include(c => c.EstadoCargo)
                .FirstOrDefaultAsync(c => c.TN_Id == id && c.TB_Estado);

            if (cargo == null)
            {
                TempData["Error"] = "El cargo no existe.";
                return RedirectToAction(nameof(Index));
            }

            if (cargo.EstadoCargo.TC_Nombre != ESTADO_PENDIENTE)
            {
                TempData["Error"] = $"No se puede eliminar un cargo en estado \"{cargo.EstadoCargo.TC_Nombre}\".";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Borrado lógico: se conserva para historial/auditoría
                cargo.TB_Estado = false;
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Cargo eliminado correctamente";
            }
            catch
            {
                TempData["Error"] = "No fue posible completar la operación";
            }

            return RedirectToAction(nameof(Index));
        }

        // ============ Helpers ============

        private async Task<THBT_CAT_TipoCargo> ObtenerOCrearTipoCargo(string texto)
        {
            var tipoTexto = texto.Trim();
            var tipoCargo = await _context.TiposCargo
                .FirstOrDefaultAsync(t => t.TC_Nombre.ToLower() == tipoTexto.ToLower());

            if (tipoCargo == null)
            {
                tipoCargo = new THBT_CAT_TipoCargo { TC_Nombre = tipoTexto };
                _context.TiposCargo.Add(tipoCargo);
                await _context.SaveChangesAsync();
            }

            return tipoCargo;
        }

        private async Task<List<SelectListItem>> ObtenerTiposCargo()
        {
            return await _context.TiposCargo
                .OrderBy(t => t.TC_Nombre)
                .Select(t => new SelectListItem { Value = t.TC_Nombre, Text = t.TC_Nombre })
                .ToListAsync();
        }

        private async Task<List<ResidenteBusquedaViewModel>> ObtenerResidentesBusqueda()
        {
            return await _context.Users
                .Where(u => _context.UserRoles
                    .Any(ur => ur.UserId == u.Id &&
                        _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Residente")))
                .Select(u => new ResidenteBusquedaViewModel
                {
                    Id = u.Id,
                    Nombre = u.TC_Nombre + " " + u.TC_Apellido,
                    Identificacion = u.TC_Identificacion
                })
                .ToListAsync();
        }
    }
}