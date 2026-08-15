using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Habitia.ViewModels.Financiero.Admin
{
    public class CargoCreateViewModel : IValidatableObject
    {
        // Ya NO lleva [Required] aquí: la validación condicional se hace en Validate()
        public string? TC_IdResidente { get; set; }

        // --- Modo aplicar a todos ---
        public bool TB_AplicarATodos { get; set; }
        public List<string> ResidentesSeleccionados { get; set; } = new();

        [Required(ErrorMessage = "Debe indicar el tipo de cargo")]
        public string TC_TipoCargoTexto { get; set; }

        [Required(ErrorMessage = "Ingrese el monto base")]
        public decimal? TN_MontoBase { get; set; }

        public bool TB_AplicaIva { get; set; }

        [Required(ErrorMessage = "Ingrese la fecha límite de pago")]
        [DataType(DataType.Date)]
        public DateTime? TF_FechaVencimiento { get; set; }

        public string? TC_Descripcion { get; set; }

        // --- NUEVO: recargo programado (se aplica si el cargo vence) ---
        public bool TB_RecargoProgramado { get; set; }
        public int? TN_IdTipoRecargo { get; set; }
        public decimal? TN_ValorRecargo { get; set; }
        // "Unico" o "PorDia"
        public string? TC_FrecuenciaRecargo { get; set; }

        public List<SelectListItem> TiposCargo { get; set; } = new();
        public List<ResidenteBusquedaViewModel> Residentes { get; set; } = new();
        public List<SelectListItem> TiposRecargo { get; set; } = new(); // <-- NUEVO

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // --- validación condicional de residente(s) ---
            if (TB_AplicarATodos)
            {
                if (ResidentesSeleccionados == null || ResidentesSeleccionados.Count == 0)
                {
                    yield return new ValidationResult(
                        "Debe seleccionar al menos un residente.",
                        new[] { nameof(ResidentesSeleccionados) });
                }
            }
            else if (string.IsNullOrWhiteSpace(TC_IdResidente))
            {
                yield return new ValidationResult(
                    "Debe seleccionar un residente.",
                    new[] { nameof(TC_IdResidente) });
            }

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

            // --- NUEVO: validación condicional del recargo programado ---
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

    public class ResidenteBusquedaViewModel
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Identificacion { get; set; }
    }
}