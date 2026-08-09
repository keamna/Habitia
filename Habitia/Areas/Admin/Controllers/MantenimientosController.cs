using Habitia.Data;
using Habitia.Enums;
using Habitia.Models;
using Habitia.Services.Interfaces;
using Habitia.ViewModels;
using Habitia.ViewModels.Mantenimiento;
using Habitia.ViewModels.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MantenimientosController : Controller
    {
        private readonly IMantenimientoService _mantenimientoService;
        private readonly ITipoMantenimientoService _tipoMantenimientoService;
        private readonly IIncidenciaService _incidenciaService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public MantenimientosController(
            IMantenimientoService mantenimientoService,
            ITipoMantenimientoService tipoMantenimientoService,
            IIncidenciaService incidenciaService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _mantenimientoService = mantenimientoService;
            _tipoMantenimientoService = tipoMantenimientoService;
            _incidenciaService = incidenciaService;
            _userManager = userManager;
            _context = context;
        }

        // GET: /Admin/Mantenimientos
        public async Task<IActionResult> Index()
        {
            var mantenimientos = await _mantenimientoService.ObtenerTodosAsync();
            return View(mantenimientos);
        }

        public async Task<IActionResult> Create(int? idIncidencia)
        {
            var model = new MantenimientoCreateViewModel();

            if (idIncidencia.HasValue)
            {
                var incidencia = await _incidenciaService.ObtenerPorIdAsync(idIncidencia.Value);

                if (incidencia == null)
                    return NotFound();

                if (incidencia.TN_Responsabilidad == ResponsabilidadEnum.Privado)
                {
                    TempData["Error"] = "Las incidencias privadas no requieren tarea de mantenimiento.";
                    return RedirectToAction("Details", "Incidencias", new { id = idIncidencia.Value });
                }

                if (await _incidenciaService.TieneMantenimientoAsociadoAsync(idIncidencia.Value))
                {
                    TempData["Error"] = "Esta incidencia ya tiene una tarea de mantenimiento asociada.";
                    return RedirectToAction("Details", "Incidencias", new { id = idIncidencia.Value });
                }

                model.IdIncidencia = idIncidencia.Value;
                model.Descripcion = incidencia.TC_Titulo;
            }

            await CargarListasAsync(model, idIncidencia.HasValue);
            return View(model);
        }

        // POST: /Admin/Mantenimientos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MantenimientoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasAsync(model, false);
                return View(model);
            }

            try
            {
                await _mantenimientoService.ConvertirDesdeIncidenciaAsync(model);

                TempData["Success"] = "Tarea de mantenimiento creada y asignada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync(model, false);
                return View(model);
            }
        }

        // GET: /Admin/Mantenimientos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var mantenimiento = await _mantenimientoService.ObtenerPorIdAsync(id);

            if (mantenimiento == null)
                return NotFound();

            return View(mantenimiento);
        }

        // GET: /Admin/Mantenimientos/PersonalPorTipo?idTipo=3
        // Devuelve el personal ACTIVO configurado para atender ese tipo de mantenimiento.
        // Usado por el JS de Create.cshtml para filtrar el <select> dinámicamente.
        [HttpGet]
        public async Task<IActionResult> PersonalPorTipo(int idTipo)
        {
            var personal = await _mantenimientoService.ObtenerPersonalActivoPorTipoAsync(idTipo);

            var resultado = personal.Select(u => new
            {
                id = u.Id,
                nombre = u.UserName
            });

            return Json(resultado);
        }

        // ==========================
        // LISTADO DE PERSONAL DE MANTENIMIENTO (filtrable por tipo)
        // ==========================
        // GET: /Admin/Mantenimientos/Personal?idTipo=3
        [HttpGet]
        public async Task<IActionResult> Personal(int? idTipo)
        {
            CargarTiposMantenimientoViewBag();
            ViewBag.IdTipoSeleccionado = idTipo;

            var personalUsers = await _userManager.GetUsersInRoleAsync("Mantenimiento");

            var asignaciones = await _context.PersonalTipoMantenimiento.ToListAsync();
            var tiposTodos = await _context.TiposMantenimiento.ToListAsync();

            var idsFiltrados = idTipo.HasValue
                ? asignaciones.Where(a => a.TN_IdTipo == idTipo.Value).Select(a => a.TC_IdPersonal).ToHashSet()
                : null;

            var lista = personalUsers
                .Where(u => idsFiltrados == null || idsFiltrados.Contains(u.Id))
                .OrderBy(u => u.TC_Nombre)
                .Select(u => new PersonalMantenimientoListItemViewModel
                {
                    Id = u.Id,
                    Nombre = u.TC_Nombre,
                    Apellido = u.TC_Apellido,
                    NombreCompleto = $"{u.TC_Nombre} {u.TC_Apellido}",
                    Identificacion = u.TC_Identificacion,
                    Telefono = u.TC_Telefono,
                    Email = u.Email,
                    Estado = u.TN_Estado.ToString(),
                    TiposIds = asignaciones
                        .Where(a => a.TC_IdPersonal == u.Id)
                        .Select(a => a.TN_IdTipo)
                        .ToList(),
                    TiposNombres = asignaciones
                        .Where(a => a.TC_IdPersonal == u.Id)
                        .Join(tiposTodos, a => a.TN_IdTipo, t => t.TN_Id, (a, t) => t.TC_Nombre)
                        .ToList()
                })
                .ToList();

            return View(lista);
        }

        // ==========================
        // CREAR PERSONAL DE MANTENIMIENTO (sin salir del módulo)
        // ==========================
        // GET: /Admin/Mantenimientos/CrearPersonal
        [HttpGet]
        public IActionResult CrearPersonal()
        {
            CargarTiposMantenimientoViewBag();
            return View();
        }

        // POST: /Admin/Mantenimientos/CrearPersonal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearPersonal(CrearUsuarioViewModel model)
        {
            // El rol siempre es Mantenimiento en este flujo; se fuerza en servidor
            // y se limpia cualquier error de validación que hubiera quedado sobre ese campo.
            model.Rol = "Mantenimiento";
            ModelState.Remove(nameof(model.Rol));

            CargarTiposMantenimientoViewBag();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.TipoIdentificacion == null)
            {
                ModelState.AddModelError(
                    nameof(model.TipoIdentificacion),
                    "Debe seleccionar el tipo de identificación.");
                return View(model);
            }

            if (model.TiposMantenimientoIds == null || !model.TiposMantenimientoIds.Any())
            {
                ModelState.AddModelError(
                    nameof(model.TiposMantenimientoIds),
                    "Debe asignar al menos un tipo de mantenimiento.");
                return View(model);
            }

            var usuario = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                TC_Nombre = model.Nombre,
                TC_Apellido = model.Apellido,
                TN_TipoIdentificacion = model.TipoIdentificacion.Value,
                TC_Identificacion = model.Identificacion,
                TC_Telefono = model.Telefono,
                TN_Estado = EstadoUsuarioEnum.Activo,
                TF_FechaRegistro = DateTime.Now
            };

            var resultado = await _userManager.CreateAsync(usuario, model.Password);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            var rolValido = _context.Roles.Any(x => x.Name == "Mantenimiento");
            if (rolValido)
            {
                await _userManager.AddToRoleAsync(usuario, "Mantenimiento");
            }

            await AsignarTiposMantenimiento(usuario.Id, model.TiposMantenimientoIds!);

            TempData["Success"] = "Personal de mantenimiento creado correctamente.";
            return RedirectToAction(nameof(Personal));
        }

        // ==========================
        // EDITAR PERSONAL (datos básicos + tipos)
        // ==========================
        // POST: /Admin/Mantenimientos/EditarPersonal
        [HttpPost]
        public async Task<IActionResult> EditarPersonal([FromBody] EditarPersonalMantenimientoVM model)
        {
            var usuario = await _userManager.FindByIdAsync(model.Id);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Personal no encontrado." });
            }

            if (!await _userManager.IsInRoleAsync(usuario, "Mantenimiento"))
            {
                return Json(new { success = false, message = "Este usuario no pertenece al personal de mantenimiento." });
            }

            if (string.IsNullOrWhiteSpace(model.Nombre) || string.IsNullOrWhiteSpace(model.Apellido))
            {
                return Json(new { success = false, message = "Nombre y apellido son obligatorios." });
            }

            if (model.TiposMantenimientoIds == null || !model.TiposMantenimientoIds.Any())
            {
                return Json(new { success = false, message = "Debe asignar al menos un tipo de mantenimiento." });
            }

            usuario.TC_Nombre = model.Nombre.Trim();
            usuario.TC_Apellido = model.Apellido.Trim();
            usuario.TC_Telefono = model.Telefono?.Trim();

            await _userManager.UpdateAsync(usuario);
            await AsignarTiposMantenimiento(usuario.Id, model.TiposMantenimientoIds);

            return Json(new { success = true });
        }

        // ==========================
        // SUSPENDER PERSONAL
        // ==========================
        // POST: /Admin/Mantenimientos/SuspenderPersonal
        [HttpPost]
        public async Task<IActionResult> SuspenderPersonal([FromBody] IdUsuarioVM model)
        {
            var usuario = await _userManager.FindByIdAsync(model.Id);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Personal no encontrado." });
            }

            usuario.TN_Estado = EstadoUsuarioEnum.Suspendido;
            await _userManager.UpdateAsync(usuario);

            // Invalida la sesión activa (si estaba conectado, se le cierra la sesión)
            await _userManager.UpdateSecurityStampAsync(usuario);

            return Json(new { success = true });
        }

        // ==========================
        // REACTIVAR PERSONAL
        // ==========================
        // POST: /Admin/Mantenimientos/ReactivarPersonal
        [HttpPost]
        public async Task<IActionResult> ReactivarPersonal([FromBody] IdUsuarioVM model)
        {
            var usuario = await _userManager.FindByIdAsync(model.Id);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Personal no encontrado." });
            }

            usuario.TN_Estado = EstadoUsuarioEnum.Activo;
            await _userManager.UpdateAsync(usuario);

            return Json(new { success = true });
        }

        // ==========================
        // ELIMINAR PERSONAL
        // ==========================
        // POST: /Admin/Mantenimientos/EliminarPersonal
        [HttpPost]
        public async Task<IActionResult> EliminarPersonal([FromBody] IdUsuarioVM model)
        {
            var usuario = await _userManager.FindByIdAsync(model.Id);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Personal no encontrado." });
            }

            if (!await _userManager.IsInRoleAsync(usuario, "Mantenimiento"))
            {
                return Json(new { success = false, message = "Este usuario no pertenece al personal de mantenimiento." });
            }

            // No se puede eliminar a alguien con tareas de mantenimiento todavía en curso
            var tieneTareasActivas = await _context.Mantenimientos.AnyAsync(m =>
                m.TC_IdPersonalAsignado == model.Id &&
                m.TN_Estado != EstadoMantenimientoEnum.Completado &&
                m.TN_Estado != EstadoMantenimientoEnum.Cancelado);

            if (tieneTareasActivas)
            {
                return Json(new { success = false, message = "No se puede eliminar: tiene tareas de mantenimiento activas asignadas." });
            }

            var tipos = _context.PersonalTipoMantenimiento.Where(x => x.TC_IdPersonal == model.Id);
            _context.PersonalTipoMantenimiento.RemoveRange(tipos);
            await _context.SaveChangesAsync();

            var resultado = await _userManager.DeleteAsync(usuario);

            if (!resultado.Succeeded)
            {
                return Json(new { success = false, message = "No se pudo eliminar el personal." });
            }

            return Json(new { success = true });
        }

        private void CargarTiposMantenimientoViewBag()
        {
            ViewBag.TiposMantenimiento = _context.TiposMantenimiento
                .Where(t => t.TB_Estado)
                .OrderBy(t => t.TC_Nombre)
                .Select(t => new SelectListItem
                {
                    Value = t.TN_Id.ToString(),
                    Text = t.TC_Nombre
                })
                .ToList();
        }

        private async Task AsignarTiposMantenimiento(string idUsuario, List<int> tiposIds)
        {
            var actuales = _context.PersonalTipoMantenimiento
                .Where(x => x.TC_IdPersonal == idUsuario);

            _context.PersonalTipoMantenimiento.RemoveRange(actuales);

            foreach (var idTipo in tiposIds.Distinct())
            {
                _context.PersonalTipoMantenimiento.Add(new PersonalTipoMantenimiento
                {
                    TC_IdPersonal = idUsuario,
                    TN_IdTipo = idTipo
                });
            }

            await _context.SaveChangesAsync();
        }

        private async Task CargarListasAsync(MantenimientoCreateViewModel model, bool incidenciaFija)
        {
            var tipos = await _tipoMantenimientoService.ObtenerActivosAsync();
            model.TiposMantenimiento = tipos.Select(t => new SelectListItem
            {
                Value = t.TN_Id.ToString(),
                Text = t.TC_Nombre
            }).ToList();

            // Carga inicial completa (personal activo); el JS la reemplaza al elegir un tipo existente.
            var personalMantenimiento = await _userManager.GetUsersInRoleAsync("Mantenimiento");
            model.PersonalMantenimiento = personalMantenimiento
                .Where(u => u.TN_Estado == EstadoUsuarioEnum.Activo)
                .OrderBy(u => u.UserName)
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.UserName
                }).ToList();

            if (!incidenciaFija)
            {
                var elegibles = await _incidenciaService.ObtenerElegiblesParaMantenimientoAsync();
                model.IncidenciasElegibles = elegibles.Select(i => new SelectListItem
                {
                    Value = i.TN_Id.ToString(),
                    Text = $"{i.TC_Titulo} ({i.TN_Responsabilidad})"
                }).ToList();
            }
        }
    }
}