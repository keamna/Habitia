// RecargoCreateViewModel.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Habitia.ViewModels.Financiero.Admin
{
    // (opcional): aplicar recargo a un cargo vencido.
    public class RecargoCreateViewModel
    {
        [Required(ErrorMessage = "Debe completar los campos obligatorios")]
        public int TN_IdCargo { get; set; }

        // Datos de solo lectura para contexto en pantalla.
        public string NombreResidente { get; set; }
        public decimal TN_MontoTotal { get; set; }
        public DateTime TF_FechaVencimiento { get; set; }

        [Required(ErrorMessage = "Debe completar los campos obligatorios")]
        public int TN_IdTipoRecargo { get; set; }

        [Required(ErrorMessage = "Debe completar los campos obligatorios")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Debe completar los campos obligatorios")]
        public decimal TN_Valor { get; set; }

        public List<SelectListItem> CargosVencidos { get; set; } = new();
        public List<SelectListItem> TiposRecargo { get; set; } = new();
    }
}