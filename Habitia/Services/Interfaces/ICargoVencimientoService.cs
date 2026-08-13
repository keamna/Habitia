// Servicio que revisa vencidos y aplica recargos
namespace Habitia.Services.Interfaces
{
    public interface ICargoVencimientoService
    {
        Task ProcesarCargosVencidosAsync();
    }
}