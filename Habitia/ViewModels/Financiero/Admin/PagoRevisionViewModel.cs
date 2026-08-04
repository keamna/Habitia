// /ViewModels/Financiero/Admin/PagoRevisionViewModel.cs
namespace Habitia.ViewModels.Financiero.Admin
{
    public class PagoRevisionViewModel
    {
        public int TN_IdPago { get; set; }
        public int TN_IdCargo { get; set; }
        public string NombreResidente { get; set; }
        public string IdentificacionResidente { get; set; }
        public string TipoCargo { get; set; }
        public string? Descripcion { get; set; }
        public decimal TN_MontoTotal { get; set; }
        public string MetodoPago { get; set; }
        public DateTime TF_FechaPago { get; set; }
        public string TC_RutaComprobante { get; set; }
    }
}