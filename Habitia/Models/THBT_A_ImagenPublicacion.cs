using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models
{
    [Table("THBT_A_ImagenPublicacion")]
    public class ImagenPublicacion
    {
        [Key]
        public int TN_Id { get; set; }


        [Required]
        public int TN_IdPublicacion { get; set; }


        [Required]
        [MaxLength(250)]
        public string TC_Url { get; set; }


    }
}