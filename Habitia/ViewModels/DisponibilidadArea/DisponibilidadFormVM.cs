using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels
{
    public class DisponibilidadFormVM
    {
        [Required]
        public int IdAreaComun { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        // ===== Hora de inicio =====
        // Se digitan libremente (no se eligen de una lista fija).

        [Required(ErrorMessage = "Ingrese la hora de inicio")]
        [Range(1, 12, ErrorMessage = "La hora de inicio debe estar entre 1 y 12")]
        public int HoraInicioHora { get; set; }

        [Required(ErrorMessage = "Ingrese los minutos de inicio")]
        [Range(0, 59, ErrorMessage = "Los minutos de inicio deben estar entre 0 y 59")]
        public int HoraInicioMinuto { get; set; }

        [Required(ErrorMessage = "Seleccione AM o PM en la hora de inicio")]
        public string HoraInicioAmPm { get; set; }

        // ===== Hora de fin =====

        [Required(ErrorMessage = "Ingrese la hora de finalización")]
        [Range(1, 12, ErrorMessage = "La hora de finalización debe estar entre 1 y 12")]
        public int HoraFinHora { get; set; }

        [Required(ErrorMessage = "Ingrese los minutos de finalización")]
        [Range(0, 59, ErrorMessage = "Los minutos de finalización deben estar entre 0 y 59")]
        public int HoraFinMinuto { get; set; }

        [Required(ErrorMessage = "Seleccione AM o PM en la hora de finalización")]
        public string HoraFinAmPm { get; set; }

        [Required(ErrorMessage = "Indique la cantidad de personas permitidas")]
        [Range(1, 100, ErrorMessage = "Ingrese una cantidad válida")]
        public int Cantidad { get; set; }

        // ===== Anticipación mínima para cancelar =====
        // Antes se pedía al crear el área común; ahora se define por horario.

        [Required(ErrorMessage = "Ingrese la anticipación mínima (horas)")]
        [Range(0, 168, ErrorMessage = "Las horas de anticipación deben estar entre 0 y 168")]
        public int AnticipacionHoras { get; set; }

        [Required(ErrorMessage = "Ingrese la anticipación mínima (minutos)")]
        [Range(0, 59, ErrorMessage = "Los minutos de anticipación deben estar entre 0 y 59")]
        public int AnticipacionMinutos { get; set; }

        // Total en minutos, que es como se guarda en la base de datos.
        public int AnticipacionEnMinutos => (AnticipacionHoras * 60) + AnticipacionMinutos;
    }
}