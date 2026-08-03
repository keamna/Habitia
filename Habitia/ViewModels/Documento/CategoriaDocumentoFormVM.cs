using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Documento
{
    public class CategoriaDocumentoFormVM
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(50, ErrorMessage = "Máximo 50 caracteres.")]
        [Display(Name = "Nombre de la categoría")]
        public string Nombre { get; set; }
    }
}