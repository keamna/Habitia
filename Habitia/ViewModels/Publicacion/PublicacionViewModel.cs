using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels.Publicacion
{
    public class PublicacionViewModel
    {
        public int Id { get; set; }


        [Required]
        public string IdUsuario { get; set; }


        [Required]
        [MaxLength(100)]
        public string Titulo { get; set; }


        [Required]
        [MaxLength(500)]
        public string Descripcion { get; set; }


        [Required]
        public int IdCategoria { get; set; }


        // Precio opcional para productos o servicios

        public decimal? Precio { get; set; }


        [MaxLength(100)]
        public string Contacto { get; set; }


        public EstadoPublicacionEnum Estado { get; set; }


        public DateTime FechaPublicacion { get; set; }
    }
}