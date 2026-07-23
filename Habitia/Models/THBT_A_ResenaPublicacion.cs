using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models
{
    [Table("THBT_A_ResenaPublicacion")]
    public class ResenaPublicacion
    {
        [Key]
        public int TN_Id { get; set; }


        public int TN_IdPublicacion { get; set; }


        [Required]
        public string TC_IdUsuario { get; set; }


        [Range(1, 5)]
        public int TN_Calificacion { get; set; }


        [MaxLength(500)]
        public string? TC_Comentario { get; set; }


        public DateTime TF_FechaResena { get; set; }


        // Relaciones

        [ForeignKey(nameof(TC_IdUsuario))]
        public ApplicationUser Usuario { get; set; }


        [ForeignKey(nameof(TN_IdPublicacion))]
        public Publicacion Publicacion { get; set; }
    }
}