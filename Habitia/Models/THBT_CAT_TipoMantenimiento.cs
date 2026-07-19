using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models.Catalogos
{
    [Table("THBT_CAT_TipoMantenimiento")]
    public class TipoMantenimiento
    {
        [Key]
        public int TN_Id { get; set; }


        [Required]
        [MaxLength(50)]
        public string TC_Nombre { get; set; }


        [Required]
        public bool TB_Estado { get; set; }


        // Relaciones

        public ICollection<Mantenimiento> Mantenimientos { get; set; }
            = new List<Mantenimiento>();
    }
}