// /Areas/Admin/Controllers/HistorialFinancieroController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Habitia.Data;
using Habitia.ViewModels.Financiero.Admin;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HistorialFinancieroController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HistorialFinancieroController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cargos = await _context.Cargos
                .Include(c => c.Residente)
                .Include(c => c.EstadoCargo)
                .Select(c => new HistorialItemViewModel
                {
                    NombreResidente = c.Residente.UserName,
                    TipoRegistro = "Cargo",
                    Descripcion = c.TC_Descripcion,
                    Monto = c.TN_MontoTotal,
                    Fecha = c.TF_FechaEmision,
                    Estado = c.EstadoCargo.TC_Nombre
                })
                .ToListAsync();

            var recargos = await _context.Recargos
                .Include(r => r.Cargo).ThenInclude(c => c.Residente)
                .Select(r => new HistorialItemViewModel
                {
                    NombreResidente = r.Cargo.Residente.UserName,
                    TipoRegistro = "Recargo",
                    Descripcion = "Recargo sobre: " + r.Cargo.TC_Descripcion,
                    Monto = r.TN_MontoAplicado,
                    Fecha = r.TF_FechaAplicacion,
                    Estado = "Aplicado"
                })
                .ToListAsync();

            var vm = new HistorialFinancieroViewModel
            {
                Registros = cargos.Concat(recargos).OrderByDescending(r => r.Fecha).ToList()
            };

            return View(vm);
        }
    }
}