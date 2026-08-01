using Habitia.Data;
using Habitia.Enums;
using Habitia.Models;
using Habitia.Services.Interfaces;
using Habitia.ViewModels.Incidencias;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Services
{
    public class IncidenciaService : IIncidenciaService
    {
        private readonly ApplicationDbContext _context;

        public IncidenciaService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Usado por Residente, Seguridad y Admin — mismo flujo para los tres roles.
        public async Task<Incidencia> CrearAsync(IncidenciaCreateViewModel model, string idUsuario, string? imagenUrl)
        {
            var incidencia = new Incidencia
            {
                TC_IdUsuario = idUsuario,
                TN_Tipo = model.Tipo,
                TN_IdVivienda = model.Tipo == TipoIncidenciaEnum.Vivienda ? model.IdVivienda : null,
                TN_IdAreaComun = model.Tipo == TipoIncidenciaEnum.AreaComun ? model.IdAreaComun : null,
                TN_Responsabilidad = model.Responsabilidad,
                TC_Titulo = model.Titulo.Trim(),
                TC_Descripcion = model.Descripcion.Trim(),
                TC_ImagenUrl = imagenUrl,
                TN_Estado = EstadoIncidenciaEnum.Pendiente,
                TF_FechaRegistro = DateTime.Now
            };

            _context.Incidencias.Add(incidencia);
            await _context.SaveChangesAsync();

            return incidencia;
        }

        public async Task<List<IncidenciaListItemViewModel>> ObtenerTodasAsync(IncidenciaFiltroViewModel? filtro = null)
        {
            var query = _context.Incidencias
                .Include(i => i.Usuario)
                .Include(i => i.Vivienda)
                .Include(i => i.AreaComun)
                .Include(i => i.Mantenimiento)
                    .ThenInclude(m => m!.Tipo)
                .AsQueryable();

            if (filtro != null)
            {
                if (filtro.Estado.HasValue)
                    query = query.Where(i => i.TN_Estado == filtro.Estado.Value);

                if (filtro.Responsabilidad.HasValue)
                    query = query.Where(i => i.TN_Responsabilidad == filtro.Responsabilidad.Value);

                if (filtro.FechaDesde.HasValue)
                    query = query.Where(i => i.TF_FechaRegistro >= filtro.FechaDesde.Value.Date);

                if (filtro.FechaHasta.HasValue)
                    query = query.Where(i => i.TF_FechaRegistro <= filtro.FechaHasta.Value.Date.AddDays(1).AddTicks(-1));
            }

            var incidencias = await query
                .OrderByDescending(i => i.TF_FechaRegistro)
                .ToListAsync();

            return incidencias.Select(MapearAListItem).ToList();
        }

        public async Task<List<IncidenciaListItemViewModel>> ObtenerPorUsuarioAsync(string idUsuario)
        {
            var incidencias = await _context.Incidencias
                .Include(i => i.Usuario)
                .Include(i => i.Vivienda)
                .Include(i => i.AreaComun)
                .Include(i => i.Mantenimiento)
                    .ThenInclude(m => m!.Tipo)
                .Where(i => i.TC_IdUsuario == idUsuario)
                .OrderByDescending(i => i.TF_FechaRegistro)
                .ToListAsync();

            return incidencias.Select(MapearAListItem).ToList();
        }

        public async Task<Incidencia?> ObtenerPorIdAsync(int id)
        {
            return await _context.Incidencias
                .Include(i => i.Usuario)
                .Include(i => i.Vivienda)
                .Include(i => i.AreaComun)
                .Include(i => i.Mantenimiento)
                    .ThenInclude(m => m!.Tipo)
                .Include(i => i.Mantenimiento)
                    .ThenInclude(m => m!.PersonalAsignado)
                .FirstOrDefaultAsync(i => i.TN_Id == id);
        }

        // Reemplaza a "ClasificarAsync": ya no se elige el tipo de responsabilidad
        // aquí (eso se define al crear la incidencia), solo se asigna la prioridad.
        public async Task AsignarPrioridadAsync(IncidenciaPrioridadViewModel model)
        {
            var incidencia = await _context.Incidencias.FirstOrDefaultAsync(i => i.TN_Id == model.Id);

            if (incidencia == null)
                throw new InvalidOperationException("La incidencia no existe.");

            incidencia.TN_Prioridad = model.Prioridad;
            await _context.SaveChangesAsync();
        }

        public async Task CambiarEstadoAsync(int idIncidencia, EstadoIncidenciaEnum nuevoEstado)
        {
            var incidencia = await _context.Incidencias.FirstOrDefaultAsync(i => i.TN_Id == idIncidencia);

            if (incidencia == null)
                throw new InvalidOperationException("La incidencia no existe.");

            incidencia.TN_Estado = nuevoEstado;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TieneMantenimientoAsociadoAsync(int idIncidencia)
        {
            return await _context.Mantenimientos.AnyAsync(m => m.TN_IdIncidencia == idIncidencia);
        }

        // Cierre de incidencia Privada: puede hacerlo el residente propietario o el Admin.
        public async Task MarcarComoResueltaAsync(int idIncidencia, string idUsuarioQueResuelve, bool esAdmin)
        {
            var incidencia = await _context.Incidencias.FirstOrDefaultAsync(i => i.TN_Id == idIncidencia);

            if (incidencia == null)
                throw new InvalidOperationException("La incidencia no existe.");

            if (incidencia.TN_Responsabilidad != ResponsabilidadEnum.Privado)
                throw new InvalidOperationException("No fue posible completar la operación. Intente nuevamente.");

            if (!esAdmin && incidencia.TC_IdUsuario != idUsuarioQueResuelve)
                throw new UnauthorizedAccessException("No tiene permiso para modificar esta incidencia.");

            incidencia.TN_Estado = EstadoIncidenciaEnum.Resuelta;
            incidencia.TF_FechaResolucion = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task<List<Incidencia>> ObtenerElegiblesParaMantenimientoAsync()
        {
            var idsConMantenimiento = await _context.Mantenimientos
                .Select(m => m.TN_IdIncidencia)
                .ToListAsync();

            return await _context.Incidencias
                .Include(i => i.Vivienda)
                .Include(i => i.AreaComun)
                .Where(i =>
                    (i.TN_Responsabilidad == ResponsabilidadEnum.Comun || i.TN_Responsabilidad == ResponsabilidadEnum.Mixto) &&
                    !idsConMantenimiento.Contains(i.TN_Id))
                .OrderByDescending(i => i.TF_FechaRegistro)
                .ToListAsync();
        }

        private static IncidenciaListItemViewModel MapearAListItem(Incidencia i)
        {
            return new IncidenciaListItemViewModel
            {
                Id = i.TN_Id,
                Titulo = i.TC_Titulo,
                Ubicacion = i.TN_Tipo == TipoIncidenciaEnum.Vivienda
                    ? $"Vivienda: {i.Vivienda?.TC_Numero ?? "N/D"}"
                    : $"Área común: {i.AreaComun?.TC_Nombre ?? "N/D"}",
                Estado = i.TN_Estado,
                Responsabilidad = i.TN_Responsabilidad,
                Prioridad = i.TN_Prioridad,
                FechaRegistro = i.TF_FechaRegistro,
                NombreUsuarioReporta = i.Usuario?.UserName ?? "N/D",
                TieneMantenimientoAsociado = i.Mantenimiento != null,
                MantenimientoTipoNombre = i.Mantenimiento?.Tipo?.TC_Nombre,
                MantenimientoEstado = i.Mantenimiento?.TN_Estado,
                MantenimientoId = i.Mantenimiento?.TN_Id
            };
        }
    }
}