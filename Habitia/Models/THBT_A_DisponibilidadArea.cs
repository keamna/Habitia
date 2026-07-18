using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models
{
    [Table("THBT_A_DisponibilidadArea")]
    public class DisponibilidadArea
    {
        [Key]
        public int Id { get; set; }

        public int IdAreaComun { get; set; }

        public DateTime Fecha { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFin { get; set; }

        public bool Estado { get; set; }

        // Relaciones


        [ForeignKey(nameof(IdAreaComun))]
        public AreaComun Area { get; set; }

        public ICollection<Reserva> Reservas { get; set; }
    }
}