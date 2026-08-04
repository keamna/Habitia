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
        public async Task<IActionResult> Create(AreaComunViewModel vm)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.Tipos = new SelectList(_context.TiposArea, "Id", "Nombre", vm.IdTipo);
            }



            var area = new AreaComun
            {
                TC_Nombre = vm.Nombre,
                TC_Codigo = vm.Codigo,
                TN_IdTipo = vm.IdTipo,
                TN_Capacidad = vm.Capacidad,
                TB_Estado = true
            };


            _context.AreasComunes.Add(area);

            await _context.SaveChangesAsync();



            await GuardarFotos(vm.Fotos, area.TN_Id);



            return RedirectToAction(nameof(Index));

        }





        // =========================
        // EDIT GET
        // =========================
        public IActionResult Edit(int id)
        {

            var area = _context.AreasComunes
                .Include(a => a.Fotos)
                .FirstOrDefault(a => a.TN_Id == id);


            if (area == null)
                return NotFound();



            ViewBag.Tipos = new SelectList(_context.TiposArea, "Id", "Nombre", area.TN_IdTipo);



            var vm = new AreaComunViewModel
            {
                Id = area.TN_Id,
                Nombre = area.TC_Nombre,
                Codigo = area.TC_Codigo,
                IdTipo = area.TN_IdTipo,
                Capacidad = area.TN_Capacidad,
                Estado = area.TB_Estado,
                FotosExistentes = area.Fotos
                    .Where(f => f.TB_Estado)
                    .ToList()
            };


            return View(vm);

        }





        // =========================
        // EDIT POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AreaComunViewModel vm)
        {

            var area = await _context.AreasComunes
                .Include(a => a.Fotos)
                .FirstOrDefaultAsync(a => a.TN_Id == vm.Id);



            if (area == null)
                return NotFound();



            if (!ModelState.IsValid)
            {
                ViewBag.Tipos = new SelectList(_context.TiposArea, "Id", "Nombre", vm.IdTipo);
                vm.FotosExistentes = area.Fotos
                    .Where(f => f.TB_Estado)
                    .ToList();
                return View(vm);
            }



            area.TC_Nombre = vm.Nombre;
            area.TC_Codigo = vm.Codigo;
            area.TN_IdTipo = vm.IdTipo;
            area.TN_Capacidad = vm.Capacidad;
            area.TB_Estado = vm.Estado;


            await GuardarFotos(vm.Fotos, area.TN_Id);



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
                .FirstOrDefault(a => a.TN_Id == id);



            if (area == null)
                return NotFound();



            ViewBag.Disponibilidades =
                _context.Disponibilidades
                .Where(d => d.TN_IdAreaComun == id)
                .OrderBy(d => d.TF_Fecha)
                .ThenBy(d => d.TF_HoraInicio)
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
                .FirstOrDefaultAsync(f => f.TN_Id == fotoId);



            if (foto != null)
            {
                foto.TB_Estado = false;

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

            if (vm.Cantidad <= 0)
            {
                TempData["Error"] = "Debe indicar la cantidad de personas permitidas.";
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

            // Validar que la fecha/hora no sea un momento ya pasado (5.1.5)
            var fechaHoraInicio = vm.Fecha.Date.Add(horaInicio);

            if (fechaHoraInicio <= DateTime.Now)
            {
                TempData["Error"] = "No se puede registrar un horario en una fecha u hora ya pasada.";
                return RedirectToAction(nameof(Detalle), new { id = vm.IdAreaComun });
            }

            // Validar que no exista conflicto (cruce) con otro horario ya registrado (5.1.6)
            bool existeConflicto = await _context.Disponibilidades
                .AnyAsync(d =>
                    d.TN_IdAreaComun == vm.IdAreaComun &&
                    d.TF_Fecha == vm.Fecha.Date &&
                    d.TB_Estado &&
                    horaInicio < d.TF_HoraFin &&
                    horaFin > d.TF_HoraInicio);

            if (existeConflicto)
            {
                TempData["Error"] = "El horario se cruza con otro horario ya registrado para esta área.";
                return RedirectToAction(nameof(Detalle), new { id = vm.IdAreaComun });
            }

            var disponibilidad = new DisponibilidadArea
            {
                TN_IdAreaComun = vm.IdAreaComun,
                TF_Fecha = vm.Fecha.Date,
                TF_HoraInicio = horaInicio,
                TF_HoraFin = horaFin,
                TN_Cantidad = vm.Cantidad,
                TB_Estado = true
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
                .FirstOrDefaultAsync(d => d.TN_Id == id);



            if (disponibilidad != null)
            {
                disponibilidad.TB_Estado = false;

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
                .CountAsync(f => f.TN_IdAreaComun == areaId) + 1;




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
                        TN_IdAreaComun = areaId,
                        TC_Url = "/uploads/areas/" + nombre,
                        TN_Orden = orden,
                        TB_EsPrincipal = orden == 1,
                        TB_Estado = true
                    });


                orden++;

            }


            await _context.SaveChangesAsync();

        }

    }
}