namespace Habitia.Services.Interfaces
{
    public interface ICargoRecurrenteService
    {
        Task GenerarCargosPendientesAsync();
        Task GenerarCargoInmediatoAsync(int idCargoRecurrente); 
    }
}