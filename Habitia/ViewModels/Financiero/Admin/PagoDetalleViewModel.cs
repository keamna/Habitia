namespace Habitia.ViewModels.Financiero.Admin
{
    public class PagoDetalleViewModel
    {
        public int TN_IdPago { get; set; }
        public int TN_IdCargo { get; set; }

        public string NombreResidente { get; set; }
        public string IdentificacionResidente { get; set; }
        public string? CorreoResidente { get; set; }
        public string? TelefonoResidente { get; set; }

        public string TipoCargo { get; set; }
        public string? Descripcion { get; set; }
        public decimal TN_MontoBase { get; set; }
        public bool TB_AplicaIva { get; set; }
        public decimal TN_MontoIva { get; set; }
        public decimal TN_MontoTotal { get; set; }

        // recargo ya aplicado a este cargo (la diferencia entre el total y base+IVA)
        public decimal TN_MontoRecargo => Math.Max(0, TN_MontoTotal - TN_MontoBase - TN_MontoIva);

        public DateTime TF_FechaEmision { get; set; }
        public DateTime TF_FechaVencimiento { get; set; }

        public string MetodoPago { get; set; }
        public DateTime TF_FechaPago { get; set; }
        public string? TC_RutaComprobante { get; set; }

        public string EstadoCargo { get; set; }

        // NUEVO: vivienda asociada al cargo, si tiene
        public string? NumeroVivienda { get; set; }
    }
}