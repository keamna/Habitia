using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models
{
    [Table("THBT_A_Acceso")]
    public class Acceso
    {
        [Key]
        public int TN_IdAcceso { get; set; }


        [Required]
        public int TN_IdAutorizacion { get; set; }


        [Required]
        public DateTime TF_FechaIngreso { get; set; }


        public DateTime? TF_FechaSalida { get; set; }


        [Required]
        public bool TB_Estado { get; set; }


        // Relación
        [ForeignKey(nameof(TN_IdAutorizacion))]
        public Autorizacion Autorizacion { get; set; }
    }
}