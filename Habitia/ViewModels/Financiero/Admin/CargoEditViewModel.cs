using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Habitia.ViewModels.Financiero.Admin
{
    public class CargoEditViewModel : IValidatableObject
    {
        public int TN_Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un residente")]
        public string TC_IdResidente { get; set; }
        public string ResidenteDisplay { get; set; } // para mostrar en el buscador al cargar

        // vivienda actual del cargo (si el residente tiene 2+, se puede cambiar)
        public int? TN_IdVivienda { get; set; }

        [Required(ErrorMessage = "Debe indicar el tipo de cargo")]
        public string TC_TipoCargoTexto { get; set; }

        [Required(ErrorMessage = "Ingrese el monto base")]
        public decimal? TN_MontoBase { get; set; }

        public bool TB_AplicaIva { get; set; }

        [Required(ErrorMessage = "Ingrese la fecha límite de pago")]
        [DataType(DataType.Date)]
        public DateTime? TF_FechaVencimiento { get; set; }

        public string? TC_Descripcion { get; set; }
        public string EstadoActual { get; set; } // solo informativo

        public List<SelectListItem> TiposCargo { get; set; } = new();
        public List<ResidenteBusquedaViewModel> Residentes { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TN_MontoBase.HasValue && TN_MontoBase.Value <= 0)
            {
                yield return new ValidationResult(
                    "El monto base debe ser mayor a 0.",
                    new[] { nameof(TN_MontoBase) });
            }
            if (!TF_FechaVencimiento.HasValue || TF_FechaVencimiento.Value.Year < 2026)
            {
                yield return new ValidationResult(
                    "La fecha límite de pago no es válida.",
                    new[] { nameof(TF_FechaVencimiento) });
            }
            else if (TF_FechaVencimiento.Value.Date < DateTime.Today)
            {
                yield return new ValidationResult(
                    "La fecha límite de pago no puede ser anterior a la fecha actual.",
                    new[] { nameof(TF_FechaVencimiento) });
            }
        }
    }
}