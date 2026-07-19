using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels
{
    public class DisponibilidadFormVM
    {
        [Required]
        public int IdAreaComun { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "Seleccione la hora de inicio")]
        [Range(1, 12, ErrorMessage = "Hora inválida")]
        public int HoraInicioHora { get; set; }

        [Required(ErrorMessage = "Seleccione los minutos de inicio")]
        public int HoraInicioMinuto { get; set; }

        [Required(ErrorMessage = "Seleccione AM o PM")]
        public string HoraInicioAmPm { get; set; }

        [Required(ErrorMessage = "Seleccione la hora de fin")]
        [Range(1, 12, ErrorMessage = "Hora inválida")]
        public int HoraFinHora { get; set; }

        [Required(ErrorMessage = "Seleccione los minutos de fin")]
        public int HoraFinMinuto { get; set; }

        [Required(ErrorMessage = "Seleccione AM o PM")]
        public string HoraFinAmPm { get; set; }
    }
}