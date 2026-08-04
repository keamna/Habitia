// /Areas/Admin/Controllers/PagosController.cs
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
        private const string ESTADO_EN_REVISION = "En revisión";
        private const string ESTADO_PAGADO = "Pagado";
        private const string ESTADO_PENDIENTE = "Pendiente";

        private readonly ApplicationDbContext _context;

        public PagosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============ LISTADO: pagos en revisión ============

        public async Task<IActionResult> Index()
        {
            var pagos = await _context.Pagos
                .Include(p => p.Cargo).ThenInclude(c => c.Residente)
                .Include(p => p.Cargo).ThenInclude(c => c.TipoCargo)
                .Include(p => p.Cargo).ThenInclude(c => c.EstadoCargo)
                .Include(p => p.MetodoPago)
                .Where(p => p.TB_Estado && p.Cargo.EstadoCargo.TC_Nombre == ESTADO_EN_REVISION)
                .OrderBy(p => p.TF_FechaPago)
                .Select(p => new PagoRevisionViewModel
                {
                    TN_IdPago = p.TN_Id,
                    TN_IdCargo = p.TN_IdCargo,
                    NombreResidente = p.Cargo.Residente.TC_Nombre + " " + p.Cargo.Residente.TC_Apellido,
                    IdentificacionResidente = p.Cargo.Residente.TC_Identificacion,
                    TipoCargo = p.Cargo.TipoCargo.TC_Nombre,
                    Descripcion = p.Cargo.TC_Descripcion,
                    TN_MontoTotal = p.Cargo.TN_MontoTotal,
                    MetodoPago = p.MetodoPago.TC_Nombre,
                    TF_FechaPago = p.TF_FechaPago,
                    TC_RutaComprobante = p.TC_RutaComprobante
                })
                .ToListAsync();

            return View(pagos);
        }

        // ============ DETALLE: revisar un pago puntual ============

        public async Task<IActionResult> Detalle(int id)
        {
            var pago = await _context.Pagos
                .Include(p => p.Cargo).ThenInclude(c => c.Residente)
                .Include(p => p.Cargo).ThenInclude(c => c.TipoCargo)
                .Include(p => p.Cargo).ThenInclude(c => c.EstadoCargo)
                .Include(p => p.MetodoPago)
                .FirstOrDefaultAsync(p => p.TN_Id == id && p.TB_Estado);

            if (pago == null)
            {
                TempData["Error"] = "El pago no existe o ya fue procesado.";
                return RedirectToAction(nameof(Index));
            }

            if (pago.Cargo.EstadoCargo.TC_Nombre != ESTADO_EN_REVISION)
            {
                TempData["Error"] = $"Este pago ya no está en revisión (estado actual del cargo: {pago.Cargo.EstadoCargo.TC_Nombre}).";
                return RedirectToAction(nameof(Index));
            }

            var vm = new PagoDetalleViewModel
            {
                TN_IdPago = pago.TN_Id,
                TN_IdCargo = pago.TN_IdCargo,
                NombreResidente = pago.Cargo.Residente.TC_Nombre + " " + pago.Cargo.Residente.TC_Apellido,
                IdentificacionResidente = pago.Cargo.Residente.TC_Identificacion,
                CorreoResidente = pago.Cargo.Residente.Email,
                TelefonoResidente = pago.Cargo.Residente.TC_Telefono,
                TipoCargo = pago.Cargo.TipoCargo.TC_Nombre,
                Descripcion = pago.Cargo.TC_Descripcion,
                TN_MontoBase = pago.Cargo.TN_MontoBase,
                TB_AplicaIva = pago.Cargo.TB_AplicaIva,
                TN_MontoIva = pago.Cargo.TN_MontoIva,
                TN_MontoTotal = pago.Cargo.TN_MontoTotal,
                TF_FechaEmision = pago.Cargo.TF_FechaEmision,
                TF_FechaVencimiento = pago.Cargo.TF_FechaVencimiento,
                MetodoPago = pago.MetodoPago.TC_Nombre,
                TF_FechaPago = pago.TF_FechaPago,
                TC_RutaComprobante = pago.TC_RutaComprobante,
                EstadoCargo = pago.Cargo.EstadoCargo.TC_Nombre
            };

            return View(vm);
        }

        // ============ APROBAR ============

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aprobar(int idPago)
        {
            var pago = await _context.Pagos
                .Include(p => p.Cargo).ThenInclude(c => c.EstadoCargo)
                .FirstOrDefaultAsync(p => p.TN_Id == idPago && p.TB_Estado);

            if (pago == null)
            {
                TempData["Error"] = "El pago no existe o ya fue procesado.";
                return RedirectToAction(nameof(Index));
            }

            if (pago.Cargo.EstadoCargo.TC_Nombre != ESTADO_EN_REVISION)
            {
                TempData["Error"] = "Este pago ya no está en revisión.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var estadoPagado = await _context.EstadosCargo.FirstAsync(e => e.TC_Nombre == ESTADO_PAGADO);
                pago.Cargo.TN_IdEstadoCargo = estadoPagado.TN_Id;

                _context.HistorialPagos.Add(new THBT_H_Pago
                {
                    TN_IdPago = pago.TN_Id,
                    TB_Aprobado = true,
                    TC_MotivoRechazo = null,
                    TF_FechaCambio = DateTime.Now,
                    TC_IdUsuarioCambio = User.FindFirst("Id")?.Value
                                         ?? _context.Users.First(u => u.UserName == User.Identity!.Name).Id
                });

                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Pago aprobado. El cargo quedó marcado como Pagado.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No fue posible completar la operación: " + (ex.InnerException?.Message ?? ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        // ============ RECHAZAR ============

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rechazar(RechazarPagoViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Debe indicar el motivo del rechazo.";
                return RedirectToAction(nameof(Detalle), new { id = vm.TN_IdPago });
            }

            var pago = await _context.Pagos
                .Include(p => p.Cargo).ThenInclude(c => c.EstadoCargo)
                .FirstOrDefaultAsync(p => p.TN_Id == vm.TN_IdPago && p.TB_Estado);

            if (pago == null)
            {
                TempData["Error"] = "El pago no existe o ya fue procesado.";
                return RedirectToAction(nameof(Index));
            }

            if (pago.Cargo.EstadoCargo.TC_Nombre != ESTADO_EN_REVISION)
            {
                TempData["Error"] = "Este pago ya no está en revisión.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var estadoPendiente = await _context.EstadosCargo.FirstAsync(e => e.TC_Nombre == ESTADO_PENDIENTE);
                pago.Cargo.TN_IdEstadoCargo = estadoPendiente.TN_Id;
                pago.TC_MotivoRechazo = vm.TC_MotivoRechazo.Trim();

                var userId = _context.Users.First(u => u.UserName == User.Identity!.Name).Id;

                _context.HistorialPagos.Add(new THBT_H_Pago
                {
                    TN_IdPago = pago.TN_Id,
                    TB_Aprobado = false,
                    TC_MotivoRechazo = vm.TC_MotivoRechazo.Trim(),
                    TF_FechaCambio = DateTime.Now,
                    TC_IdUsuarioCambio = userId
                });

                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Pago rechazado. El cargo volvió a estado Pendiente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No fue posible completar la operación: " + (ex.InnerException?.Message ?? ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        // ============ HISTORIAL DE PAGOS ============

        public async Task<IActionResult> Historial()
        {
            var historial = await (
                from h in _context.HistorialPagos
                join p in _context.Pagos on h.TN_IdPago equals p.TN_Id
                join c in _context.Cargos on p.TN_IdCargo equals c.TN_Id
                join r in _context.Users on c.TC_IdResidente equals r.Id
                join tc in _context.TiposCargo on c.TN_IdTipoCargo equals tc.TN_Id
                join mp in _context.MetodosPago on p.TN_IdMetodoPago equals mp.TN_Id
                join u in _context.Users on h.TC_IdUsuarioCambio equals u.Id
                orderby h.TF_FechaCambio descending
                select new HistorialPagoViewModel
                {
                    NombreResidente = r.TC_Nombre + " " + r.TC_Apellido,
                    TipoCargo = tc.TC_Nombre,
                    TN_MontoTotal = c.TN_MontoTotal,
                    MetodoPago = mp.TC_Nombre,
                    TB_Aprobado = h.TB_Aprobado,
                    TC_MotivoRechazo = h.TC_MotivoRechazo,
                    TF_FechaCambio = h.TF_FechaCambio,
                    NombreUsuarioCambio = u.TC_Nombre + " " + u.TC_Apellido
                })
                .ToListAsync();

            return View(historial);
        }
    }
}