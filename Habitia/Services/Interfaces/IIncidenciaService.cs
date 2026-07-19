using Habitia.Enums;
using Habitia.Models;
using Habitia.ViewModels.Incidencias;

namespace Habitia.Services.Interfaces
{
    public interface IIncidenciaService
    {
        Task<Incidencia> CrearAsync(IncidenciaCreateViewModel model, string idUsuario, string? imagenUrl);

        Task<List<IncidenciaListItemViewModel>> ObtenerTodasAsync(IncidenciaFiltroViewModel? filtro = null);

        Task<List<IncidenciaListItemViewModel>> ObtenerPorUsuarioAsync(string idUsuario);

        Task<Incidencia?> ObtenerPorIdAsync(int id);

        Task ClasificarAsync(IncidenciaClasificarViewModel model);

        Task CambiarEstadoAsync(int idIncidencia, EstadoIncidenciaEnum nuevoEstado);

        Task<bool> TieneMantenimientoAsociadoAsync(int idIncidencia);

        Task<(Incidencia Incidencia, Mantenimiento? Mantenimiento)> CrearPorAdminAsync(
    IncidenciaAdminCreateViewModel model, string idAdmin, string? imagenUrl);
    }
}