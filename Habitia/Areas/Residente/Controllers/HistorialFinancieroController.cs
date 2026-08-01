// /Areas/Residente/Controllers/HistorialFinancieroController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Habitia.Data;
using Habitia.Models;
using Habitia.ViewModels.Financiero.Residente;

namespace Habitia.Areas.Residente.Controllers
{
    [Area("Residente")]
    [Authorize(Roles = "Residente")]
    public class HistorialFinancieroController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HistorialFinancieroController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var cargos = await _context.Cargos
                .Include(c => c.EstadoCargo)
                .Where(c => c.TC_IdResidente == userId)
                .Select(c => new HistorialItemViewModel
                {
                    TipoRegistro = "Cargo",
                    Descripcion = c.TC_Descripcion,
                    Monto = c.TN_MontoTotal,
                    Fecha = c.TF_FechaEmision,
                    Estado = c.EstadoCargo.TC_Nombre
                })
                .ToListAsync();

            var recargos = await _context.Recargos
                .Include(r => r.Cargo)
                .Where(r => r.Cargo.TC_IdResidente == userId)
                .Select(r => new HistorialItemViewModel
                {
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