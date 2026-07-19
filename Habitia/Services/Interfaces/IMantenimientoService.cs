using Habitia.Enums;
using Habitia.Models;
using Habitia.ViewModels.Mantenimiento;

namespace Habitia.Services.Interfaces
{
    public interface IMantenimientoService
    {
        Task<Mantenimiento> ConvertirDesdeIncidenciaAsync(MantenimientoCreateViewModel model);

        Task<List<Mantenimiento>> ObtenerPorPersonalAsignadoAsync(string idPersonal);

        Task<Mantenimiento?> ObtenerPorIdAsync(int id);

        Task ActualizarEstadoAsync(MantenimientoEstadoUpdateViewModel model, string idPersonalQueActualiza);

        Task<List<Mantenimiento>> ObtenerTodosAsync();
    }
}