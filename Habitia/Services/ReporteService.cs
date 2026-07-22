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
                incidenciasQuery = incidenciasQuery.Where(i => i.TF_FechaRegistro >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                incidenciasQuery = incidenciasQuery.Where(i => i.TF_FechaRegistro <= fechaHasta.Value.Date.AddDays(1).AddTicks(-1));

            var incidencias = await incidenciasQuery.ToListAsync();

            var reporte = new ReporteIncidenciasViewModel
            {
                TotalIncidencias = incidencias.Count,
                TotalPendientes = incidencias.Count(i => i.TN_Estado == EstadoIncidenciaEnum.Pendiente),
                TotalEnProceso = incidencias.Count(i => i.TN_Estado == EstadoIncidenciaEnum.EnProceso),
                TotalResueltas = incidencias.Count(i => i.TN_Estado == EstadoIncidenciaEnum.Resuelta)
            };

            reporte.PorResponsabilidad = incidencias
                .Where(i => i.TN_Responsabilidad != 0)
                .GroupBy(i => i.TN_Responsabilidad.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            reporte.PorTipoUbicacion = incidencias
                .GroupBy(i => i.TN_Tipo == TipoIncidenciaEnum.Vivienda ? "Vivienda" : "Área Común")
                .ToDictionary(g => g.Key, g => g.Count());

            reporte.UbicacionesMasFrecuentes = incidencias
                .GroupBy(i => i.TN_Tipo == TipoIncidenciaEnum.Vivienda
                    ? $"Vivienda: {i.Vivienda?.TC_Numero ?? "N/D"}"
                    : $"Área común: {i.AreaComun?.TC_Nombre ?? "N/D"}")
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
                mantenimientosQuery = mantenimientosQuery.Where(m => m.TF_FechaInicio >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                mantenimientosQuery = mantenimientosQuery.Where(m => m.TF_FechaInicio <= fechaHasta.Value.Date.AddDays(1).AddTicks(-1));

            var mantenimientos = await mantenimientosQuery.ToListAsync();

            reporte.TotalMantenimientosRealizados = mantenimientos.Count(m => m.TN_Estado == EstadoMantenimientoEnum.Completado);
            reporte.TotalMantenimientosPendientes = mantenimientos.Count(m =>
                m.TN_Estado == EstadoMantenimientoEnum.Programado || m.TN_Estado == EstadoMantenimientoEnum.EnProceso);

            reporte.MantenimientosPorTipo = mantenimientos
                .GroupBy(m => m.Tipo?.TC_Nombre ?? "Sin tipo")
                .ToDictionary(g => g.Key, g => g.Count());

            return reporte;
        }
    }
}