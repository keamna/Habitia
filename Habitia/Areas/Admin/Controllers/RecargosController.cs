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
        private const string ESTADO_VENCIDO = "Vencido";
        private readonly ApplicationDbContext _context;

        public RecargosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============ MODAL: contenido del modal "Aplicar recargo", cargado vía AJAX ============
        [HttpGet]
        public async Task<IActionResult> Modal(int id)
        {
            var cargo = await ObtenerCargoVencido(id);
            if (cargo == null)
                return NotFound();

            var vm = await MapearVm(cargo);
            return PartialView("_RecargoModalContent", vm);
        }

        // ============ CREATE (página completa) ============
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new RecargoCreateViewModel
            {
                TiposRecargo = await ObtenerTiposRecargo(),
                CargosVencidos = await ObtenerCargosVencidos()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecargoCreateViewModel vm)
        {
            var cargo = await ObtenerCargoVencido(vm.TN_IdCargo);
            if (cargo == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                var vmError = await MapearVm(cargo);
                vmError.TN_IdTipoRecargo = vm.TN_IdTipoRecargo;
                vmError.TN_Valor = vm.TN_Valor;
                vmError.CargosVencidos = await ObtenerCargosVencidos();
                return PartialView("_RecargoModalContent", vmError);
            }

            try
            {
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
                cargo.TB_RecargoAplicado = true; // <-- NUEVO: evita duplicar el recargo

                await _context.SaveChangesAsync();
                TempData["Mensaje"] = $"Recargo aplicado a {cargo.Residente.TC_Nombre} {cargo.Residente.TC_Apellido} correctamente.";
            }
            catch
            {
                TempData["Error"] = "No fue posible completar la operación";
            }

            return RedirectToAction("Index", "Cargos", new { estado = ESTADO_VENCIDO });
        }

        // ============ Helpers ============

        private async Task<THBT_A_Cargo?> ObtenerCargoVencido(int id)
        {
            var cargo = await _context.Cargos
                .Include(c => c.Residente)
                .Include(c => c.TipoCargo)
                .Include(c => c.EstadoCargo)
                .FirstOrDefaultAsync(c => c.TN_Id == id && c.TB_Estado);

            // <-- NUEVO: ya no se puede aplicar recargo si ya se aplicó uno antes
            if (cargo == null || cargo.EstadoCargo.TC_Nombre != ESTADO_VENCIDO || cargo.TB_RecargoAplicado)
                return null;

            return cargo;
        }

        private async Task<RecargoCreateViewModel> MapearVm(THBT_A_Cargo cargo)
        {
            return new RecargoCreateViewModel
            {
                TN_IdCargo = cargo.TN_Id,
                NombreResidente = cargo.Residente.TC_Nombre + " " + cargo.Residente.TC_Apellido,
                IdentificacionResidente = cargo.Residente.TC_Identificacion,
                CorreoResidente = cargo.Residente.Email,
                TelefonoResidente = cargo.Residente.TC_Telefono,
                TipoCargo = cargo.TipoCargo.TC_Nombre,
                Descripcion = cargo.TC_Descripcion,
                TN_MontoBase = cargo.TN_MontoBase,
                TB_AplicaIva = cargo.TB_AplicaIva,
                TN_MontoIva = cargo.TN_MontoIva,
                TN_MontoTotal = cargo.TN_MontoTotal,
                TF_FechaVencimiento = cargo.TF_FechaVencimiento,
                DiasVencido = Math.Max(0, (DateTime.Now.Date - cargo.TF_FechaVencimiento.Date).Days),
                TiposRecargo = await ObtenerTiposRecargo()
            };
        }

        private async Task<List<SelectListItem>> ObtenerTiposRecargo()
        {
            return await _context.TiposRecargo
                .Select(t => new SelectListItem { Value = t.TN_Id.ToString(), Text = t.TC_Nombre })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> ObtenerCargosVencidos()
        {
            return await _context.Cargos
                .Include(c => c.Residente)
                .Include(c => c.TipoCargo)
                .Include(c => c.EstadoCargo)
                .Where(c => c.TB_Estado
                         && c.EstadoCargo.TC_Nombre == ESTADO_VENCIDO
                         && !c.TB_RecargoAplicado)
                .OrderBy(c => c.TF_FechaVencimiento)
                .Select(c => new SelectListItem
                {
                    Value = c.TN_Id.ToString(),
                    Text = c.Residente.TC_Nombre + " " + c.Residente.TC_Apellido
                           + " — " + c.TipoCargo.TC_Nombre
                           + " (" + c.TF_FechaVencimiento.ToString("dd/MM/yyyy") + ")"
                })
                .ToListAsync();
        }
    }
}