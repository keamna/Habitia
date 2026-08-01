using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels.Reserva
{
    public class ReservaViewModel
    {
        public int Id { get; set; }

        [Required]
        public string IdUsuario { get; set; }

        [Required]
        public int IdVivienda { get; set; }

        [Required]
        public int IdDisponibilidad { get; set; }

        [Required]
        [Range(1, 100)]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El motivo de la reserva es obligatorio")]
        [StringLength(250)]
        public string Motivo { get; set; }

        public EstadoReservaEnum Estado { get; set; }

        public DateTime FechaRegistro { get; set; }

        public DateTime? FechaCancelacion { get; set; }

        // ===== Datos del horario reservado (para mostrar en listados) =====
        public string NombreAreaComun { get; set; }

        public DateTime FechaHorario { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFin { get; set; }

        // ===== Fotos del área común reservada (para ver en grande / carrusel) =====
        public List<string> FotosAreaComun { get; set; } = new();

        // ===== Datos del residente (solo se llenan en la vista Admin/Seguridad) =====
        public string NombreResidente { get; set; }

        public string NumeroVivienda { get; set; }

        // Detalle extendido del residente, para el panel expandible en Admin/Seguridad
        public string CorreoResidente { get; set; }

        public string TelefonoResidente { get; set; }

        public string IdentificacionResidente { get; set; }

        public string TipoVivienda { get; set; }

        // ===== Bandera para la vista: controla si se muestra el botón "Cancelar" =====
        public bool PuedeCancelar { get; set; }
    }
}