using System.Security.Claims;
using Habitia.Data;
using Habitia.Enums;
using Habitia.Models.Documentos;
using Habitia.ViewModels.Asamblea;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Residente.Controllers
{
    [Area("Residente")]
    [Authorize(Roles = "Residente")]
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
        public async Task<IActionResult> Index()
        {
            var asambleas = await _context.Asambleas
                .OrderByDescending(a => a.TF_FechaHora)
                .ToListAsync();

            // Próximas: programadas y con fecha futura
            ViewBag.Proximas = asambleas
                .Where(a => a.TN_Estado == EstadoAsambleaEnum.Programada &&
                            a.TF_FechaHora > DateTime.Now)
                .OrderBy(a => a.TF_FechaHora)
                .ToList();

            // Historial: el resto
            ViewBag.Historial = asambleas
                .Where(a => a.TN_Estado != EstadoAsambleaEnum.Programada ||
                            a.TF_FechaHora <= DateTime.Now)
                .ToList();

            // En cuáles ya confirmé
            ViewBag.MisConfirmaciones = await _context.ParticipantesAsamblea
                .Where(p => p.TC_IdUsuario == UsuarioActualId && p.TB_Confirmado)
                .Select(p => p.TN_IdAsamblea)
                .ToListAsync();

            return View(asambleas);
        }


        // ===== DETALLE =====
        public async Task<IActionResult> Detalle(int id)
        {
            var asamblea = await _context.Asambleas
                .FirstOrDefaultAsync(a => a.TN_Id == id);

            if (asamblea == null)
                return NotFound();

            var idsAsociados = await _context.DocumentosAsamblea
                .Where(da => da.TN_IdAsamblea == id)
                .Select(da => da.TN_IdDocumento)
                .ToListAsync();

            // Solo documentos activos
            var documentos = await _context.Documentos
                .Include(d => d.Categoria)
                .Where(d => idsAsociados.Contains(d.TN_Id) && d.TB_Estado)
                .OrderBy(d => d.TC_Nombre)
                .ToListAsync();

            var miParticipacion = await _context.ParticipantesAsamblea
                .FirstOrDefaultAsync(p => p.TN_IdAsamblea == id &&
                                          p.TC_IdUsuario == UsuarioActualId);

            return View(new AsambleaDetalleVM
            {
                Asamblea = asamblea,
                DocumentosAsociados = documentos,
                YoConfirme = miParticipacion?.TB_Confirmado ?? false
            });
        }


        // ===== CONFIRMAR ASISTENCIA =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarAsistencia(int id)
        {
            var asamblea = await _context.Asambleas
                .FirstOrDefaultAsync(a => a.TN_Id == id &&
                                          a.TN_Estado == EstadoAsambleaEnum.Programada);

            if (asamblea == null)
            {
                TempData["Error"] = "Esta asamblea ya no admite confirmaciones.";
                return RedirectToAction(nameof(Index));
            }

            if (asamblea.TF_FechaHora <= DateTime.Now)
            {
                TempData["Error"] = "La asamblea ya inició.";
                return RedirectToAction(nameof(Detalle), new { id });
            }

            var participante = await _context.ParticipantesAsamblea
                .FirstOrDefaultAsync(p => p.TN_IdAsamblea == id &&
                                          p.TC_IdUsuario == UsuarioActualId);

            if (participante == null)
            {
                _context.ParticipantesAsamblea.Add(new ParticipanteAsamblea
                {
                    TN_IdAsamblea = id,
                    TC_IdUsuario = UsuarioActualId,
                    TB_Confirmado = true,
                    TB_Asistio = false,
                    TF_FechaRegistro = DateTime.Now
                });
            }
            else
            {
                participante.TB_Confirmado = true;
            }

            await _context.SaveChangesAsync();

            TempData["Exito"] = "Confirmaste tu asistencia a la asamblea.";
            return RedirectToAction(nameof(Detalle), new { id });
        }


        // ===== CANCELAR CONFIRMACIÓN =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarConfirmacion(int id)
        {
            var asamblea = await _context.Asambleas
                .FirstOrDefaultAsync(a => a.TN_Id == id);

            if (asamblea == null)
                return NotFound();

            if (asamblea.TF_FechaHora <= DateTime.Now)
            {
                TempData["Error"] =
                    "La asamblea ya inició, no se puede cancelar la confirmación.";

                return RedirectToAction(nameof(Detalle), new { id });
            }

            var participante = await _context.ParticipantesAsamblea
                .FirstOrDefaultAsync(p => p.TN_IdAsamblea == id &&
                                          p.TC_IdUsuario == UsuarioActualId);

            if (participante != null)
            {
                participante.TB_Confirmado = false;
                await _context.SaveChangesAsync();

                TempData["Exito"] = "Cancelaste tu confirmación de asistencia.";
            }

            return RedirectToAction(nameof(Detalle), new { id });
        }
    }
}