using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Habitia.ViewModels.Financiero.Admin
{
    public class CargoRecurrenteCreateViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Debe indicar el tipo de cargo")]
        public string TC_TipoCargoTexto { get; set; }

        [Required(ErrorMessage = "Ingrese el monto base")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal TN_MontoBase { get; set; }

        public bool TB_AplicaIva { get; set; }
        public string? TC_Descripcion { get; set; }

        [Required(ErrorMessage = "Seleccione la frecuencia")]
        public string TC_Frecuencia { get; set; }

        public bool TB_AplicarATodos { get; set; }
        public List<string> ResidentesSeleccionados { get; set; } = new();

        public bool TB_RecargoProgramado { get; set; }
        public int? TN_IdTipoRecargo { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El valor del recargo debe ser mayor a 0")]
        public decimal? TN_ValorRecargo { get; set; }

        // NUEVO: "Unico" o "PorDia" — se valida condicionalmente abajo
        public string? TC_FrecuenciaRecargo { get; set; }

        [Required(ErrorMessage = "Indique la fecha de inicio")]
        [DataType(DataType.Date)]
        public DateTime TF_FechaInicio { get; set; } = DateTime.Today;

        public List<SelectListItem> TiposCargo { get; set; } = new();
        public List<SelectListItem> TiposRecargo { get; set; } = new();
        public List<ResidenteBusquedaViewModel> Residentes { get; set; } = new();

        // NUEVO: validación condicional, igual que en CargoCreateViewModel
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TB_AplicarATodos)
            {
                if (ResidentesSeleccionados == null || ResidentesSeleccionados.Count == 0)
                {
                    yield return new ValidationResult(
                        "Debe seleccionar al menos un residente.",
                        new[] { nameof(ResidentesSeleccionados) });
                }
            }

            if (TB_RecargoProgramado)
            {
                if (TN_IdTipoRecargo == null)
                {
                    yield return new ValidationResult(
                        "Debe seleccionar el tipo de recargo.",
                        new[] { nameof(TN_IdTipoRecargo) });
                }

                if (TN_ValorRecargo == null || TN_ValorRecargo <= 0)
                {
                    yield return new ValidationResult(
                        "Debe ingresar un valor de recargo mayor a 0.",
                        new[] { nameof(TN_ValorRecargo) });
                }

                if (string.IsNullOrWhiteSpace(TC_FrecuenciaRecargo)
                    || (TC_FrecuenciaRecargo != "Unico" && TC_FrecuenciaRecargo != "PorDia"))
                {
                    yield return new ValidationResult(
                        "Debe seleccionar la frecuencia del recargo.",
                        new[] { nameof(TC_FrecuenciaRecargo) });
                }
            }
        }
    }
}