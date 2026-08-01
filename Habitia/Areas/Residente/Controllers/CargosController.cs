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
                .Where(c => c.TC_IdResidente == userId && c.EstadoCargo.TC_Nombre == "Pendiente")
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
                    EstadoCargo = c.EstadoCargo.TC_Nombre
                })
                .ToListAsync();

            return View(cargos);
        }

        public async Task<IActionResult> Pagar(int id)
        {
            var userId = _userManager.GetUserId(User);

            var cargo = await _context.Cargos
                .FirstOrDefaultAsync(c => c.TN_Id == id && c.TC_IdResidente == userId);

            if (cargo == null) return NotFound();

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
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Debe completar los campos obligatorios";
                return RedirectToAction(nameof(Pagar), new { id = vm.TN_IdCargo });
            }

            var extension = Path.GetExtension(vm.Comprobante.FileName).ToLowerInvariant();
            if (!FormatosPermitidos.Contains(extension))
            {
                TempData["Error"] = "Formato no permitido";
                return RedirectToAction(nameof(Pagar), new { id = vm.TN_IdCargo });
            }

            try
            {
                var userId = _userManager.GetUserId(User);
                var cargo = await _context.Cargos
                    .FirstOrDefaultAsync(c => c.TN_Id == vm.TN_IdCargo && c.TC_IdResidente == userId);

                if (cargo == null) return NotFound();

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
                    TN_IdMetodoPago = vm.TN_IdMetodoPago,
                    TC_RutaComprobante = $"/uploads/comprobantes/{nombreArchivo}",
                    TF_FechaPago = DateTime.Now,
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
                    TC_IdUsuarioCambio = User.Identity?.Name
                });

                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Datos guardados correctamente";
            }
            catch
            {
                TempData["Error"] = "No fue posible completar la operación";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}