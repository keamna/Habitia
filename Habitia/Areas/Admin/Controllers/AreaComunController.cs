// /Areas/Admin/Controllers/AreaComunController.cs
using Habitia.Data;
using Habitia.Enums;
using Habitia.Helpers;
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

            // Antes faltaba el "return": aunque el modelo fuera inválido, el área
            // se guardaba igual y nunca se veían los mensajes de validación.
            if (!ModelState.IsValid)
            {
                ViewBag.Tipos = new SelectList(_context.TiposArea, "Id", "Nombre", vm.IdTipo);
                return View(vm);
            }



            var codigoNormalizado = vm.Codigo.Trim();

            // El código de área común debe ser único.
            var codigoRepetido = await _context.AreasComunes
                .AnyAsync(a => a.TC_Codigo.ToLower() == codigoNormalizado.ToLower());

            if (codigoRepetido)
            {
                ModelState.AddModelError(nameof(vm.Codigo),
                    "Ya existe un área común con ese código.");

                ViewBag.Tipos = new SelectList(_context.TiposArea, "Id", "Nombre", vm.IdTipo);
                return View(vm);
            }

            var area = new AreaComun
            {
                TC_Nombre = vm.Nombre.Trim(),
                TC_Codigo = codigoNormalizado,
                TN_IdTipo = vm.IdTipo!.Value,
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



            var codigoNormalizado = vm.Codigo.Trim();

            // El código debe ser único, sin contarse a sí misma.
            var codigoRepetido = await _context.AreasComunes
                .AnyAsync(a => a.TC_Codigo.ToLower() == codigoNormalizado.ToLower() &&
                               a.TN_Id != vm.Id);

            if (codigoRepetido)
            {
                ModelState.AddModelError(nameof(vm.Codigo),
                    "Ya existe un área común con ese código.");

                ViewBag.Tipos = new SelectList(_context.TiposArea, "Id", "Nombre", vm.IdTipo);
                vm.FotosExistentes = area.Fotos.Where(f => f.TB_Estado).ToList();
                return View(vm);
            }

            area.TC_Nombre = vm.Nombre.Trim();
            area.TC_Codigo = codigoNormalizado;
            area.TN_IdTipo = vm.IdTipo!.Value;
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



            // Solo los horarios vigentes: los eliminados quedan con TB_Estado = false.
            ViewBag.Disponibilidades =
                _context.Disponibilidades
                .Where(d => d.TN_IdAreaComun == id && d.TB_Estado)
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

            if (vm.Fecha.Date < DateTime.Today)
            {
                TempData["Error"] = "No se puede registrar un horario en una fecha que ya pasó.";
                return RedirectToAction(nameof(Detalle), new { id = vm.IdAreaComun });
            }

            if (fechaHoraInicio <= DateTime.Now)
            {
                TempData["Error"] = "La hora de inicio ya pasó. Indique una hora posterior a la actual.";
                return RedirectToAction(nameof(Detalle), new { id = vm.IdAreaComun });
            }

            // La anticipación mínima no puede ser mayor al tiempo que falta para
            // que inicie el horario: si no, nadie podría cancelar nunca.
            if (vm.AnticipacionEnMinutos > (fechaHoraInicio - DateTime.Now).TotalMinutes)
            {
                TempData["Error"] = "La anticipación mínima es mayor al tiempo que falta para que inicie el horario.";
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
                TN_AnticipacionMinima = vm.AnticipacionEnMinutos,
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



            if (disponibilidad == null)
            {
                TempData["Error"] = "No se encontró el horario indicado.";
                return RedirectToAction(nameof(Detalle), new { id = areaId });
            }

            // Si un residente ya reservó este horario, no se puede eliminar.
            var tieneReservaActiva = await _context.Reservas
                .AnyAsync(r => r.TN_IdDisponibilidad == id &&
                               r.TN_Estado == EstadoReservaEnum.Activa);

            if (disponibilidad.TB_Reservado || tieneReservaActiva)
            {
                TempData["Error"] = "No se puede eliminar este horario porque ya fue reservado por un residente. Cancele la reserva primero.";
                return RedirectToAction(nameof(Detalle), new { id = areaId });
            }

            disponibilidad.TB_Estado = false;
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Horario eliminado correctamente.";



            return RedirectToAction(
                nameof(Detalle),
                new { id = areaId });

        }









        // =========================
        // VERIFICAR SI EL AREA SE PUEDE ELIMINAR
        // =========================
        [HttpGet]
        public async Task<IActionResult> VerificarEliminacion(int id)
        {
            var area = await _context.AreasComunes
                .FirstOrDefaultAsync(a => a.TN_Id == id);

            if (area == null)
            {
                return Json(new { puedeEliminar = false, message = "Área común no encontrada." });
            }

            var mensaje = await ValidarEliminacionAreaAsync(id);

            if (mensaje != null)
            {
                return Json(new { puedeEliminar = false, message = mensaje });
            }

            return Json(new { puedeEliminar = true });
        }


        // Devuelve el motivo por el que el área NO se puede eliminar,
        // o null si sí se puede.
        //
        // Antes de contar las reservas activas, se actualizan las vencidas a
        // Finalizada (vía ReservaHelper) para que TN_Estado en BD sea la
        // fuente de verdad en vez de recalcular la fecha de fin aquí mismo.
        private async Task<string?> ValidarEliminacionAreaAsync(int idArea)
        {
            // 1) Reservas activas cuyo horario todavía no terminó.
            await ReservaHelper.FinalizarReservasVencidasAsync(_context);

            var vigentes = await _context.Reservas
                .CountAsync(r =>
                    r.Disponibilidad.TN_IdAreaComun == idArea &&
                    r.TN_Estado == EstadoReservaEnum.Activa);

            if (vigentes > 0)
            {
                return vigentes == 1
                    ? "No se puede eliminar el área común porque tiene 1 reserva activa. Cancele esa reserva primero."
                    : $"No se puede eliminar el área común porque tiene {vigentes} reservas activas. Cancele esas reservas primero.";
            }

            // 2) Incidencias reportadas sobre el área.
            //    La FK está configurada con DeleteBehavior.Restrict, así que sin
            //    esta validación SQL Server rechazaba el DELETE y el usuario solo
            //    veía "No fue posible completar la operación".
            var incidencias = await _context.Incidencias
                .CountAsync(i => i.TN_IdAreaComun == idArea);

            if (incidencias > 0)
            {
                return incidencias == 1
                    ? "No se puede eliminar el área común porque tiene 1 incidencia asociada. El historial de incidencias no se puede borrar; puede desactivar el área desde Editar."
                    : $"No se puede eliminar el área común porque tiene {incidencias} incidencias asociadas. El historial de incidencias no se puede borrar; puede desactivar el área desde Editar.";
            }

            // 3) Tareas de mantenimiento sobre el área (misma FK Restrict).
            var mantenimientos = await _context.Mantenimientos
                .CountAsync(m => m.TN_IdAreaComun == idArea);

            if (mantenimientos > 0)
            {
                return mantenimientos == 1
                    ? "No se puede eliminar el área común porque tiene 1 tarea de mantenimiento asociada. Puede desactivar el área desde Editar."
                    : $"No se puede eliminar el área común porque tiene {mantenimientos} tareas de mantenimiento asociadas. Puede desactivar el área desde Editar.";
            }

            return null;
        }


        // =========================
        // ELIMINAR AREA COMUN
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar([FromBody] IdAreaComunVM model)
        {
            var area = await _context.AreasComunes
                .Include(a => a.Fotos)
                .Include(a => a.Disponibilidades)
                .FirstOrDefaultAsync(a => a.TN_Id == model.Id);

            if (area == null)
            {
                return Json(new { success = false, message = "Área común no encontrada." });
            }

            // Se revalida: pudo entrar una reserva nueva mientras el modal estaba abierto.
            var mensaje = await ValidarEliminacionAreaAsync(model.Id);

            if (mensaje != null)
            {
                return Json(new { success = false, message = mensaje });
            }

            var idsDisponibilidades = area.Disponibilidades
                .Select(d => d.TN_Id)
                .ToList();

            // Reservas históricas (finalizadas o canceladas): no bloquean, pero
            // referencian los horarios por FK, así que se limpian antes.
            var reservasHistoricas = await _context.Reservas
                .Where(r => idsDisponibilidades.Contains(r.TN_IdDisponibilidad))
                .ToListAsync();

            if (reservasHistoricas.Any())
                _context.Reservas.RemoveRange(reservasHistoricas);

            // Archivos físicos de las fotos.
            foreach (var foto in area.Fotos)
            {
                var ruta = Path.Combine(_env.WebRootPath, foto.TC_Url.TrimStart('/')
                    .Replace('/', Path.DirectorySeparatorChar));

                if (System.IO.File.Exists(ruta))
                {
                    try { System.IO.File.Delete(ruta); } catch { /* si falla, se ignora */ }
                }
            }

            _context.AreaComunFotos.RemoveRange(area.Fotos);
            _context.Disponibilidades.RemoveRange(area.Disponibilidades);
            _context.AreasComunes.Remove(area);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Red de seguridad: si quedó algún otro registro apuntando al área,
                // se avisa en vez de devolver un error 500 sin explicación.
                return Json(new
                {
                    success = false,
                    message = "No se puede eliminar el área común porque tiene información asociada en otras secciones del sistema. Puede desactivarla desde Editar."
                });
            }

            return Json(new { success = true });
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