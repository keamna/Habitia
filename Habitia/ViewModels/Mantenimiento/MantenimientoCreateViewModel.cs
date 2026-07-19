using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Habitia.ViewModels.Mantenimiento
{
    public class MantenimientoCreateViewModel : IValidatableObject
    {
        [Required]
        public int IdIncidencia { get; set; }

        [Display(Name = "Tipo de mantenimiento existente")]
        public int? IdTipoMantenimiento { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nuevo tipo debe tener entre {2} y {1} caracteres.")]
        [Display(Name = "Nuevo tipo de mantenimiento")]
        public string? NuevoTipoMantenimiento { get; set; }

        [Required(ErrorMessage = "Debe asignar personal de mantenimiento.")]
        [Display(Name = "Personal asignado")]
        public string IdPersonalAsignado { get; set; }

        [Required(ErrorMessage = "La fecha programada es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha programada")]
        public DateTime FechaProgramada { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(200, MinimumLength = 5)]
        [Display(Name = "Descripción de la tarea")]
        public string Descripcion { get; set; }

        public List<SelectListItem>? TiposMantenimiento { get; set; }
        public List<SelectListItem>? PersonalMantenimiento { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
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

            if (FechaProgramada.Date < DateTime.Today)
            {
                yield return new ValidationResult(
                    "La fecha programada no puede ser anterior al día de hoy.",
                    new[] { nameof(FechaProgramada) });
            }
        }
    }
}