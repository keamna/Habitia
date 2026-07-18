using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models.Catalogos
{
    [Table("THBT_CAT_TipoArea")]
    public class TipoArea
    {
        [Key]
        public int Id { get; set; }


        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }


        public bool Estado { get; set; }



        // Relaciones

        public ICollection<AreaComun> Areas { get; set; }
    }
}