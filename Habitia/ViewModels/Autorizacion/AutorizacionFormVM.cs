using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels.Autorizacion
{
    public class AutorizacionFormVM
    {
        // Si el residente reutiliza un visitante ya registrado, viene con valor.
        // Si es null, se crea un visitante nuevo con los campos de abajo.
        public int? IdVisitanteExistente { get; set; }

        [MaxLength(150)]
        public string? NombreVisitante { get; set; }

        public TipoIdentificacionEnum? TipoIdentificacionVisitante { get; set; }

        [MaxLength(20)]
        public string? IdentificacionVisitante { get; set; }

        [MaxLength(20)]
        public string? TelefonoVisitante { get; set; }

        [Required(ErrorMessage = "Debe indicar la fecha de la visita")]
        [DataType(DataType.Date)]
        public DateTime FechaVisita { get; set; }

        [Required(ErrorMessage = "Debe indicar el motivo de la visita")]
        [MaxLength(200)]
        public string Motivo { get; set; }
    }
}