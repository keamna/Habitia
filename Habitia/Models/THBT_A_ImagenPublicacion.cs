using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models
{
    [Table("THBT_A_ImagenPublicacion")]
    public class ImagenPublicacion
    {
        [Key]
        public int Id { get; set; }


        public int IdPublicacion { get; set; }


        [Required]
        [MaxLength(300)]
        public string Url { get; set; }



        // Relaciones

        [ForeignKey(nameof(IdPublicacion))]
        public Publicacion Publicacion { get; set; }
    }
}