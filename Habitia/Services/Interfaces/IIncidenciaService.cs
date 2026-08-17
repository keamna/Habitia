using Habitia.Enums;
using Habitia.Models;
using Habitia.ViewModels.Incidencias;

namespace Habitia.Services.Interfaces
{
    public interface IIncidenciaService
    {
        Task<Incidencia> CrearAsync(IncidenciaCreateViewModel model, string idUsuario, string? imagenUrl);
        Task<List<IncidenciaListItemViewModel>> ObtenerTodasAsync(IncidenciaFiltroViewModel? filtro = null);
        Task<List<IncidenciaListItemViewModel>> ObtenerPorUsuarioAsync(string idUsuario, IncidenciaFiltroViewModel? filtro = null);
        Task<Incidencia?> ObtenerPorIdAsync(int id);
        Task AsignarPrioridadAsync(IncidenciaPrioridadViewModel model);
        Task CambiarEstadoAsync(int idIncidencia, EstadoIncidenciaEnum nuevoEstado);
        Task<bool> TieneMantenimientoAsociadoAsync(int idIncidencia);
        Task MarcarComoResueltaAsync(int idIncidencia, string idUsuarioQueResuelve, bool esAdmin);
        Task<List<Incidencia>> ObtenerElegiblesParaMantenimientoAsync();
    }
}