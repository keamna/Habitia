using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models
{
    [Table("THBT_A_AreaComunFoto")]
    public class AreaComunFoto
    {
        [Key]
        public int Id { get; set; }

        public int IdAreaComun { get; set; }

        [Required]
        [MaxLength(250)]
        public string Url { get; set; }

        public bool EsPrincipal { get; set; }

        public int Orden { get; set; }

        public bool Estado { get; set; }

        // Relación
        [ForeignKey(nameof(IdAreaComun))]
        public AreaComun AreaComun { get; set; }
    }
}