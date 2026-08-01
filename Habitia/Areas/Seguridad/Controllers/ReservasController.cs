using Habitia.Data;
using Habitia.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Habitia.Areas.Seguridad.Controllers
{
    [Area("Seguridad")]
    [Authorize(Roles = "Seguridad")]
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ReservasController(ApplicationDbContext context)
        {
            _context = context;
        }
        // ================= Listado completo de reservas (solo consulta, sin cancelar) =================
        public async Task<IActionResult> Todas()
        {
            await ReservaHelper.FinalizarReservasVencidasAsync(_context);
            var reservas = await _context.Reservas
                .Include(r => r.Disponibilidad).ThenInclude(d => d.AreaComun).ThenInclude(a => a.Fotos)
                .Include(r => r.Usuario)
                .Include(r => r.Vivienda)
                .OrderByDescending(r => r.Disponibilidad.TF_Fecha)
                .ToListAsync();
            // mostrarDatosResidente: true -> Seguridad sí ve nombre y vivienda (5.3.3)
            // esAdmin: false -> la vista NO muestra el botón "Cancelar" ni columna de acciones (5.3.5)
            var vm = reservas.Select(r => ReservaHelper.MapToVM(r, mostrarDatosResidente: true, esAdmin: false)).ToList();
            ViewBag.EsAdmin = false;
            return View(vm);
        }
    }
}