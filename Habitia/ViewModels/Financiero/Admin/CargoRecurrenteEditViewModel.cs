using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Habitia.ViewModels.Financiero.Admin
{
    public class CargoRecurrenteEditViewModel
    {
        public int TN_Id { get; set; }

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

        // NUEVO: "Unico" o "PorDia"
        public string? TC_FrecuenciaRecargo { get; set; }

        // Solo informativo: no editable, para mostrarlo en la vista
        public DateTime TF_FechaInicio { get; set; }
        public DateTime? TF_UltimaGeneracion { get; set; }

        public List<SelectListItem> TiposCargo { get; set; } = new();
        public List<SelectListItem> TiposRecargo { get; set; } = new();
        public List<ResidenteBusquedaViewModel> Residentes { get; set; } = new();
    }
}