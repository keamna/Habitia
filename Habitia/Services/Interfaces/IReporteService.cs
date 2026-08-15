using Habitia.ViewModels.Reportes;

namespace Habitia.Services.Interfaces
{
    public interface IReporteService
    {
        Task<ReporteIncidenciasViewModel> GenerarReporteAsync(DateTime? fechaDesde, DateTime? fechaHasta);

        // Reporte completo: usuarios, viviendas, áreas comunes, accesos e incidencias.
        Task<ReporteGeneralViewModel> GenerarReporteGeneralAsync(DateTime? fechaDesde, DateTime? fechaHasta);
    }
}