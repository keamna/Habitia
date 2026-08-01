// /Areas/Admin/Controllers/CargosController.cs
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
        private readonly ApplicationDbContext _context;

        public CargosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cargos = await _context.Cargos
                .Include(c => c.Residente)
                .Include(c => c.TipoCargo)
                .Include(c => c.EstadoCargo)
                .OrderByDescending(c => c.TF_FechaEmision)
                .Select(c => new CargoListItemViewModel
                {
                    TN_Id = c.TN_Id,
                    NombreResidente = c.Residente.UserName,
                    TipoCargo = c.TipoCargo.TC_Nombre,
                    TN_MontoTotal = c.TN_MontoTotal,
                    TF_FechaEmision = c.TF_FechaEmision,
                    TF_FechaVencimiento = c.TF_FechaVencimiento,
                    EstadoCargo = c.EstadoCargo.TC_Nombre
                })
                .ToListAsync();

            return View(cargos);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new CargoCreateViewModel
            {
                TiposCargo = await ObtenerTiposCargo(),
                Residentes = await ObtenerResidentes()
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
                vm.Residentes = await ObtenerResidentes();
                return View(vm);
            }

            try
            {
                var estadoPendiente = await _context.EstadosCargo
                    .FirstAsync(e => e.TC_Nombre == "Pendiente");

                var cargo = new THBT_A_Cargo
                {
                    TC_IdResidente = vm.TC_IdResidente,
                    TN_IdTipoCargo = vm.TN_IdTipoCargo,
                    TN_IdEstadoCargo = estadoPendiente.TN_Id,
                    TC_Descripcion = vm.TC_Descripcion,
                    TN_MontoBase = vm.TN_MontoBase,
                    TN_MontoIva = vm.TN_MontoIva,
                    TN_MontoTotal = vm.TN_MontoBase + vm.TN_MontoIva,
                    TF_FechaEmision = DateTime.Now,
                    TF_FechaVencimiento = vm.TF_FechaVencimiento,
                    TB_Estado = true
                };

                _context.Cargos.Add(cargo);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Cargo creado correctamente";
            }
            catch
            {
                TempData["Error"] = "No fue posible completar la operación";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> ObtenerTiposCargo()
        {
            return await _context.TiposCargo
                .Select(t => new SelectListItem { Value = t.TN_Id.ToString(), Text = t.TC_Nombre })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> ObtenerResidentes()
        {
            return await _context.Users
                .Where(u => _context.UserRoles
                    .Any(ur => ur.UserId == u.Id &&
                        _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Residente")))
                .Select(u => new SelectListItem { Value = u.Id, Text = u.UserName })
                .ToListAsync();
        }
    }
}