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
        public int Id { get; set; }


        [Required]
        public string IdUsuario { get; set; }


        public int IdCategoria { get; set; }


        [Required]
        [MaxLength(150)]
        public string Titulo { get; set; }


        [MaxLength(1000)]
        public string Descripcion { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }


        public EstadoPublicacionEnum Estado { get; set; }

        public TipoPublicacionEnum Tipo { get; set; }
        public DateTime FechaPublicacion { get; set; }



        // Relaciones

        [ForeignKey(nameof(IdUsuario))]
        public ApplicationUser Usuario { get; set; }


        [ForeignKey(nameof(IdCategoria))]
        public THBT_CAT_CategoriaPublicacion Categoria { get; set; }


        public ICollection<ImagenPublicacion> Imagenes { get; set; }
    }
}