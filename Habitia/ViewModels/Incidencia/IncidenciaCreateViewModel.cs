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

        // Ya no lo elige el usuario: el servicio lo calcula a partir de Responsabilidad
        // (Privado -> Vivienda, Comun -> AreaComun, Mixto -> Mixto).
        public TipoIncidenciaEnum Tipo { get; set; }

        [Display(Name = "Vivienda")]
        public int? IdVivienda { get; set; }

        [Display(Name = "Área común")]
        public int? IdAreaComun { get; set; }

        [Required(ErrorMessage = "Debe indicar el tipo de responsabilidad.")]
        [Display(Name = "Tipo de responsabilidad")]
        public ResponsabilidadEnum? Responsabilidad { get; set; }

        [Display(Name = "Evidencia (imagen)")]
        public IFormFile? Evidencia { get; set; }

        // Solo para poblar los <select> en la vista; no se validan
        public List<SelectListItem>? Viviendas { get; set; }
        public List<SelectListItem>? AreasComunes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            switch (Responsabilidad)
            {
                case ResponsabilidadEnum.Privado:
                    if (IdVivienda == null)
                    {
                        yield return new ValidationResult(
                            "Debe seleccionar la vivienda.",
                            new[] { nameof(IdVivienda) });
                    }
                    break;

                case ResponsabilidadEnum.Comun:
                    if (IdAreaComun == null)
                    {
                        yield return new ValidationResult(
                            "Debe seleccionar el área común.",
                            new[] { nameof(IdAreaComun) });
                    }
                    break;

                case ResponsabilidadEnum.Mixto:
                    if (IdVivienda == null)
                    {
                        yield return new ValidationResult(
                            "Debe seleccionar la vivienda.",
                            new[] { nameof(IdVivienda) });
                    }
                    if (IdAreaComun == null)
                    {
                        yield return new ValidationResult(
                            "Debe seleccionar el área común.",
                            new[] { nameof(IdAreaComun) });
                    }
                    break;
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

                const int maxSizeBytes = 5 * 1024 * 1024;
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