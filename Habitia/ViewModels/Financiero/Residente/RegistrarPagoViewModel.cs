using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Habitia.ViewModels.Financiero.Residente
{
    // registro de pago de un cargo.
    public class RegistrarPagoViewModel
    {
        public int TN_IdCargo { get; set; }

        // Desglose de solo lectura (US-10, punto 2.4).
        // No vienen como <input> en el formulario, así que no deben validarse en el POST.
        [ValidateNever]
        public string TC_Descripcion { get; set; }
        [ValidateNever]
        public decimal TN_MontoBase { get; set; }
        [ValidateNever]
        public decimal TN_MontoIva { get; set; }
        [ValidateNever]
        public decimal TN_MontoTotal { get; set; }

        // Si el total no cuadra con base + IVA, la diferencia es un recargo por atraso.
        public decimal TN_MontoRecargo => Math.Max(0, TN_MontoTotal - TN_MontoBase - TN_MontoIva);

        // CAMBIO: int? en vez de int — así "no seleccionado" es null (error claro y controlado)
        // en lugar de un fallo de conversión silencioso del model binder.
        [Required(ErrorMessage = "Debe seleccionar un método de pago")]
        public int? TN_IdMetodoPago { get; set; }

        // Se muestran en pantalla solo si el método elegido es Tarjeta o SINPE — no vienen del formulario
        [ValidateNever]
        public string TitularTarjeta { get; set; }
        [ValidateNever]
        public string IbanTarjeta { get; set; }
        [ValidateNever]
        public string TitularSinpe { get; set; }
        [ValidateNever]
        public string NumeroSinpe { get; set; }

        [Required(ErrorMessage = "Debe adjuntar el comprobante de pago")]
        public IFormFile Comprobante { get; set; }

        [ValidateNever]
        public List<SelectListItem> MetodosPago { get; set; } = new();
    }
}