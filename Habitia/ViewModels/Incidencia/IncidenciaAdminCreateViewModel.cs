using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Habitia.Enums;

namespace Habitia.ViewModels.Incidencias
{
    public class IncidenciaAdminCreateViewModel : IValidatableObject
    {
        // ---------- Datos de la Incidencia ----------
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

        [StringLength(200, ErrorMessage = "El comentario no puede superar los {1} caracteres.")]
        [Display(Name = "Comentario adicional")]
        public string? ComentarioAdicional { get; set; }

        // ---------- Clasificación (el Admin la define al crear) ----------
        [Required(ErrorMessage = "Debe indicar la responsabilidad.")]
        [Display(Name = "Tipo de responsabilidad")]
        public ResponsabilidadEnum Responsabilidad { get; set; }

        // ---------- Datos de la Tarea de Mantenimiento (solo si Común/Mixto) ----------
        [Display(Name = "Tipo de mantenimiento existente")]
        public int? IdTipoMantenimiento { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nuevo tipo debe tener entre {2} y {1} caracteres.")]
        [Display(Name = "Nuevo tipo de mantenimiento")]
        public string? NuevoTipoMantenimiento { get; set; }

        [Display(Name = "Personal asignado")]
        public string? IdPersonalAsignado { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha programada")]
        public DateTime? FechaProgramada { get; set; }

        [StringLength(200, MinimumLength = 5)]
        [Display(Name = "Descripción de la tarea")]
        public string? DescripcionTarea { get; set; }

        // ---------- Listas para los <select> ----------
        public List<SelectListItem>? Viviendas { get; set; }
        public List<SelectListItem>? AreasComunes { get; set; }
        public List<SelectListItem>? TiposMantenimiento { get; set; }
        public List<SelectListItem>? PersonalMantenimiento { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // ---- Ubicación de la incidencia ----
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

                const int maxSizeBytes = 5 * 1024 * 1024;
                if (Evidencia.Length > maxSizeBytes)
                {
                    yield return new ValidationResult(
                        "El tamaño máximo permitido para la evidencia es 5 MB.",
                        new[] { nameof(Evidencia) });
                }
            }

            // ---- Solo se valida la tarea de mantenimiento si NO es Privado ----
            if (Responsabilidad == ResponsabilidadEnum.Privado)
                yield break;

            var tieneTipoExistente = IdTipoMantenimiento.HasValue;
            var tieneTipoNuevo = !string.IsNullOrWhiteSpace(NuevoTipoMantenimiento);

            if (!tieneTipoExistente && !tieneTipoNuevo)
            {
                yield return new ValidationResult(
                    "Debe seleccionar un tipo de mantenimiento existente o ingresar uno nuevo.",
                    new[] { nameof(IdTipoMantenimiento), nameof(NuevoTipoMantenimiento) });
            }

            if (tieneTipoExistente && tieneTipoNuevo)
            {
                yield return new ValidationResult(
                    "No puede seleccionar un tipo existente e ingresar uno nuevo al mismo tiempo.",
                    new[] { nameof(NuevoTipoMantenimiento) });
            }

            if (string.IsNullOrWhiteSpace(IdPersonalAsignado))
            {
                yield return new ValidationResult(
                    "Debe asignar personal de mantenimiento.",
                    new[] { nameof(IdPersonalAsignado) });
            }

            if (!FechaProgramada.HasValue)
            {
                yield return new ValidationResult(
                    "La fecha programada es obligatoria.",
                    new[] { nameof(FechaProgramada) });
            }
            else if (FechaProgramada.Value.Date < DateTime.Today)
            {
                yield return new ValidationResult(
                    "La fecha programada no puede ser anterior al día de hoy.",
                    new[] { nameof(FechaProgramada) });
            }

            if (string.IsNullOrWhiteSpace(DescripcionTarea))
            {
                yield return new ValidationResult(
                    "La descripción de la tarea es obligatoria.",
                    new[] { nameof(DescripcionTarea) });
            }
        }
    }
}