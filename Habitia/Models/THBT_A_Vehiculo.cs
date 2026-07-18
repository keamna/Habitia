using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models
{
    [Table("THBT_A_Vehiculo")]
    public class Vehiculo
    {
        [Key]
        public int Id { get; set; }


        public int IdVisitante { get; set; }


        [Required]
        [MaxLength(20)]
        public string Placa { get; set; }


        [MaxLength(50)]
        public string Tipo { get; set; }


        [MaxLength(200)]
        public string Observaciones { get; set; }



        // Relaciones

        [ForeignKey(nameof(IdVisitante))]
        public Visitante Visitante { get; set; }
    }
}