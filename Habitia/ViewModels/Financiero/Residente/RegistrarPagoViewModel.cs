// RegistrarPagoViewModel.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Habitia.ViewModels.Financiero.Residente
{
    // registro de pago de un cargo.
    public class RegistrarPagoViewModel
    {
        public int TN_IdCargo { get; set; }

        // Desglose de solo lectura (US-10, punto 2.4).
        public string TC_Descripcion { get; set; }
        public decimal TN_MontoBase { get; set; }
        public decimal TN_MontoIva { get; set; }
        public decimal TN_MontoTotal { get; set; }

        [Required(ErrorMessage = "Debe completar los campos obligatorios")]
        public int TN_IdMetodoPago { get; set; }

        // Se muestran en pantalla solo si el método elegido es Tarjeta o SINPE
        public string TitularTarjeta { get; set; }
        public string IbanTarjeta { get; set; }
        public string TitularSinpe { get; set; }
        public string NumeroSinpe { get; set; }

        [Required(ErrorMessage = "Debe completar los campos obligatorios")]
        public IFormFile Comprobante { get; set; }

        public List<SelectListItem> MetodosPago { get; set; } = new();
    }
}