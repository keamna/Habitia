using Habitia.Models.Catalogos;

namespace Habitia.Services.Interfaces
{
    public interface ITipoMantenimientoService
    {
        Task<List<TipoMantenimiento>> ObtenerActivosAsync();
        Task<TipoMantenimiento> ObtenerOCrearAsync(int? idExistente, string? nombreNuevo);
        Task<bool> ExisteNombreAsync(string nombre);
        Task<List<TipoMantenimiento>> ObtenerPorPersonalAsync(string idPersonal);
    }
}