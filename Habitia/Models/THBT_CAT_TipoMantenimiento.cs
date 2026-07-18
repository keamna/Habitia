using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models.Catalogos
{
    [Table("THBT_CAT_TipoMantenimiento")]
    public class TipoMantenimiento
    {
        [Key]
        public int Id { get; set; }


        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }


        public bool Estado { get; set; }



        public ICollection<Mantenimiento> Mantenimientos { get; set; }
    }
}