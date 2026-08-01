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
    public class RecargosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecargosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var hoy = DateTime.Now.Date;

            var vencidos = await _context.Cargos
                .Include(c => c.Residente)
                .Include(c => c.EstadoCargo)
                .Where(c => c.EstadoCargo.TC_Nombre == "Pendiente" && c.TF_FechaVencimiento < hoy)
                .Select(c => new CargoListItemViewModel
                {
                    TN_Id = c.TN_Id,
                    NombreResidente = c.Residente.UserName,
                    TN_MontoTotal = c.TN_MontoTotal,
                    TF_FechaVencimiento = c.TF_FechaVencimiento,
                    EstadoCargo = c.EstadoCargo.TC_Nombre
                })
                .ToListAsync();

            return View(vencidos);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new RecargoCreateViewModel
            {
                CargosVencidos = await ObtenerCargosVencidos(),
                TiposRecargo = await ObtenerTiposRecargo()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecargoCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.CargosVencidos = await ObtenerCargosVencidos();
                vm.TiposRecargo = await ObtenerTiposRecargo();
                return View(vm);
            }

            try
            {
                var cargo = await _context.Cargos.FirstOrDefaultAsync(c => c.TN_Id == vm.TN_IdCargo);
                if (cargo == null) return NotFound();

                var tipoRecargo = await _context.TiposRecargo.FirstAsync(t => t.TN_Id == vm.TN_IdTipoRecargo);

                var montoAplicado = tipoRecargo.TC_Nombre == "Porcentaje"
                    ? Math.Round(cargo.TN_MontoTotal * (vm.TN_Valor / 100), 2)
                    : vm.TN_Valor;

                _context.Recargos.Add(new THBT_A_Recargo
                {
                    TN_IdCargo = cargo.TN_Id,
                    TN_IdTipoRecargo = vm.TN_IdTipoRecargo,
                    TN_Valor = vm.TN_Valor,
                    TN_MontoAplicado = montoAplicado,
                    TF_FechaAplicacion = DateTime.Now,
                    TB_Estado = true
                });

                cargo.TN_MontoTotal += montoAplicado;

                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Datos guardados correctamente";
            }
            catch
            {
                TempData["Error"] = "No fue posible completar la operación";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> ObtenerCargosVencidos()
        {
            var hoy = DateTime.Now.Date;
            return await _context.Cargos
                .Include(c => c.Residente)
                .Include(c => c.EstadoCargo)
                .Where(c => c.EstadoCargo.TC_Nombre == "Pendiente" && c.TF_FechaVencimiento < hoy)
                .Select(c => new SelectListItem
                {
                    Value = c.TN_Id.ToString(),
                    Text = c.Residente.UserName + " - " + c.TC_Descripcion
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> ObtenerTiposRecargo()
        {
            return await _context.TiposRecargo
                .Select(t => new SelectListItem { Value = t.TN_Id.ToString(), Text = t.TC_Nombre })
                .ToListAsync();
        }
    }
}