using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Enums;

namespace Habitia.Models
{
    [Table("THBT_A_Reserva")]
    public class Reserva
    {
        [Key]
        public int Id { get; set; }


        [Required]
        public string IdUsuario { get; set; }


        public int IdVivienda { get; set; }


        public int IdDisponibilidad { get; set; }


        public int Cantidad { get; set; }


        public EstadoReservaEnum Estado { get; set; }


        public DateTime FechaRegistro { get; set; }


        // Relaciones

        [ForeignKey(nameof(IdUsuario))]
        public ApplicationUser Usuario { get; set; }


        [ForeignKey(nameof(IdVivienda))]
        public Vivienda Vivienda { get; set; }


        [ForeignKey(nameof(IdDisponibilidad))]
        public DisponibilidadArea Disponibilidad { get; set; }
    }
}