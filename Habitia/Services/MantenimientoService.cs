using Habitia.Data;
using Habitia.Enums;
using Habitia.Models;
using Habitia.Services.Interfaces;
using Habitia.ViewModels.Mantenimiento;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Services
{
    public class MantenimientoService : IMantenimientoService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITipoMantenimientoService _tipoMantenimientoService;

        public MantenimientoService(
            ApplicationDbContext context,
            ITipoMantenimientoService tipoMantenimientoService)
        {
            _context = context;
            _tipoMantenimientoService = tipoMantenimientoService;
        }

        public async Task<Mantenimiento> ConvertirDesdeIncidenciaAsync(MantenimientoCreateViewModel model)
        {
            var incidencia = await _context.Incidencias
                .FirstOrDefaultAsync(i => i.TN_Id == model.IdIncidencia);

            if (incidencia == null)
                throw new InvalidOperationException("La incidencia no existe.");

            if (incidencia.TN_Responsabilidad == ResponsabilidadEnum.Privado)
            {
                throw new InvalidOperationException(
                    "Las incidencias privadas no requieren tarea de mantenimiento.");
            }

            var yaTieneMantenimiento = await _context.Mantenimientos
                .AnyAsync(m => m.TN_IdIncidencia == model.IdIncidencia);

            if (yaTieneMantenimiento)
            {
                throw new InvalidOperationException(
                    "Esta incidencia ya tiene una tarea de mantenimiento asociada.");
            }

            var tipoMantenimiento = await _tipoMantenimientoService.ObtenerOCrearAsync(
                model.IdTipoMantenimiento,
                model.NuevoTipoMantenimiento);

            // La fecha ya no la ingresa el Admin: se registra automáticamente al crear la tarea.
            var mantenimiento = new Mantenimiento
            {
                TN_IdIncidencia = incidencia.TN_Id,
                TN_IdTipo = tipoMantenimiento.TN_Id,
                TN_IdAreaComun = incidencia.TN_IdAreaComun,
                TC_IdPersonalAsignado = model.IdPersonalAsignado,
                TC_Descripcion = model.Descripcion.Trim(),
                TF_FechaRegistro = DateTime.Now,
                TN_Estado = EstadoMantenimientoEnum.Programado
            };

            _context.Mantenimientos.Add(mantenimiento);

            incidencia.TN_Estado = EstadoIncidenciaEnum.EnProceso;

            await _context.SaveChangesAsync();

            return mantenimiento;
        }

        public async Task<List<Mantenimiento>> ObtenerPorPersonalAsignadoAsync(string idPersonal)
        {
            return await _context.Mantenimientos
                .Include(m => m.Incidencia)
                    .ThenInclude(i => i.Vivienda)
                .Include(m => m.Incidencia)
                    .ThenInclude(i => i.AreaComun)
                .Include(m => m.Tipo)
                .Include(m => m.AreaComun)
                .Where(m => m.TC_IdPersonalAsignado == idPersonal)
                .OrderByDescending(m => m.TF_FechaRegistro)
                .ToListAsync();
        }

        public async Task<Mantenimiento?> ObtenerPorIdAsync(int id)
        {
            return await _context.Mantenimientos
                .Include(m => m.Incidencia)
                    .ThenInclude(i => i.Vivienda)
                .Include(m => m.Incidencia)
                    .ThenInclude(i => i.AreaComun)
                .Include(m => m.Tipo)
                .Include(m => m.AreaComun)
                .Include(m => m.PersonalAsignado)
                .FirstOrDefaultAsync(m => m.TN_Id == id);
        }

        public async Task ActualizarEstadoAsync(MantenimientoEstadoUpdateViewModel model, string idPersonalQueActualiza)
        {
            var mantenimiento = await _context.Mantenimientos
                .Include(m => m.Incidencia)
                .FirstOrDefaultAsync(m => m.TN_Id == model.Id);

            if (mantenimiento == null)
                throw new InvalidOperationException("La tarea de mantenimiento no existe.");

            if (mantenimiento.TC_IdPersonalAsignado != idPersonalQueActualiza)
            {
                throw new UnauthorizedAccessException(
                    "No tiene permiso para actualizar una tarea que no le fue asignada.");
            }

            // Regla nueva: una tarea Completada queda bloqueada, no se puede modificar más.
            if (mantenimiento.TN_Estado == EstadoMantenimientoEnum.Completado)
            {
                throw new InvalidOperationException(
                    "No es posible modificar una tarea que ya fue completada.");
            }

            mantenimiento.TN_Estado = model.Estado;
            mantenimiento.TC_Observaciones = model.Observaciones?.Trim();

            if (model.Estado == EstadoMantenimientoEnum.Completado)
            {
                mantenimiento.TF_FechaFin = DateTime.Now;

                if (mantenimiento.Incidencia != null)
                {
                    mantenimiento.Incidencia.TN_Estado = EstadoIncidenciaEnum.Resuelta;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Mantenimiento>> ObtenerTodosAsync()
        {
            return await _context.Mantenimientos
                .Include(m => m.Incidencia)
                    .ThenInclude(i => i.Vivienda)
                .Include(m => m.Incidencia)
                    .ThenInclude(i => i.AreaComun)
                .Include(m => m.Tipo)
                .Include(m => m.AreaComun)
                .Include(m => m.PersonalAsignado)
                .OrderByDescending(m => m.TF_FechaRegistro)
                .ToListAsync();
        }
    }
}