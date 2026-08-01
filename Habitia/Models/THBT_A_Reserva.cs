using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Enums;

namespace Habitia.Models
{
    [Table("THBT_A_Reserva")]
    public class Reserva
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        public string TC_IdUsuario { get; set; }

        [Required]
        public int TN_IdVivienda { get; set; }

        [Required]
        public int TN_IdDisponibilidad { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Cantidad inválida")]
        public int TN_Cantidad { get; set; }

        [Required]
        public EstadoReservaEnum TN_Estado { get; set; }

        [Required]
        public DateTime TF_FechaRegistro { get; set; }

        [Required]
        [StringLength(200)]
        public string TC_Motivo { get; set; }

        public DateTime? TF_FechaCancelacion { get; set; }

        // Relaciones

        [ForeignKey(nameof(TC_IdUsuario))]
        public ApplicationUser Usuario { get; set; }

        [ForeignKey(nameof(TN_IdVivienda))]
        public Vivienda Vivienda { get; set; }

        [ForeignKey(nameof(TN_IdDisponibilidad))]
        public DisponibilidadArea Disponibilidad { get; set; }
    }
}