using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Marketplace
{
    public class ResenaFormVM
    {
        public int IdPublicacion { get; set; }


        [Required(ErrorMessage = "Seleccione una calificación")]
        [Range(1, 5, ErrorMessage = "La calificación debe ser de 1 a 5 estrellas")]
        public int Calificacion { get; set; }


        [MaxLength(500)]
        public string? Comentario { get; set; }
    }
}