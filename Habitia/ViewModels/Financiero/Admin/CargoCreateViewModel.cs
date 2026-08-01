// CargoCreateViewModel.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Habitia.ViewModels.Financiero.Admin
{
    // crear cargo.
    public class CargoCreateViewModel
    {
        [Required(ErrorMessage = "Debe completar los campos obligatorios")]
        public int TN_IdTipoCargo { get; set; }

        [Required(ErrorMessage = "Debe completar los campos obligatorios")]
        public string TC_IdResidente { get; set; }

        [Required(ErrorMessage = "Debe completar los campos obligatorios")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Debe completar los campos obligatorios")]
        public decimal TN_MontoBase { get; set; }

        [Required(ErrorMessage = "Debe completar los campos obligatorios")]
        [Range(0, double.MaxValue, ErrorMessage = "Debe completar los campos obligatorios")]
        public decimal TN_MontoIva { get; set; }

        [Required(ErrorMessage = "Debe completar los campos obligatorios")]
        public DateTime TF_FechaVencimiento { get; set; }

        [Required(ErrorMessage = "Debe completar los campos obligatorios")]
        [MaxLength(200)]
        public string TC_Descripcion { get; set; }

        // Listas para llenar los <select> del formulario.
        public List<SelectListItem> TiposCargo { get; set; } = new();
        public List<SelectListItem> Residentes { get; set; } = new();
    }
}