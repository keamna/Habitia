using Habitia.Data;
using Habitia.Enums;
using Habitia.Models;
using Habitia.Services.Interfaces;
using Habitia.ViewModels.Incidencias;
using Habitia.ViewModels.Mantenimiento;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Services
{
    public class IncidenciaService : IIncidenciaService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMantenimientoService _mantenimientoService;

        public IncidenciaService(
            ApplicationDbContext context,
            IMantenimientoService mantenimientoService)
        {
            _context = context;
            _mantenimientoService = mantenimientoService;
        }

        public async Task<Incidencia> CrearAsync(IncidenciaCreateViewModel model, string idUsuario, string? imagenUrl)
        {
            var incidencia = new Incidencia
            {
                IdUsuario = idUsuario,
                Tipo = model.Tipo,
                IdVivienda = model.Tipo == TipoIncidenciaEnum.Vivienda ? model.IdVivienda : null,
                IdAreaComun = model.Tipo == TipoIncidenciaEnum.AreaComun ? model.IdAreaComun : null,
                Titulo = model.Titulo.Trim(),
                Descripcion = model.Descripcion.Trim(),
                ComentarioAdicional = model.ComentarioAdicional?.Trim(),
                ImagenUrl = imagenUrl,
                Estado = EstadoIncidenciaEnum.Pendiente,
                FechaRegistro = DateTime.Now
            };

            _context.Incidencias.Add(incidencia);
            await _context.SaveChangesAsync();

            return incidencia;
        }

        /// <summary>
        /// Flujo exclusivo del Admin: crea la incidencia ya clasificada y, si la
        /// responsabilidad es Común o Mixta, genera la tarea de mantenimiento
        /// en la misma transacción. Si es Privada, solo registra la incidencia.
        /// </summary>
        public async Task<(Incidencia Incidencia, Mantenimiento? Mantenimiento)> CrearPorAdminAsync(
            IncidenciaAdminCreateViewModel model, string idAdmin, string? imagenUrl)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var incidencia = new Incidencia
                {
                    IdUsuario = idAdmin,
                    Tipo = model.Tipo,
                    IdVivienda = model.Tipo == TipoIncidenciaEnum.Vivienda ? model.IdVivienda : null,
                    IdAreaComun = model.Tipo == TipoIncidenciaEnum.AreaComun ? model.IdAreaComun : null,
                    Titulo = model.Titulo.Trim(),
                    Descripcion = model.Descripcion.Trim(),
                    ComentarioAdicional = model.ComentarioAdicional?.Trim(),
                    ImagenUrl = imagenUrl,
                    Responsabilidad = model.Responsabilidad,
                    Estado = EstadoIncidenciaEnum.Pendiente,
                    FechaRegistro = DateTime.Now
                };

                _context.Incidencias.Add(incidencia);
                await _context.SaveChangesAsync();

                Mantenimiento? mantenimiento = null;

                // Regla de negocio: Privado no genera tarea de mantenimiento
                if (model.Responsabilidad != ResponsabilidadEnum.Privado)
                {
                    var mantenimientoModel = new MantenimientoCreateViewModel
                    {
                        IdIncidencia = incidencia.Id,
                        IdTipoMantenimiento = model.IdTipoMantenimiento,
                        NuevoTipoMantenimiento = model.NuevoTipoMantenimiento,
                        IdPersonalAsignado = model.IdPersonalAsignado!,
                        FechaProgramada = model.FechaProgramada!.Value,
                        Descripcion = model.DescripcionTarea!.Trim()
                    };

                    mantenimiento = await _mantenimientoService.ConvertirDesdeIncidenciaAsync(mantenimientoModel);
                }

                await transaction.CommitAsync();
                return (incidencia, mantenimiento);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<IncidenciaListItemViewModel>> ObtenerTodasAsync(IncidenciaFiltroViewModel? filtro = null)
        {
            var query = _context.Incidencias
                .Include(i => i.Usuario)
                .Include(i => i.Vivienda)
                .Include(i => i.AreaComun)
                .AsQueryable();

            if (filtro != null)
            {
                if (filtro.Estado.HasValue)
                    query = query.Where(i => i.Estado == filtro.Estado.Value);

                if (filtro.Responsabilidad.HasValue)
                    query = query.Where(i => i.Responsabilidad == filtro.Responsabilidad.Value);

                if (filtro.FechaDesde.HasValue)
                    query = query.Where(i => i.FechaRegistro >= filtro.FechaDesde.Value.Date);

                if (filtro.FechaHasta.HasValue)
                    query = query.Where(i => i.FechaRegistro <= filtro.FechaHasta.Value.Date.AddDays(1).AddTicks(-1));
            }

            var incidencias = await query
                .OrderByDescending(i => i.FechaRegistro)
                .ToListAsync();

            var idsConMantenimiento = await _context.Mantenimientos
                .Select(m => m.IdIncidencia)
                .ToListAsync();

            return incidencias.Select(i => MapearAListItem(i, idsConMantenimiento)).ToList();
        }

        public async Task<List<IncidenciaListItemViewModel>> ObtenerPorUsuarioAsync(string idUsuario)
        {
            var incidencias = await _context.Incidencias
                .Include(i => i.Usuario)
                .Include(i => i.Vivienda)
                .Include(i => i.AreaComun)
                .Where(i => i.IdUsuario == idUsuario)
                .OrderByDescending(i => i.FechaRegistro)
                .ToListAsync();

            var idsConMantenimiento = await _context.Mantenimientos
                .Select(m => m.IdIncidencia)
                .ToListAsync();

            return incidencias.Select(i => MapearAListItem(i, idsConMantenimiento)).ToList();
        }

        public async Task<Incidencia?> ObtenerPorIdAsync(int id)
        {
            return await _context.Incidencias
                .Include(i => i.Usuario)
                .Include(i => i.Vivienda)
                .Include(i => i.AreaComun)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task ClasificarAsync(IncidenciaClasificarViewModel model)
        {
            var incidencia = await _context.Incidencias.FirstOrDefaultAsync(i => i.Id == model.Id);

            if (incidencia == null)
                throw new InvalidOperationException("La incidencia no existe.");

            incidencia.Responsabilidad = model.Responsabilidad;
            await _context.SaveChangesAsync();
        }

        public async Task CambiarEstadoAsync(int idIncidencia, EstadoIncidenciaEnum nuevoEstado)
        {
            var incidencia = await _context.Incidencias.FirstOrDefaultAsync(i => i.Id == idIncidencia);

            if (incidencia == null)
                throw new InvalidOperationException("La incidencia no existe.");

            incidencia.Estado = nuevoEstado;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TieneMantenimientoAsociadoAsync(int idIncidencia)
        {
            return await _context.Mantenimientos.AnyAsync(m => m.IdIncidencia == idIncidencia);
        }

        private static IncidenciaListItemViewModel MapearAListItem(Incidencia i, List<int> idsConMantenimiento)
        {
            return new IncidenciaListItemViewModel
            {
                Id = i.Id,
                Titulo = i.Titulo,
                Ubicacion = i.Tipo == TipoIncidenciaEnum.Vivienda
                    ? $"Vivienda: {i.Vivienda?.Numero ?? "N/D"}"
                    : $"Área común: {i.AreaComun?.Nombre ?? "N/D"}",
                Estado = i.Estado,
                Responsabilidad = i.Responsabilidad,
                FechaRegistro = i.FechaRegistro,
                NombreUsuarioReporta = i.Usuario?.UserName ?? "N/D",
                TieneMantenimientoAsociado = idsConMantenimiento.Contains(i.Id)
            };
        }
    }
}