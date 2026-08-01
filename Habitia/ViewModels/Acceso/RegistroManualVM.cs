using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels.Acceso
{
    public class RegistroManualVM
    {
        // Visitante: reutilizar existente o crear nuevo
        public int? IdVisitanteExistente { get; set; }

        [MaxLength(150)]
        public string? NombreVisitante { get; set; }

        public TipoIdentificacionEnum? TipoIdentificacionVisitante { get; set; }

        [MaxLength(20)]
        public string? IdentificacionVisitante { get; set; }

        [MaxLength(20)]
        public string? TelefonoVisitante { get; set; }

        [Required]
        public string IdUsuario { get; set; } // residente que autoriza o recibe

        [Required]
        public int IdVivienda { get; set; }

        [Required(ErrorMessage = "Debe indicar el motivo de la visita")]
        [MaxLength(200)]
        public string Motivo { get; set; }

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