using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models.Acceso
{
    [Table("THBT_A_Vehiculo")]
    public class Vehiculo
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        public int TN_IdVisitante { get; set; }

        [Required]
        [MaxLength(10)]
        public string TC_Placa { get; set; }

        [MaxLength(50)]
        public string? TC_Tipo { get; set; }

        [MaxLength(300)]
        public string? TC_Observaciones { get; set; }

        [Required]
        public bool TB_Estado { get; set; }

        // Relaciones
        [ForeignKey(nameof(TN_IdVisitante))]
        public Visitante Visitante { get; set; }
    }
}