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

            // Regla de negocio: solo incidencias Comunes o Mixtas generan tarea de mantenimiento
            if (incidencia.TN_Responsabilidad == ResponsabilidadEnum.Privado)
            {
                throw new InvalidOperationException(
                    "No se puede generar una tarea de mantenimiento para una incidencia de tipo Privado.");
            }

            // Regla de negocio: trazabilidad 1 a 1 (una incidencia, una tarea como máximo)
            var yaTieneMantenimiento = await _context.Mantenimientos
                .AnyAsync(m => m.TN_IdIncidencia == model.IdIncidencia);

            if (yaTieneMantenimiento)
            {
                throw new InvalidOperationException(
                    "Esta incidencia ya tiene una tarea de mantenimiento asociada.");
            }

            // Resuelve el tipo de mantenimiento (existente o nuevo, sin duplicados)
            var tipoMantenimiento = await _tipoMantenimientoService.ObtenerOCrearAsync(
                model.IdTipoMantenimiento,
                model.NuevoTipoMantenimiento);

            var mantenimiento = new Mantenimiento
            {
                TN_IdIncidencia = incidencia.TN_Id,
                TN_IdTipo = tipoMantenimiento.TN_Id,
                TN_IdAreaComun = incidencia.TN_IdAreaComun,
                TC_IdPersonalAsignado = model.IdPersonalAsignado,
                TC_Descripcion = model.Descripcion.Trim(),
                TF_FechaProgramada = model.FechaProgramada,
                TF_FechaInicio = DateTime.Now,
                TN_Estado = EstadoMantenimientoEnum.Programado
            };

            _context.Mantenimientos.Add(mantenimiento);

            // La incidencia pasa a "En proceso" al generarse la tarea
            incidencia.TN_Estado = EstadoIncidenciaEnum.EnProceso;

            await _context.SaveChangesAsync();

            return mantenimiento;
        }

        public async Task<List<Mantenimiento>> ObtenerPorPersonalAsignadoAsync(string idPersonal)
        {
            return await _context.Mantenimientos
                .Include(m => m.Incidencia)
                .Include(m => m.Tipo)
                .Include(m => m.AreaComun)
                .Where(m => m.TC_IdPersonalAsignado == idPersonal)
                .OrderBy(m => m.TF_FechaProgramada)
                .ToListAsync();
        }

        public async Task<Mantenimiento?> ObtenerPorIdAsync(int id)
        {
            return await _context.Mantenimientos
                .Include(m => m.Incidencia)
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

            // Regla de seguridad: solo el personal asignado puede actualizar su propia tarea
            if (mantenimiento.TC_IdPersonalAsignado != idPersonalQueActualiza)
            {
                throw new UnauthorizedAccessException(
                    "No tiene permiso para actualizar una tarea que no le fue asignada.");
            }

            mantenimiento.TN_Estado = model.Estado;
            mantenimiento.TC_Observaciones = model.Observaciones?.Trim();

            if (model.Estado == EstadoMantenimientoEnum.Completado)
            {
                mantenimiento.TF_FechaFin = DateTime.Now;

                // Al completarse el mantenimiento, la incidencia asociada se marca Resuelta
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
                .Include(m => m.Tipo)
                .Include(m => m.AreaComun)
                .Include(m => m.PersonalAsignado)
                .OrderByDescending(m => m.TF_FechaProgramada)
                .ToListAsync();
        }
    }
}