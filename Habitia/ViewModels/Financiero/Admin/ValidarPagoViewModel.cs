// ValidarPagoViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Financiero.Admin
{
    // validación de pagos en revisión.
    public class ValidarPagoViewModel
    {
        public int TN_IdPago { get; set; }
        public int TN_IdCargo { get; set; }

        // Datos de solo lectura para mostrar al admin antes de decidir.
        public string NombreResidente { get; set; }
        public decimal TN_MontoTotal { get; set; }
        public string MetodoPago { get; set; }
        public string TC_RutaComprobante { get; set; }
        public DateTime TF_FechaPago { get; set; }

        // true = aprobar, false = rechazar 
        [Required(ErrorMessage = "Debe completar los campos obligatorios")]
        public bool Aprobar { get; set; }

        // Obligatorio solo si Aprobar = false 
        [MaxLength(300)]
        public string TC_MotivoRechazo { get; set; }
    }
}