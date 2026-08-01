using Habitia.Data;
using Habitia.Enums;
using Habitia.Helpers;
using Habitia.Models.Acceso;
using Habitia.ViewModels.Acceso;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Seguridad.Controllers
{
    [Area("Seguridad")]
    [Authorize(Roles = "Seguridad")]
    public class AccesosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccesosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= Consulta de accesos =================
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

            return View(vm);
        }

        // ================= Búsqueda de visitantes para reutilizar (AJAX) =================
        [HttpGet]
        public async Task<IActionResult> BuscarVisitantes(string termino)
        {
            var resultado = await AccesoHelper.BuscarVisitantesAsync(_context, termino);
            return Json(resultado);
        }

        // ================= Búsqueda combinada de residente + vivienda (AJAX) =================
        [HttpGet]
        public async Task<IActionResult> BuscarResidentes(string termino)
        {
            var resultado = await AccesoHelper.BuscarResidentesAsync(_context, termino);
            return Json(resultado);
        }

        // ================= Registro manual =================
        public IActionResult RegistroManual()
        {
            return View(new RegistroManualVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistroManual(RegistroManualVM vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Debe completar todos los campos requeridos";
                return View(vm);
            }

            int idVisitante;

            if (vm.IdVisitanteExistente.HasValue)
            {
                var existente = await _context.Visitantes
                    .FirstOrDefaultAsync(v => v.TN_Id == vm.IdVisitanteExistente.Value && v.TB_Estado);

                if (existente == null)
                {
                    TempData["Error"] = "El visitante seleccionado ya no está disponible.";
                    return View(vm);
                }

                idVisitante = existente.TN_Id;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(vm.NombreVisitante) ||
                    string.IsNullOrWhiteSpace(vm.IdentificacionVisitante) ||
                    vm.TipoIdentificacionVisitante == null)
                {
                    TempData["Error"] = "Debe completar todos los campos requeridos";
                    return View(vm);
                }

                var visitante = new Visitante
                {
                    TC_Nombre = vm.NombreVisitante,
                    TN_TipoIdentificacion = vm.TipoIdentificacionVisitante.Value,
                    TC_Identificacion = vm.IdentificacionVisitante,
                    TC_Telefono = vm.TelefonoVisitante,
                    TB_Estado = true
                };

                _context.Visitantes.Add(visitante);
                await _context.SaveChangesAsync();

                idVisitante = visitante.TN_Id;
            }

            // Residente/vivienda: el visitante debe quedar asociado obligatoriamente (US-04, punto 2)
            var viviendaExiste = await _context.Viviendas.AnyAsync(v => v.TN_Id == vm.IdVivienda);
            var usuarioExiste = await _context.Users.AnyAsync(u => u.Id == vm.IdUsuario);

            if (!viviendaExiste || !usuarioExiste)
            {
                TempData["Error"] = "Debe asociar el visitante a un residente o unidad habitacional";
                return View(vm);
            }

            int? idVehiculo = null;

            if (vm.IngresaEnVehiculo)
            {
                if (string.IsNullOrWhiteSpace(vm.Placa))
                {
                    TempData["Error"] = "Debe ingresar el número de placa del vehículo";
                    return View(vm);
                }

                var vehiculo = new Vehiculo
                {
                    TN_IdVisitante = idVisitante,
                    TC_Placa = vm.Placa,
                    TC_Tipo = vm.TipoVehiculo,
                    TC_Observaciones = vm.ObservacionesVehiculo,
                    TB_Estado = true
                };

                _context.Vehiculos.Add(vehiculo);
                await _context.SaveChangesAsync();

                idVehiculo = vehiculo.TN_Id;
            }

            var acceso = new Acceso
            {
                TN_IdVisitante = idVisitante,
                TC_IdUsuario = vm.IdUsuario,
                TN_IdVivienda = vm.IdVivienda,
                TC_Motivo = vm.Motivo,
                TN_IdAutorizacion = null,
                TN_IdVehiculo = idVehiculo,
                TF_FechaIngreso = DateTime.Now,
                TF_FechaSalida = null,
                TB_Estado = true
            };

            _context.Accesos.Add(acceso);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Ingreso registrado correctamente";

            return RedirectToAction(nameof(Index));
        }

        // ================= Validar autorización por QR =================
        public IActionResult ValidarQR()
        {
            return View(new ValidarQRVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidarQR(ValidarQRVM vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "La autorización de ingreso no es válida";
                return View(vm);
            }

            await AccesoHelper.ExpirarAutorizacionesVencidasAsync(_context);

            var autorizacion = await _context.Autorizaciones
                .Include(a => a.Visitante)
                .Include(a => a.Usuario)
                .Include(a => a.Vivienda)
                .FirstOrDefaultAsync(a => a.TC_Codigo == vm.Codigo.Trim());

            if (autorizacion == null || autorizacion.TN_Estado != EstadoAutorizacionEnum.Pendiente)
            {
                TempData["Error"] = "La autorización de ingreso no es válida";
                return View(vm);
            }

            var detalle = AccesoHelper.MapAutorizacionToVM(autorizacion, mostrarDatosResidente: true, puedeInvalidar: false);

            return View("ConfirmarIngresoQR", detalle);
        }

        // ================= Confirmar el ingreso validado por QR =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarIngresoQR(ConfirmarIngresoQRVM vm)
        {
            var autorizacion = await _context.Autorizaciones
                .Include(a => a.Visitante)
                .FirstOrDefaultAsync(a => a.TN_Id == vm.IdAutorizacion);

            if (autorizacion == null || autorizacion.TN_Estado != EstadoAutorizacionEnum.Pendiente)
            {
                TempData["Error"] = "La autorización de ingreso no es válida";
                return RedirectToAction(nameof(ValidarQR));
            }

            if (autorizacion.TF_FechaVencimiento <= DateTime.Now)
            {
                autorizacion.TN_Estado = EstadoAutorizacionEnum.Expirada;
                await _context.SaveChangesAsync();

                TempData["Error"] = "La autorización de ingreso no es válida";
                return RedirectToAction(nameof(ValidarQR));
            }

            int? idVehiculo = null;

            if (vm.IngresaEnVehiculo)
            {
                if (string.IsNullOrWhiteSpace(vm.Placa))
                {
                    TempData["Error"] = "Debe ingresar el número de placa del vehículo";
                    var detalle = AccesoHelper.MapAutorizacionToVM(autorizacion, mostrarDatosResidente: true, puedeInvalidar: false);
                    return View("ConfirmarIngresoQR", detalle);
                }

                var vehiculo = new Vehiculo
                {
                    TN_IdVisitante = autorizacion.TN_IdVisitante,
                    TC_Placa = vm.Placa,
                    TC_Tipo = vm.TipoVehiculo,
                    TC_Observaciones = vm.ObservacionesVehiculo,
                    TB_Estado = true
                };

                _context.Vehiculos.Add(vehiculo);
                await _context.SaveChangesAsync();

                idVehiculo = vehiculo.TN_Id;
            }

            var acceso = new Acceso
            {
                TN_IdVisitante = autorizacion.TN_IdVisitante,
                TC_IdUsuario = autorizacion.TC_IdUsuario,
                TN_IdVivienda = autorizacion.TN_IdVivienda,
                TC_Motivo = autorizacion.TC_Motivo,
                TN_IdAutorizacion = autorizacion.TN_Id,
                TN_IdVehiculo = idVehiculo,
                TF_FechaIngreso = DateTime.Now,
                TF_FechaSalida = null,
                TB_Estado = true
            };

            autorizacion.TN_Estado = EstadoAutorizacionEnum.Activa;

            _context.Accesos.Add(acceso);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Ingreso autorizado correctamente";

            return RedirectToAction(nameof(Index));
        }

        // ================= Registrar salida =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarSalida(int id)
        {
            var acceso = await _context.Accesos
                .Include(a => a.Autorizacion)
                .FirstOrDefaultAsync(a => a.TN_Id == id);

            if (acceso == null || acceso.TF_FechaSalida != null)
            {
                TempData["Error"] = "No se encontró un ingreso activo para este visitante";
                return RedirectToAction(nameof(Index));
            }

            acceso.TF_FechaSalida = DateTime.Now;

            if (acceso.Autorizacion != null)
                acceso.Autorizacion.TN_Estado = EstadoAutorizacionEnum.Finalizada;

            await _context.SaveChangesAsync();

            TempData["Exito"] = "Salida registrada correctamente";

            return RedirectToAction(nameof(Index));
        }
    }
}