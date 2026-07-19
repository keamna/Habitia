using Habitia.Data;
using Habitia.Models;
using Habitia.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AreaComunController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;


        public AreaComunController(
            ApplicationDbContext context,
            IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }



        // =========================
        // LISTA
        // =========================
        public IActionResult Index()
        {
            var lista = _context.AreasComunes
                .Include(a => a.Fotos)
                .Include(a => a.Tipo)
                .ToList();


            return View(lista);
        }




        // =========================
        // CREATE GET
        // =========================
        public IActionResult Create()
        {
            ViewBag.Tipos = new SelectList(_context.TiposArea, "Id", "Nombre");
            return View();
        }





        // =========================
        // CREATE POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AreaComunVM vm)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.Tipos = new SelectList(_context.TiposArea, "Id", "Nombre", vm.IdTipo);
                return View(vm);
            }



            var area = new AreaComun
            {
                Nombre = vm.Nombre,
                Codigo = vm.Codigo,
                IdTipo = vm.IdTipo,
                Capacidad = vm.Capacidad,
                Estado = true
            };


            _context.AreasComunes.Add(area);

            await _context.SaveChangesAsync();



            await GuardarFotos(vm.Fotos, area.Id);



            return RedirectToAction(nameof(Index));

        }





        // =========================
        // EDIT GET
        // =========================
        public IActionResult Edit(int id)
        {

            var area = _context.AreasComunes
                .Include(a => a.Fotos)
                .FirstOrDefault(a => a.Id == id);


            if (area == null)
                return NotFound();



            ViewBag.Tipos = new SelectList(_context.TiposArea, "Id", "Nombre", area.IdTipo);



            var vm = new AreaComunVM
            {
                Id = area.Id,
                Nombre = area.Nombre,
                Codigo = area.Codigo,
                IdTipo = area.IdTipo,
                Capacidad = area.Capacidad,
                Estado = area.Estado,
                FotosExistentes = area.Fotos
                    .Where(f => f.Estado)
                    .ToList()
            };


            return View(vm);

        }





        // =========================
        // EDIT POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AreaComunVM vm)
        {

            var area = await _context.AreasComunes
                .Include(a => a.Fotos)
                .FirstOrDefaultAsync(a => a.Id == vm.Id);



            if (area == null)
                return NotFound();



            if (!ModelState.IsValid)
            {
                ViewBag.Tipos = new SelectList(_context.TiposArea, "Id", "Nombre", vm.IdTipo);
                vm.FotosExistentes = area.Fotos
                    .Where(f => f.Estado)
                    .ToList();
                return View(vm);
            }



            area.Nombre = vm.Nombre;
            area.Codigo = vm.Codigo;
            area.IdTipo = vm.IdTipo;
            area.Capacidad = vm.Capacidad;
            area.Estado = vm.Estado;



            await GuardarFotos(vm.Fotos, area.Id);



            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));

        }





        // =========================
        // DETALLE
        // =========================
        public IActionResult Detalle(int id)
        {

            var area = _context.AreasComunes
                .Include(a => a.Fotos)
                .FirstOrDefault(a => a.Id == id);



            if (area == null)
                return NotFound();



            ViewBag.Disponibilidades =
                _context.Disponibilidades
                .Where(d => d.IdAreaComun == id)
                .OrderBy(d => d.Fecha)
                .ThenBy(d => d.HoraInicio)
                .ToList();



            return View(area);

        }







        // =========================
        // SUBIR FOTO
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubirFoto(
            int areaId,
            IFormFile archivo)
        {

            if (archivo == null)
                return RedirectToAction(nameof(Detalle), new { id = areaId });



            await GuardarFotos(
                new List<IFormFile> { archivo },
                areaId);



            return RedirectToAction(
                nameof(Detalle),
                new { id = areaId });

        }







        // =========================
        // ELIMINAR FOTO
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarFoto(
            int fotoId,
            int areaId)
        {


            var foto = await _context.AreaComunFotos
                .FirstOrDefaultAsync(f => f.Id == fotoId);



            if (foto != null)
            {
                foto.Estado = false;

                await _context.SaveChangesAsync();
            }



            return RedirectToAction(
                nameof(Detalle),
                new { id = areaId });

        }







        // =========================
        // AGREGAR DISPONIBILIDAD
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarDisponibilidad(
            DisponibilidadFormVM vm)
        {

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Complete todos los campos del horario.";
                return RedirectToAction(nameof(Detalle), new { id = vm.IdAreaComun });
            }



            TimeSpan horaInicio;
            TimeSpan horaFin;

            try
            {
                horaInicio = DateTime.ParseExact(
                    $"{vm.HoraInicioHora}:{vm.HoraInicioMinuto:D2} {vm.HoraInicioAmPm}",
                    "h:mm tt",
                    System.Globalization.CultureInfo.InvariantCulture).TimeOfDay;

                horaFin = DateTime.ParseExact(
                    $"{vm.HoraFinHora}:{vm.HoraFinMinuto:D2} {vm.HoraFinAmPm}",
                    "h:mm tt",
                    System.Globalization.CultureInfo.InvariantCulture).TimeOfDay;
            }
            catch (FormatException)
            {
                TempData["Error"] = "La hora ingresada no es válida.";
                return RedirectToAction(nameof(Detalle), new { id = vm.IdAreaComun });
            }



            if (horaInicio >= horaFin)
            {
                TempData["Error"] = "La hora de inicio debe ser menor que la hora de finalización.";
                return RedirectToAction(nameof(Detalle), new { id = vm.IdAreaComun });
            }



            bool existeDuplicado = await _context.Disponibilidades
                .AnyAsync(d =>
                    d.IdAreaComun == vm.IdAreaComun &&
                    d.Fecha == vm.Fecha.Date &&
                    d.HoraInicio == horaInicio &&
                    d.HoraFin == horaFin &&
                    d.Estado);

            if (existeDuplicado)
            {
                TempData["Error"] = "Ya existe ese horario registrado para esta área y fecha.";
                return RedirectToAction(nameof(Detalle), new { id = vm.IdAreaComun });
            }



            var disponibilidad = new DisponibilidadArea
            {
                IdAreaComun = vm.IdAreaComun,
                Fecha = vm.Fecha.Date,
                HoraInicio = horaInicio,
                HoraFin = horaFin,
                Estado = true
            };


            _context.Disponibilidades
                .Add(disponibilidad);



            await _context.SaveChangesAsync();


            TempData["Exito"] = "Horario agregado correctamente.";

            return RedirectToAction(
                nameof(Detalle),
                new { id = vm.IdAreaComun });

        }








        // =========================
        // ELIMINAR DISPONIBILIDAD
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarDisponibilidad(
            int id,
            int areaId)
        {

            var disponibilidad =
                await _context.Disponibilidades
                .FirstOrDefaultAsync(d => d.Id == id);



            if (disponibilidad != null)
            {
                disponibilidad.Estado = false;

                await _context.SaveChangesAsync();
            }



            return RedirectToAction(
                nameof(Detalle),
                new { id = areaId });

        }








        // =========================
        // GUARDAR FOTOS
        // =========================
        private async Task GuardarFotos(
            List<IFormFile> fotos,
            int areaId)
        {

            if (fotos == null || fotos.Count == 0)
                return;



            var carpeta =
                Path.Combine(
                    _env.WebRootPath,
                    "uploads",
                    "areas");



            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);



            int orden =
                await _context.AreaComunFotos
                .CountAsync(f => f.IdAreaComun == areaId) + 1;




            foreach (var foto in fotos)
            {

                var nombre =
                    Guid.NewGuid()
                    + Path.GetExtension(foto.FileName);



                var ruta =
                    Path.Combine(carpeta, nombre);



                using (var stream =
                    new FileStream(ruta, FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }




                _context.AreaComunFotos.Add(
                    new AreaComunFoto
                    {
                        IdAreaComun = areaId,
                        Url = "/uploads/areas/" + nombre,
                        Orden = orden,
                        EsPrincipal = orden == 1,
                        Estado = true
                    });


                orden++;

            }


            await _context.SaveChangesAsync();

        }

    }
}