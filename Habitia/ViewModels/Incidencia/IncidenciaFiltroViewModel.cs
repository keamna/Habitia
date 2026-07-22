using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels.Incidencias
{
    public class IncidenciaFiltroViewModel : IValidatableObject
    {
        [Display(Name = "Estado")]
        public EstadoIncidenciaEnum? Estado { get; set; }

        [Display(Name = "Responsabilidad")]
        public ResponsabilidadEnum? Responsabilidad { get; set; }

        [Display(Name = "Fecha desde")]
        [DataType(DataType.Date)]
        public DateTime? FechaDesde { get; set; }

        [Display(Name = "Fecha hasta")]
        [DataType(DataType.Date)]
        public DateTime? FechaHasta { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaDesde.HasValue && FechaHasta.HasValue && FechaDesde > FechaHasta)
            {
                yield return new ValidationResult(
                    "La fecha 'desde' no puede ser posterior a la fecha 'hasta'.",
                    new[] { nameof(FechaDesde), nameof(FechaHasta) });
            }
        }
    }
}