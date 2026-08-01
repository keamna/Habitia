// /Areas/Admin/Controllers/ConfiguracionPagoController.cs
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
    public class ConfiguracionPagoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConfiguracionPagoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var config = await _context.ConfiguracionesPago
                .FirstOrDefaultAsync(c => c.TB_Estado);

            var tiposTarjeta = await _context.TiposTarjeta
                .Select(t => t.TC_Nombre)
                .ToListAsync();

            var vm = new ConfiguracionPagoViewModel
            {
                TN_Id = config?.TN_Id ?? 0,
                TB_EfectivoHabilitado = config?.TB_EfectivoHabilitado ?? false,
                TB_TarjetaHabilitado = config?.TB_TarjetaHabilitado ?? false,
                TB_SinpeHabilitado = config?.TB_SinpeHabilitado ?? false,
                TC_TitularTarjeta = config?.TC_TitularTarjeta,
                TC_IbanTarjeta = config?.TC_IbanTarjeta,
                TC_TitularSinpe = config?.TC_TitularSinpe,
                TC_NumeroSinpe = config?.TC_NumeroSinpe,
                TC_TiposTarjeta = tiposTarjeta
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guardar(ConfiguracionPagoViewModel vm)
        {
            if (vm.TB_TarjetaHabilitado &&
                (string.IsNullOrWhiteSpace(vm.TC_TitularTarjeta) || string.IsNullOrWhiteSpace(vm.TC_IbanTarjeta)))
            {
                ModelState.AddModelError(string.Empty, "Debe completar los campos obligatorios");
            }

            if (vm.TB_SinpeHabilitado &&
                (string.IsNullOrWhiteSpace(vm.TC_TitularSinpe) || string.IsNullOrWhiteSpace(vm.TC_NumeroSinpe)))
            {
                ModelState.AddModelError(string.Empty, "Debe completar los campos obligatorios");
            }

            if (!ModelState.IsValid)
            {
                vm.TC_TiposTarjeta ??= new List<string>();
                return View("Index", vm);
            }

            try
            {
                var config = await _context.ConfiguracionesPago
                    .FirstOrDefaultAsync(c => c.TB_Estado);

                if (config != null)
                {
                    _context.HistorialConfiguracionesPago.Add(new THBT_H_ConfiguracionPago
                    {
                        TN_IdConfiguracionPago = config.TN_Id,
                        TB_EfectivoHabilitado = config.TB_EfectivoHabilitado,
                        TB_TarjetaHabilitado = config.TB_TarjetaHabilitado,
                        TB_SinpeHabilitado = config.TB_SinpeHabilitado,
                        TC_TitularTarjeta = config.TC_TitularTarjeta,
                        TC_IbanTarjeta = config.TC_IbanTarjeta,
                        TC_TitularSinpe = config.TC_TitularSinpe,
                        TC_NumeroSinpe = config.TC_NumeroSinpe,
                        TF_FechaCambio = DateTime.Now,
                        TC_IdUsuarioCambio = User.Identity?.Name
                    });

                    config.TB_EfectivoHabilitado = vm.TB_EfectivoHabilitado;
                    config.TB_TarjetaHabilitado = vm.TB_TarjetaHabilitado;
                    config.TB_SinpeHabilitado = vm.TB_SinpeHabilitado;
                    config.TC_TitularTarjeta = vm.TC_TitularTarjeta;
                    config.TC_IbanTarjeta = vm.TC_IbanTarjeta;
                    config.TC_TitularSinpe = vm.TC_TitularSinpe;
                    config.TC_NumeroSinpe = vm.TC_NumeroSinpe;
                    config.TF_FechaActualizacion = DateTime.Now;
                }
                else
                {
                    _context.ConfiguracionesPago.Add(new THBT_A_ConfiguracionPago
                    {
                        TB_EfectivoHabilitado = vm.TB_EfectivoHabilitado,
                        TB_TarjetaHabilitado = vm.TB_TarjetaHabilitado,
                        TB_SinpeHabilitado = vm.TB_SinpeHabilitado,
                        TC_TitularTarjeta = vm.TC_TitularTarjeta,
                        TC_IbanTarjeta = vm.TC_IbanTarjeta,
                        TC_TitularSinpe = vm.TC_TitularSinpe,
                        TC_NumeroSinpe = vm.TC_NumeroSinpe,
                        TF_FechaActualizacion = DateTime.Now,
                        TB_Estado = true
                    });
                }

                var existentes = await _context.TiposTarjeta
                    .Select(t => t.TC_Nombre)
                    .ToListAsync();

                foreach (var nombre in vm.TC_TiposTarjeta ?? new List<string>())
                {
                    if (!string.IsNullOrWhiteSpace(nombre) && !existentes.Contains(nombre))
                    {
                        _context.TiposTarjeta.Add(new THBT_CAT_TipoTarjeta { TC_Nombre = nombre.Trim() });
                    }
                }

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