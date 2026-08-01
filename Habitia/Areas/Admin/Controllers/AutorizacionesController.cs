using Habitia.Data;
using Habitia.Enums;
using Habitia.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AutorizacionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AutorizacionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= Consulta de todas las autorizaciones =================
        public async Task<IActionResult> Index()
        {
            await AccesoHelper.ExpirarAutorizacionesVencidasAsync(_context);

            var autorizaciones = await _context.Autorizaciones
                .Include(a => a.Visitante)
                .Include(a => a.Usuario)
                .Include(a => a.Vivienda)
                .OrderByDescending(a => a.TF_FechaRegistro)
                .ToListAsync();

            var vm = autorizaciones
                .Select(a => AccesoHelper.MapAutorizacionToVM(a, mostrarDatosResidente: true, puedeInvalidar: true))
                .ToList();

            return View(vm);
        }

        // ================= Invalidar una autorización (solo si sigue Pendiente) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Invalidar(int id)
        {
            var autorizacion = await _context.Autorizaciones
                .FirstOrDefaultAsync(a => a.TN_Id == id);

            if (autorizacion == null)
                return NotFound();

            if (autorizacion.TN_Estado != EstadoAutorizacionEnum.Pendiente)
            {
                TempData["Error"] = "Solo se pueden invalidar autorizaciones que aún no han sido utilizadas.";
                return RedirectToAction(nameof(Index));
            }

            autorizacion.TN_Estado = EstadoAutorizacionEnum.Cancelada;
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Autorización invalidada correctamente";

            return RedirectToAction(nameof(Index));
        }
    }
}