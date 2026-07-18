using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models.Catalogos
{
    [Table("THBT_CAT_CategoriaPublicacion")]
    public class THBT_CAT_CategoriaPublicacion
    {
        [Key]
        public int Id { get; set; }


        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }


        public bool Estado { get; set; }



        // Relaciones
        public ICollection<Publicacion> Publicaciones { get; set; }
    }
}