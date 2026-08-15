// /Areas/Residente/Controllers/CargosController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Habitia.Data;
using Habitia.Models;
using Habitia.Models.Financiero;
using Habitia.ViewModels.Financiero.Residente;

namespace Habitia.Areas.Residente.Controllers
{
    [Area("Residente")]
    [Authorize(Roles = "Residente")]
    public class CargosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] FormatosPermitidos = { ".jpg", ".jpeg", ".png", ".pdf" };
        private const long TAMANO_MAXIMO_BYTES = 5 * 1024 * 1024; // 5 MB

        // Estados que el residente puede ver y sobre los que puede pagar
        private static readonly string[] EstadosVisiblesResidente = { "Pendiente", "Vencido" };

        public CargosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var cargos = await _context.Cargos
                .Include(c => c.TipoCargo)
                .Include(c => c.EstadoCargo)
                .Include(c => c.TipoRecargo) // <-- NUEVO: para mostrar "Fijo"/"Porcentaje" en el aviso
                .Where(c => c.TC_IdResidente == userId
                            && c.TB_Estado
                            && EstadosVisiblesResidente.Contains(c.EstadoCargo.TC_Nombre))
                .OrderBy(c => c.TF_FechaVencimiento)
                .Select(c => new CargoPendienteViewModel
                {
                    TN_Id = c.TN_Id,
                    TipoCargo = c.TipoCargo.TC_Nombre,
                    TC_Descripcion = c.TC_Descripcion,
                    TN_MontoBase = c.TN_MontoBase,
                    TN_MontoIva = c.TN_MontoIva,
                    TN_MontoTotal = c.TN_MontoTotal,
                    TF_FechaEmision = c.TF_FechaEmision,
                    TF_FechaVencimiento = c.TF_FechaVencimiento,
                    EstadoCargo = c.EstadoCargo.TC_Nombre,

                    // <-- NUEVO: solo tiene sentido mostrarlo mientras el cargo sigue "Pendiente"
                    // (si ya está "Vencido", TN_MontoRecargo ya refleja lo aplicado, así que no se duplica el aviso)
                    TB_RecargoProgramado = c.TB_RecargoProgramado && c.EstadoCargo.TC_Nombre == "Pendiente",
                    TC_FrecuenciaRecargo = c.TC_FrecuenciaRecargo,
                    TN_ValorRecargo = c.TN_ValorRecargo,
                    TipoRecargoNombre = c.TipoRecargo != null ? c.TipoRecargo.TC_Nombre : null
                })
                .ToListAsync();

            return View(cargos);
        }

        public async Task<IActionResult> Pagar(int id)
        {
            var userId = _userManager.GetUserId(User);

            var cargo = await _context.Cargos
                .Include(c => c.EstadoCargo)
                .FirstOrDefaultAsync(c => c.TN_Id == id && c.TC_IdResidente == userId && c.TB_Estado);

            if (cargo == null) return NotFound();

            // Solo se puede pagar si está Pendiente o Vencido
            if (!EstadosVisiblesResidente.Contains(cargo.EstadoCargo.TC_Nombre))
            {
                TempData["Error"] = $"Este cargo ya no admite pago (estado actual: {cargo.EstadoCargo.TC_Nombre}).";
                return RedirectToAction(nameof(Index));
            }

            var config = await _context.ConfiguracionesPago.FirstOrDefaultAsync(c => c.TB_Estado);

            var metodos = new List<SelectListItem>();
            var todosMetodos = await _context.MetodosPago.ToListAsync();

            foreach (var m in todosMetodos)
            {
                var habilitado = m.TC_Nombre switch
                {
                    "Efectivo" => config?.TB_EfectivoHabilitado ?? false,
                    "Tarjeta" => config?.TB_TarjetaHabilitado ?? false,
                    "SINPE" => config?.TB_SinpeHabilitado ?? false,
                    _ => false
                };

                if (habilitado)
                {
                    metodos.Add(new SelectListItem { Value = m.TN_Id.ToString(), Text = m.TC_Nombre });
                }
            }

            if (!metodos.Any())
            {
                TempData["Error"] = "No hay métodos de pago habilitados actualmente. Contacte al administrador.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new RegistrarPagoViewModel
            {
                TN_IdCargo = cargo.TN_Id,
                TC_Descripcion = cargo.TC_Descripcion,
                TN_MontoBase = cargo.TN_MontoBase,
                TN_MontoIva = cargo.TN_MontoIva,
                TN_MontoTotal = cargo.TN_MontoTotal,
                MetodosPago = metodos,
                TitularTarjeta = config?.TC_TitularTarjeta,
                IbanTarjeta = config?.TC_IbanTarjeta,
                TitularSinpe = config?.TC_TitularSinpe,
                NumeroSinpe = config?.TC_NumeroSinpe
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pagar(RegistrarPagoViewModel vm)
        {
            // Validaciones básicas de negocio antes de tocar el archivo
            if (!ModelState.IsValid)
            {
                var primerError = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault();

                TempData["Error"] = primerError ?? "Debe completar los campos obligatorios";
                return RedirectToAction(nameof(Pagar), new { id = vm.TN_IdCargo });
            }

            if (vm.TN_IdMetodoPago == null || vm.TN_IdMetodoPago <= 0)
            {
                TempData["Error"] = "Debe seleccionar un método de pago";
                return RedirectToAction(nameof(Pagar), new { id = vm.TN_IdCargo });
            }

            if (vm.Comprobante == null || vm.Comprobante.Length == 0)
            {
                TempData["Error"] = "Debe adjuntar el comprobante de pago";
                return RedirectToAction(nameof(Pagar), new { id = vm.TN_IdCargo });
            }

            if (vm.Comprobante.Length > TAMANO_MAXIMO_BYTES)
            {
                TempData["Error"] = "El comprobante no puede superar los 5 MB";
                return RedirectToAction(nameof(Pagar), new { id = vm.TN_IdCargo });
            }

            var extension = Path.GetExtension(vm.Comprobante.FileName).ToLowerInvariant();
            if (!FormatosPermitidos.Contains(extension))
            {
                TempData["Error"] = "Formato no permitido. Use JPG, PNG o PDF.";
                return RedirectToAction(nameof(Pagar), new { id = vm.TN_IdCargo });
            }

            try
            {
                var userId = _userManager.GetUserId(User);
                var cargo = await _context.Cargos
                    .Include(c => c.EstadoCargo)
                    .FirstOrDefaultAsync(c => c.TN_Id == vm.TN_IdCargo && c.TC_IdResidente == userId && c.TB_Estado);

                if (cargo == null) return NotFound();

                if (!EstadosVisiblesResidente.Contains(cargo.EstadoCargo.TC_Nombre))
                {
                    TempData["Error"] = $"Este cargo ya no admite pago (estado actual: {cargo.EstadoCargo.TC_Nombre}).";
                    return RedirectToAction(nameof(Index));
                }

                // Si hay un intento de pago anterior activo (ej. fue rechazado), se marca como histórico
                var pagoAnterior = await _context.Pagos
                    .FirstOrDefaultAsync(p => p.TN_IdCargo == cargo.TN_Id && p.TB_Estado);

                if (pagoAnterior != null)
                {
                    pagoAnterior.TB_Estado = false;
                }

                var carpeta = Path.Combine(_env.WebRootPath, "uploads", "comprobantes");
                Directory.CreateDirectory(carpeta);
                var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                var rutaFisica = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(rutaFisica, FileMode.Create))
                {
                    await vm.Comprobante.CopyToAsync(stream);
                }

                _context.Pagos.Add(new THBT_A_Pago
                {
                    TN_IdCargo = cargo.TN_Id,
                    TN_IdMetodoPago = vm.TN_IdMetodoPago!.Value,
                    TC_RutaComprobante = $"/uploads/comprobantes/{nombreArchivo}",
                    TF_FechaPago = DateTime.Now,
                    TC_MotivoRechazo = null,
                    TB_Estado = true
                });

                var estadoAnteriorId = cargo.TN_IdEstadoCargo;
                var estadoRevision = await _context.EstadosCargo.FirstAsync(e => e.TC_Nombre == "En revisión");
                cargo.TN_IdEstadoCargo = estadoRevision.TN_Id;

                _context.HistorialCargos.Add(new THBT_H_Cargo
                {
                    TN_IdCargo = cargo.TN_Id,
                    TN_IdEstadoCargoAnterior = estadoAnteriorId,
                    TN_IdEstadoCargoNuevo = estadoRevision.TN_Id,
                    TF_FechaCambio = DateTime.Now,
                    TC_IdUsuarioCambio = userId
                });

                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Comprobante enviado. Su pago quedará en revisión.";
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException?.Message ?? ex.Message;
                TempData["Error"] = "No fue posible completar la operación: " + mensaje;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}