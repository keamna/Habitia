// /ViewModels/Financiero/Admin/HistorialPagoViewModel.cs
namespace Habitia.ViewModels.Financiero.Admin
{
    public class HistorialPagoViewModel
    {
        public string NombreResidente { get; set; }
        public string TipoCargo { get; set; }
        public decimal TN_MontoTotal { get; set; }
        public string MetodoPago { get; set; }
        public bool TB_Aprobado { get; set; }
        public string? TC_MotivoRechazo { get; set; }
        public DateTime TF_FechaCambio { get; set; }
        public string NombreUsuarioCambio { get; set; }
    }
}