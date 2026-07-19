using Habitia.Data;
using Habitia.Enums;
using Habitia.Services.Interfaces;
using Habitia.ViewModels.Reportes;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Services
{
    public class ReporteService : IReporteService
    {
        private readonly ApplicationDbContext _context;

        public ReporteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReporteIncidenciasViewModel> GenerarReporteAsync(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var incidenciasQuery = _context.Incidencias
                .Include(i => i.Vivienda)
                .Include(i => i.AreaComun)
                .AsQueryable();

            if (fechaDesde.HasValue)
                incidenciasQuery = incidenciasQuery.Where(i => i.FechaRegistro >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                incidenciasQuery = incidenciasQuery.Where(i => i.FechaRegistro <= fechaHasta.Value.Date.AddDays(1).AddTicks(-1));

            var incidencias = await incidenciasQuery.ToListAsync();

            var reporte = new ReporteIncidenciasViewModel
            {
                TotalIncidencias = incidencias.Count,
                TotalPendientes = incidencias.Count(i => i.Estado == EstadoIncidenciaEnum.Pendiente),
                TotalEnProceso = incidencias.Count(i => i.Estado == EstadoIncidenciaEnum.EnProceso),
                TotalResueltas = incidencias.Count(i => i.Estado == EstadoIncidenciaEnum.Resuelta)
            };

            reporte.PorResponsabilidad = incidencias
                .Where(i => i.Responsabilidad != 0)
                .GroupBy(i => i.Responsabilidad.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            reporte.PorTipoUbicacion = incidencias
                .GroupBy(i => i.Tipo == TipoIncidenciaEnum.Vivienda ? "Vivienda" : "Área Común")
                .ToDictionary(g => g.Key, g => g.Count());

            reporte.UbicacionesMasFrecuentes = incidencias
                .GroupBy(i => i.Tipo == TipoIncidenciaEnum.Vivienda
                    ? $"Vivienda: {i.Vivienda?.Numero ?? "N/D"}"
                    : $"Área común: {i.AreaComun?.Nombre ?? "N/D"}")
                .Select(g => new UbicacionFrecuenteViewModel
                {
                    Ubicacion = g.Key,
                    CantidadIncidencias = g.Count()
                })
                .OrderByDescending(x => x.CantidadIncidencias)
                .Take(5)
                .ToList();

            var mantenimientosQuery = _context.Mantenimientos
                .Include(m => m.Tipo)
                .AsQueryable();

            if (fechaDesde.HasValue)
                mantenimientosQuery = mantenimientosQuery.Where(m => m.FechaInicio >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                mantenimientosQuery = mantenimientosQuery.Where(m => m.FechaInicio <= fechaHasta.Value.Date.AddDays(1).AddTicks(-1));

            var mantenimientos = await mantenimientosQuery.ToListAsync();

            reporte.TotalMantenimientosRealizados = mantenimientos.Count(m => m.Estado == EstadoMantenimientoEnum.Completado);
            reporte.TotalMantenimientosPendientes = mantenimientos.Count(m =>
                m.Estado == EstadoMantenimientoEnum.Programado || m.Estado == EstadoMantenimientoEnum.EnProceso);

            reporte.MantenimientosPorTipo = mantenimientos
                .GroupBy(m => m.Tipo?.Nombre ?? "Sin tipo")
                .ToDictionary(g => g.Key, g => g.Count());

            return reporte;
        }
    }
}