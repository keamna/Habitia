using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Publicacion
{
    public class ImagenPublicacionViewModel
    {
        public int Id { get; set; }


        [Required]
        public int IdPublicacion { get; set; }


        [Required]
        [MaxLength(300)]
        public string Archivo { get; set; }


        public DateTime FechaRegistro { get; set; }
    }
}