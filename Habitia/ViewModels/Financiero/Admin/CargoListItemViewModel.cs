// CargoListItemViewModel.cs
namespace Habitia.ViewModels.Financiero.Admin
{
    public class CargoListItemViewModel
    {
        public int TN_Id { get; set; }

        // Datos del residente (para el desplegable)
        public string ResidenteId { get; set; }
        public string NombreResidente { get; set; }
        public string IdentificacionResidente { get; set; }
        public string CorreoResidente { get; set; }
        public string TelefonoResidente { get; set; }
        public string TipoCargo { get; set; }

        // Desglose del monto (para el desplegable)
        public decimal TN_MontoBase { get; set; }
        public bool TB_AplicaIva { get; set; }
        public bool TB_RecargoAplicado { get; set; }
        public bool TB_RecargoProgramado { get; set; } // <-- NUEVO
        public decimal TN_MontoIva { get; set; }
        public decimal TN_MontoTotal { get; set; }
        public DateTime TF_FechaEmision { get; set; }
        public DateTime TF_FechaVencimiento { get; set; }
        public string EstadoCargo { get; set; }
    }
}