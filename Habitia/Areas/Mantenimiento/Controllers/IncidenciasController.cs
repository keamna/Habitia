using Habitia.Models;
using Habitia.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Habitia.Areas.Mantenimiento.Controllers
{
    [Area("Mantenimiento")]
    [Authorize(Roles = "Mantenimiento")]
    public class IncidenciasController : Controller
    {
        private readonly IIncidenciaService _incidenciaService;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public IncidenciasController(
            IIncidenciaService incidenciaService,
            UserManager<ApplicationUser> userManager)
        {
            _incidenciaService = incidenciaService;
            _userManager = userManager;
        }

        // GET: /Mantenimiento/Incidencias/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var idUsuario = _userManager.GetUserId(User);
            var incidencia = await _incidenciaService.ObtenerPorIdAsync(id);

            if (incidencia == null)
                return NotFound();

            // Solo puede ver la incidencia si la tarea de mantenimiento
            // asociada le fue asignada a él.
            if (incidencia.Mantenimiento == null ||
                incidencia.Mantenimiento.TC_IdPersonalAsignado != idUsuario)
            {
                return Forbid();
            }

            return View(incidencia);
        }
    }
}