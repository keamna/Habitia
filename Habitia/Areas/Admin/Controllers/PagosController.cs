using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Habitia.Data;
using Habitia.Models.Financiero;
using Habitia.ViewModels.Financiero.Admin;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PagosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PagosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Revision()
        {
            var pagos = await _context.Pagos
                .Include(p => p.Cargo).ThenInclude(c => c.Residente)
                .Include(p => p.Cargo).ThenInclude(c => c.EstadoCargo)
                .Include(p => p.MetodoPago)
                .Where(p => p.Cargo.EstadoCargo.TC_Nombre == "En revisión")
                .Select(p => new ValidarPagoViewModel
                {
                    TN_IdPago = p.TN_Id,
                    TN_IdCargo = p.TN_IdCargo,
                    NombreResidente = p.Cargo.Residente.UserName,
                    TN_MontoTotal = p.Cargo.TN_MontoTotal,
                    MetodoPago = p.MetodoPago.TC_Nombre,
                    TC_RutaComprobante = p.TC_RutaComprobante,
                    TF_FechaPago = p.TF_FechaPago
                })
                .ToListAsync();

            return View(pagos);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var pago = await _context.Pagos
                .Include(p => p.Cargo).ThenInclude(c => c.Residente)
                .Include(p => p.MetodoPago)
                .FirstOrDefaultAsync(p => p.TN_Id == id);

            if (pago == null) return NotFound();

            var vm = new ValidarPagoViewModel
            {
                TN_IdPago = pago.TN_Id,
                TN_IdCargo = pago.TN_IdCargo,
                NombreResidente = pago.Cargo.Residente.UserName,
                TN_MontoTotal = pago.Cargo.TN_MontoTotal,
                MetodoPago = pago.MetodoPago.TC_Nombre,
                TC_RutaComprobante = pago.TC_RutaComprobante,
                TF_FechaPago = pago.TF_FechaPago
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Validar(ValidarPagoViewModel vm)
        {
            if (!vm.Aprobar && string.IsNullOrWhiteSpace(vm.TC_MotivoRechazo))
            {
                ModelState.AddModelError(nameof(vm.TC_MotivoRechazo), "Debe indicar el motivo del rechazo");
                return await Detalle(vm.TN_IdPago);
            }

            try
            {
                var pago = await _context.Pagos
                    .Include(p => p.Cargo).ThenInclude(c => c.EstadoCargo)
                    .FirstOrDefaultAsync(p => p.TN_Id == vm.TN_IdPago);

                if (pago == null) return NotFound();

                var estadoAnteriorId = pago.Cargo.TN_IdEstadoCargo;

                if (vm.Aprobar)
                {
                    var estadoPagado = await _context.EstadosCargo.FirstAsync(e => e.TC_Nombre == "Pagado");
                    pago.Cargo.TN_IdEstadoCargo = estadoPagado.TN_Id;
                }
                else
                {
                    var estadoPendiente = await _context.EstadosCargo.FirstAsync(e => e.TC_Nombre == "Pendiente");
                    pago.Cargo.TN_IdEstadoCargo = estadoPendiente.TN_Id;
                    pago.TC_MotivoRechazo = vm.TC_MotivoRechazo;
                }

                _context.HistorialCargos.Add(new THBT_H_Cargo
                {
                    TN_IdCargo = pago.TN_IdCargo,
                    TN_IdEstadoCargoAnterior = estadoAnteriorId,
                    TN_IdEstadoCargoNuevo = pago.Cargo.TN_IdEstadoCargo,
                    TF_FechaCambio = DateTime.Now,
                    TC_IdUsuarioCambio = User.Identity?.Name
                });

                _context.HistorialPagos.Add(new THBT_H_Pago
                {
                    TN_IdPago = pago.TN_Id,
                    TB_Aprobado = vm.Aprobar,
                    TC_MotivoRechazo = vm.Aprobar ? null : vm.TC_MotivoRechazo,
                    TF_FechaCambio = DateTime.Now,
                    TC_IdUsuarioCambio = User.Identity?.Name
                });

                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Pago procesado correctamente";
            }
            catch
            {
                TempData["Error"] = "No fue posible completar la operación";
            }

            return RedirectToAction(nameof(Revision));
        }
    }
}