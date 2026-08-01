using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels
{
    public class ReservaFormVM
    {
        [Required]
        public int IdDisponibilidad { get; set; }

        [Required(ErrorMessage = "El motivo de la reserva es obligatorio")]
        [StringLength(250)]
        public string Motivo { get; set; }
    }
}