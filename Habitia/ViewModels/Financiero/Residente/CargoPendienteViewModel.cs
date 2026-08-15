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

        // --- NUEVO: recargo programado (aviso preventivo mientras el cargo sigue Pendiente) ---
        public bool TB_RecargoProgramado { get; set; }
        public string? TC_FrecuenciaRecargo { get; set; } // "Unico" o "PorDia"
        public decimal? TN_ValorRecargo { get; set; }
        public string? TipoRecargoNombre { get; set; } // "Fijo" o "Porcentaje"

        public string BadgeClass => EstadoCargo switch
        {
            "Pendiente" => "badge-pendiente",
            "Vencido" => "badge-vencido",
            _ => "badge-aplicado"
        };

        // Si el total ya no cuadra con base + IVA, la diferencia es un recargo aplicado por atraso.
        public decimal TN_MontoRecargo => Math.Max(0, TN_MontoTotal - TN_MontoBase - TN_MontoIva);

        // NUEVO: texto amigable para el aviso preventivo, ej. "₡2,500 (único)" o "3% (por día de atraso)"
        public string? TextoRecargoProgramado
        {
            get
            {
                if (!TB_RecargoProgramado || TN_ValorRecargo == null)
                    return null;

                var valorTexto = TipoRecargoNombre == "Porcentaje"
                    ? $"{TN_ValorRecargo.Value:0.##}%"
                    : TN_ValorRecargo.Value.ToString("C");

                var frecuenciaTexto = TC_FrecuenciaRecargo == "PorDia"
                    ? "por cada día de atraso"
                    : "único";

                return $"{valorTexto} {frecuenciaTexto}";
            }
        }
    }
}