// RecargoCreateViewModel.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Habitia.ViewModels.Financiero.Admin
{
    // Aplicar recargo a un cargo vencido. El cargo puede llegar precargado (modal)
    // o elegirse de una lista (página completa).
    public class RecargoCreateViewModel
    {
        public int TN_IdCargo { get; set; }
        // ===== Datos de solo lectura para la tarjeta de contexto en el modal =====
        public string NombreResidente { get; set; }
        public string IdentificacionResidente { get; set; }
        public string CorreoResidente { get; set; }
        public string TelefonoResidente { get; set; }
        public string TipoCargo { get; set; }
        public string Descripcion { get; set; }
        public decimal TN_MontoBase { get; set; }
        public bool TB_AplicaIva { get; set; }
        public decimal TN_MontoIva { get; set; }
        public decimal TN_MontoTotal { get; set; }
        public DateTime TF_FechaVencimiento { get; set; }
        public int DiasVencido { get; set; }
        // ===== Datos del formulario =====
        [Required(ErrorMessage = "Debe completar los campos obligatorios")]
        public int TN_IdTipoRecargo { get; set; }
        [Required(ErrorMessage = "Debe completar los campos obligatorios")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Debe completar los campos obligatorios")]
        public decimal TN_Valor { get; set; }
        public List<SelectListItem> TiposRecargo { get; set; } = new();
        // ===== Lista de cargos vencidos disponibles (solo para la vista de página completa) =====
        public List<SelectListItem> CargosVencidos { get; set; } = new();
    }
}