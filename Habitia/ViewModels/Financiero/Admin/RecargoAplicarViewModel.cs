using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Habitia.ViewModels.Financiero.Admin
{
    public class RecargoAplicarViewModel
    {
        public int TN_IdCargo { get; set; }
        public string ResidenteDisplay { get; set; }
        public string TipoCargo { get; set; }
        public decimal MontoTotalActual { get; set; }

        [Required(ErrorMessage = "Seleccione el tipo de recargo")]
        public int? TN_IdTipoRecargo { get; set; }

        [Required(ErrorMessage = "Ingrese el valor del recargo")]
        public decimal? TN_Valor { get; set; }

        public List<SelectListItem> TiposRecargo { get; set; } = new();
    }
}