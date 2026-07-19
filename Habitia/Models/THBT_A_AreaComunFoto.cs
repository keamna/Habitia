using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models
{
    [Table("THBT_A_AreaComunFoto")]
    public class AreaComunFoto
    {
        [Key]
        public int TN_Id { get; set; }


        [Required]
        public int TN_IdAreaComun { get; set; }


        [Required]
        [MaxLength(250)]
        public string TC_Url { get; set; }


        [Required]
        public bool TB_EsPrincipal { get; set; }


        [Required]
        [Range(0, 50, ErrorMessage = "El orden debe estar entre 0 y 50")]
        public int TN_Orden { get; set; }


        [Required]
        public bool TB_Estado { get; set; }


        // Relación
        [ForeignKey(nameof(TN_IdAreaComun))]
        public AreaComun AreaComun { get; set; }
    }
}