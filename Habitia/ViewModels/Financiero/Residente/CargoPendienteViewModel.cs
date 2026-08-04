namespace Habitia.ViewModels.Financiero.Residente
{
    public class CargoPendienteViewModel
    {
        public int TN_Id { get; set; }
        public string TipoCargo { get; set; }
        public string? TC_Descripcion { get; set; }
        public decimal TN_MontoBase { get; set; }
        public decimal TN_MontoIva { get; set; }
        public decimal TN_MontoTotal { get; set; }
        public DateTime TF_FechaEmision { get; set; }
        public DateTime TF_FechaVencimiento { get; set; }
        public string EstadoCargo { get; set; }

        public string BadgeClass => EstadoCargo switch
        {
            "Pendiente" => "badge-pendiente",
            "Vencido" => "badge-vencido",
            _ => "badge-aplicado"
        };
    }
}