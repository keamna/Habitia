using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Habitia.Enums;

namespace Habitia.ViewModels.Incidencias
{
    public class IncidenciaCreateViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "El título debe tener entre {2} y {1} caracteres.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre {2} y {1} caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Debe indicar el tipo de ubicación.")]
        [Display(Name = "Tipo de ubicación")]
        public TipoIncidenciaEnum Tipo { get; set; }

        [Display(Name = "Vivienda")]
        public int? IdVivienda { get; set; }

        [Display(Name = "Área común")]
        public int? IdAreaComun { get; set; }

        [Display(Name = "Evidencia (imagen)")]
        public IFormFile? Evidencia { get; set; }

        [StringLength(500, ErrorMessage = "El comentario no puede superar los {1} caracteres.")]
        [Display(Name = "Comentario adicional")]
        public string? ComentarioAdicional { get; set; }

        // Solo para poblar los <select> en la vista; no se validan
        public List<SelectListItem>? Viviendas { get; set; }
        public List<SelectListItem>? AreasComunes { get; set; }

        // Validaciones cruzadas: dependen de más de una propiedad,
        // por eso no se pueden resolver con DataAnnotations simples.
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Tipo == TipoIncidenciaEnum.Vivienda && IdVivienda == null)
            {
                yield return new ValidationResult(
                    "Debe seleccionar la vivienda asociada a la incidencia.",
                    new[] { nameof(IdVivienda) });
            }

            if (Tipo == TipoIncidenciaEnum.AreaComun && IdAreaComun == null)
            {
                yield return new ValidationResult(
                    "Debe seleccionar el área común asociada a la incidencia.",
                    new[] { nameof(IdAreaComun) });
            }

            if (Evidencia != null)
            {
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(Evidencia.FileName).ToLowerInvariant();

                if (!extensionesPermitidas.Contains(extension))
                {
                    yield return new ValidationResult(
                        "Solo se permiten imágenes en formato JPG o PNG.",
                        new[] { nameof(Evidencia) });
                }

                const int maxSizeBytes = 5 * 1024 * 1024; // 5 MB
                if (Evidencia.Length > maxSizeBytes)
                {
                    yield return new ValidationResult(
                        "El tamaño máximo permitido para la evidencia es 5 MB.",
                        new[] { nameof(Evidencia) });
                }
            }
        }
    }
}