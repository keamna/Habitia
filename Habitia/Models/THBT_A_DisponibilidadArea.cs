using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models
{
    [Table("THBT_A_DisponibilidadArea")]
    public class DisponibilidadArea
    {
        [Key]
        public int TN_Id { get; set; }


        [Required]
        public int TN_IdAreaComun { get; set; }


        [Required]
        [DataType(DataType.Date)]
        public DateTime TF_Fecha { get; set; }


        [Required]
        public TimeSpan TF_HoraInicio { get; set; }


        [Required]
        public TimeSpan TF_HoraFin { get; set; }


        [Required]
        public bool TB_Estado { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Cantidad inválida")]
        public int TN_Cantidad { get; set; }

        public bool TB_Reservado { get; set; } = false;

        // Relaciones

        [ForeignKey(nameof(TN_IdAreaComun))]
        public AreaComun AreaComun { get; set; }


        public ICollection<Reserva> Reservas { get; set; }
            = new List<Reserva>();
    }
}