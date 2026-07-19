using Habitia.ViewModels.Reportes;

namespace Habitia.Services.Interfaces
{
    public interface IReporteService
    {
        Task<ReporteIncidenciasViewModel> GenerarReporteAsync(DateTime? fechaDesde, DateTime? fechaHasta);
    }
}