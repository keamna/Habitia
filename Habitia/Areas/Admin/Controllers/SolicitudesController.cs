using Habitia.Data;
using Habitia.Enums;
using Habitia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Habitia.Areas.Admin.Controllers
{


    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SolicitudesController : Controller
    {


        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;



        public SolicitudesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {

            _context = context;

            _userManager = userManager;

        }




        public async Task<IActionResult> Index()
        {

            var solicitudes =
                await _context.ViviendaUsuarios
                .Include(x => x.Usuario)
                .Include(x => x.Vivienda)
                .Where(x =>
                    x.Estado == EstadoUsuarioEnum.Pendiente)
                .ToListAsync();



            return View(solicitudes);

        }







        [HttpPost]
        public async Task<IActionResult> Aprobar(int id)
        {


            var solicitud =
                await _context.ViviendaUsuarios
                .Include(x => x.Usuario)
                .Include(x => x.Vivienda)
                .FirstOrDefaultAsync(x => x.Id == id);



            if (solicitud == null)
            {
                return NotFound();
            }




            solicitud.Estado =
                EstadoUsuarioEnum.Activo;




            solicitud.Usuario.Estado =
                EstadoUsuarioEnum.Activo;





            // Si es propietario viviendo ahí
            if (
                solicitud.TipoRelacion
                == TipoRelacionEnum.Propietario)
            {


                solicitud.Vivienda.Estado =
                    EstadoViviendaEnum.Ocupada;


            }




            await _context.SaveChangesAsync();




            return RedirectToAction("Index");

        }







        [HttpPost]
        public async Task<IActionResult> Rechazar(int id)
        {


            var solicitud =
                await _context.ViviendaUsuarios
                .Include(x => x.Usuario)
                .FirstOrDefaultAsync(x => x.Id == id);



            if (solicitud == null)
                return NotFound();




            solicitud.Estado =
                EstadoUsuarioEnum.Rechazado;



            solicitud.Usuario.Estado =
                EstadoUsuarioEnum.Rechazado;




            await _context.SaveChangesAsync();




            return RedirectToAction("Index");

        }


    }

}