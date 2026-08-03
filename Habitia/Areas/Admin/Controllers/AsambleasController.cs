using System.Security.Claims;
using Habitia.Data;
using Habitia.Enums;
using Habitia.Models.Documentos;
using Habitia.ViewModels.Asamblea;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AsambleasController : Controller
    {
        private readonly ApplicationDbContext _context;


        public AsambleasController(ApplicationDbContext context)
        {
            _context = context;
        }


        private string UsuarioActualId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";


        // ===== LISTA =====
        public async Task<IActionResult> Index(EstadoAsambleaEnum? estado)
        {
            var query = _context.Asambleas.AsQueryable();

            if (estado.HasValue)
                query = query.Where(a => a.TN_Estado == estado.Value);

            var asambleas = await query
                .OrderByDescending(a => a.TF_FechaHora)
                .ToListAsync();

            // Conteo de confirmaciones por asamblea
            ViewBag.Confirmados = await _context.ParticipantesAsamblea
                .Where(p => p.TB_Confirmado)
                .GroupBy(p => p.TN_IdAsamblea)
                .Select(g => new { Id = g.Key, Total = g.Count() })
                .ToDictionaryAsync(x => x.Id, x => x.Total);

            ViewBag.FiltroEstado = estado;

            return View(asambleas);
        }


        // ===== CREAR (GET) =====
        public IActionResult Crear()
        {
            return View(new AsambleaFormVM());
        }


        // ===== CREAR (POST) =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(AsambleaFormVM vm)
        {
            ValidarAsamblea(vm, esNueva: true);

            if (!ModelState.IsValid)
                return View(vm);

            _context.Asambleas.Add(new Asamblea
            {
                TC_Titulo = vm.Titulo.Trim(),
                TF_FechaHora = vm.FechaHora,
                TN_Modalidad = vm.Modalidad,
                TC_Lugar = vm.Lugar?.Trim(),
                TC_Descripcion = vm.Descripcion?.Trim(),
                TN_Estado = EstadoAsambleaEnum.Programada,
                TC_IdUsuario = UsuarioActualId,
                TF_FechaRegistro = DateTime.Now
            });

            await _context.SaveChangesAsync();

            TempData["Exito"] = "Asamblea creada correctamente.";
            return RedirectToAction(nameof(Index));
        }


        // ===== EDITAR (GET) =====
        public async Task<IActionResult> Editar(int id)
        {
            var asamblea = await _context.Asambleas
                .FirstOrDefaultAsync(a => a.TN_Id == id);

            if (asamblea == null)
                return NotFound();

            if (asamblea.TN_Estado == EstadoAsambleaEnum.Realizada)
            {
                TempData["Error"] =
                    "No se puede editar una asamblea que ya fue realizada.";

                return RedirectToAction(nameof(Detalle), new { id });
            }

            return View(new AsambleaFormVM
            {
                Id = asamblea.TN_Id,
                Titulo = asamblea.TC_Titulo,
                FechaHora = asamblea.TF_FechaHora,
                Modalidad = asamblea.TN_Modalidad,
                Lugar = asamblea.TC_Lugar,
                Descripcion = asamblea.TC_Descripcion
            });
        }


        // ===== EDITAR (POST) =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(AsambleaFormVM vm)
        {
            var asamblea = await _context.Asambleas
                .FirstOrDefaultAsync(a => a.TN_Id == vm.Id);

            if (asamblea == null)
                return NotFound();

            if (asamblea.TN_Estado == EstadoAsambleaEnum.Realizada)
            {
                TempData["Error"] =
                    "No se puede editar una asamblea que ya fue realizada.";

                return RedirectToAction(nameof(Detalle), new { id = vm.Id });
            }

            ValidarAsamblea(vm, esNueva: false);

            if (!ModelState.IsValid)
                return View(vm);

            asamblea.TC_Titulo = vm.Titulo.Trim();
            asamblea.TF_FechaHora = vm.FechaHora;
            asamblea.TN_Modalidad = vm.Modalidad;
            asamblea.TC_Lugar = vm.Lugar?.Trim();
            asamblea.TC_Descripcion = vm.Descripcion?.Trim();

            await _context.SaveChangesAsync();

            TempData["Exito"] = "Asamblea actualizada correctamente.";
            return RedirectToAction(nameof(Detalle), new { id = vm.Id });
        }


        // ===== DETALLE =====
        public async Task<IActionResult> Detalle(int id)
        {
            var asamblea = await _context.Asambleas
                .FirstOrDefaultAsync(a => a.TN_Id == id);

            if (asamblea == null)
                return NotFound();

            var participantes = await _context.ParticipantesAsamblea
                .Include(p => p.Usuario)
                .Where(p => p.TN_IdAsamblea == id)
                .OrderByDescending(p => p.TB_Confirmado)
                .ThenBy(p => p.Usuario.TC_Nombre)
                .ToListAsync();

            var idsAsociados = await _context.DocumentosAsamblea
                .Where(da => da.TN_IdAsamblea == id)
                .Select(da => da.TN_IdDocumento)
                .ToListAsync();

            var asociados = await _context.Documentos
                .Include(d => d.Categoria)
                .Where(d => idsAsociados.Contains(d.TN_Id))
                .OrderBy(d => d.TC_Nombre)
                .ToListAsync();

            var disponibles = await _context.Documentos
                .Include(d => d.Categoria)
                .Where(d => d.TB_Estado && !idsAsociados.Contains(d.TN_Id))
                .OrderBy(d => d.TC_Nombre)
                .ToListAsync();

            return View(new AsambleaDetalleVM
            {
                Asamblea = asamblea,
                Participantes = participantes,
                DocumentosAsociados = asociados,
                DocumentosDisponibles = disponibles
            });
        }


        // ===== CAMBIAR ESTADO =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, EstadoAsambleaEnum estado)
        {
            var asamblea = await _context.Asambleas
                .FirstOrDefaultAsync(a => a.TN_Id == id);

            if (asamblea == null)
                return NotFound();

            if (asamblea.TN_Estado == estado)
                return RedirectToAction(nameof(Detalle), new { id });

            // Una asamblea realizada ya no cambia de estado
            if (asamblea.TN_Estado == EstadoAsambleaEnum.Realizada)
            {
                TempData["Error"] =
                    "Una asamblea realizada no puede cambiar de estado.";

                return RedirectToAction(nameof(Detalle), new { id });
            }

            // No se puede marcar como realizada antes de su fecha
            if (estado == EstadoAsambleaEnum.Realizada &&
                asamblea.TF_FechaHora > DateTime.Now)
            {
                TempData["Error"] =
                    "No se puede marcar como realizada una asamblea que aún no ocurre.";

                return RedirectToAction(nameof(Detalle), new { id });
            }

            asamblea.TN_Estado = estado;
            await _context.SaveChangesAsync();

            TempData["Exito"] = $"La asamblea se marcó como {estado}.";
            return RedirectToAction(nameof(Detalle), new { id });
        }


        // ===== ASOCIAR DOCUMENTO =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsociarDocumento(int idAsamblea, int idDocumento)
        {
            var existeAsamblea = await _context.Asambleas
                .AnyAsync(a => a.TN_Id == idAsamblea);

            var existeDocumento = await _context.Documentos
                .AnyAsync(d => d.TN_Id == idDocumento);

            if (!existeAsamblea || !existeDocumento)
                return NotFound();

            var yaAsociado = await _context.DocumentosAsamblea
                .AnyAsync(da => da.TN_IdAsamblea == idAsamblea &&
                                da.TN_IdDocumento == idDocumento);

            if (yaAsociado)
            {
                TempData["Error"] = "Ese documento ya está asociado a la asamblea.";
                return RedirectToAction(nameof(Detalle), new { id = idAsamblea });
            }

            _context.DocumentosAsamblea.Add(new DocumentoAsamblea
            {
                TN_IdAsamblea = idAsamblea,
                TN_IdDocumento = idDocumento,
                TF_FechaAsociacion = DateTime.Now
            });

            await _context.SaveChangesAsync();

            TempData["Exito"] = "Documento asociado correctamente.";
            return RedirectToAction(nameof(Detalle), new { id = idAsamblea });
        }


        // ===== QUITAR DOCUMENTO =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuitarDocumento(int idAsamblea, int idDocumento)
        {
            var relacion = await _context.DocumentosAsamblea
                .FirstOrDefaultAsync(da => da.TN_IdAsamblea == idAsamblea &&
                                           da.TN_IdDocumento == idDocumento);

            if (relacion != null)
            {
                // Solo se elimina la relación, nunca el documento
                _context.DocumentosAsamblea.Remove(relacion);
                await _context.SaveChangesAsync();

                TempData["Exito"] = "Documento desvinculado de la asamblea.";
            }

            return RedirectToAction(nameof(Detalle), new { id = idAsamblea });
        }


        // ===== MARCAR ASISTENCIA =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarAsistencia(int idParticipante)
        {
            var participante = await _context.ParticipantesAsamblea
                .FirstOrDefaultAsync(p => p.TN_Id == idParticipante);

            if (participante == null)
                return NotFound();

            participante.TB_Asistio = !participante.TB_Asistio;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Detalle),
                new { id = participante.TN_IdAsamblea });
        }


        // ===== VALIDACIONES =====
        private void ValidarAsamblea(AsambleaFormVM vm, bool esNueva)
        {
            // Al crear, la fecha debe ser futura.
            // Al editar no se exige, porque puede editarse una asamblea ya pasada
            // que todavía está en estado Programada.
            if (esNueva && vm.FechaHora <= DateTime.Now)
                ModelState.AddModelError(nameof(vm.FechaHora),
                    "La fecha de la asamblea debe ser futura.");

            if (string.IsNullOrWhiteSpace(vm.Lugar))
            {
                ModelState.AddModelError(nameof(vm.Lugar),
                    vm.Modalidad == ModalidadAsambleaEnum.Virtual
                        ? "Indique el enlace de la reunión virtual."
                        : "Indique el lugar de la asamblea presencial.");
            }
        }
    }
}