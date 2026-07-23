using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Enums;
using Habitia.Models.Catalogos;

namespace Habitia.Models
{
    [Table("THBT_A_Publicacion")]
    public class Publicacion
    {
        [Key]
        public int TN_Id { get; set; }


        [Required]
        public string TC_IdUsuario { get; set; }


        public int TN_IdCategoria { get; set; }


        [Required]
        [MaxLength(150)]
        public string TC_Titulo { get; set; }


        [MaxLength(1000)]
        public string TC_Descripcion { get; set; }


        [Column(TypeName = "decimal(10,2)")]
        public decimal TN_Precio { get; set; }


        public EstadoPublicacionEnum TN_Estado { get; set; }


        public TipoPublicacionEnum TN_Tipo { get; set; }


        public DateTime TF_FechaPublicacion { get; set; }


        // ===== Campos Marketplace =====

        [MaxLength(1000)]
        public string? TC_Especificaciones { get; set; }


        [MaxLength(100)]
        public string? TC_Contacto { get; set; }


        public DateTime? TF_FechaServicio { get; set; }


        public TimeSpan? TT_HoraInicioServicio { get; set; }


        public TimeSpan? TT_HoraFinServicio { get; set; }


        // ===== Relaciones =====

        [ForeignKey(nameof(TC_IdUsuario))]
        public ApplicationUser Usuario { get; set; }


        [ForeignKey(nameof(TN_IdCategoria))]
        public THBT_CAT_CategoriaPublicacion Categoria { get; set; }


        [ForeignKey("TN_IdPublicacion")]
        public ICollection<ImagenPublicacion> Imagenes { get; set; }

        public ICollection<ResenaPublicacion> Resenas { get; set; }
    }
}