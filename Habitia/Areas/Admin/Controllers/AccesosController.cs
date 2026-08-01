using Habitia.Data;
using Habitia.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AccesosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccesosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= Consulta de accesos (solo lectura, reutiliza la vista de Seguridad) =================
        public async Task<IActionResult> Index()
        {
            await AccesoHelper.ExpirarAutorizacionesVencidasAsync(_context);

            var accesos = await _context.Accesos
                .Include(a => a.Visitante)
                .Include(a => a.Usuario)
                .Include(a => a.Vivienda)
                .Include(a => a.Vehiculo)
                .OrderByDescending(a => a.TF_FechaIngreso)
                .ToListAsync();

            var vm = accesos.Select(AccesoHelper.MapAccesoToVM).ToList();

            ViewBag.EsAdmin = true;
            return View("~/Areas/Seguridad/Views/Accesos/Index.cshtml", vm);
        }
    }
}