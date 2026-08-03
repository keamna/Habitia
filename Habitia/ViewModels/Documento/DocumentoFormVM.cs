using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Documento
{
    public class DocumentoFormVM
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100, ErrorMessage = "Máximo 100 caracteres.")]
        [Display(Name = "Nombre del documento")]
        public string Nombre { get; set; }


        [MaxLength(300, ErrorMessage = "Máximo 300 caracteres.")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }


        [Required(ErrorMessage = "Seleccione una categoría.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una categoría.")]
        [Display(Name = "Categoría")]
        public int IdCategoria { get; set; }


        [Display(Name = "Archivo")]
        public IFormFile? Archivo { get; set; }


        // Solo se usan en la vista de edición, para mostrar
        // qué archivo hay cargado actualmente.
        public string? ArchivoActual { get; set; }
        public string? NombreOriginalActual { get; set; }
    }
}