using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Acceso
{
    public class ConfirmarIngresoQRVM
    {
        [Required]
        public int IdAutorizacion { get; set; }

        // Vehículo (opcional)
        public bool IngresaEnVehiculo { get; set; }

        [MaxLength(10)]
        public string? Placa { get; set; }

        [MaxLength(50)]
        public string? TipoVehiculo { get; set; }

        [MaxLength(300)]
        public string? ObservacionesVehiculo { get; set; }
    }
}