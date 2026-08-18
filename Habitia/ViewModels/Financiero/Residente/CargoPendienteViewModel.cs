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
        public string? NumeroVivienda { get; set; } // NUEVO

        // Motivo del último pago rechazado, si lo hubo.
        // OJO: "Rechazado" NO es un estado del cargo. Cuando el administrador
        // rechaza un comprobante, el cargo vuelve a "Pendiente" y el motivo
        // queda en el pago. Acá se deriva para poder mostrarlo y filtrarlo.
        public string? MotivoRechazo { get; set; }

        public bool FuePagoRechazado =>
            !string.IsNullOrWhiteSpace(MotivoRechazo) &&
            (EstadoCargo == "Pendiente" || EstadoCargo == "Vencido");

        // Lo que se muestra al residente y por lo que se filtra en la pantalla.
        public string EstadoVisible => FuePagoRechazado ? "Rechazado" : EstadoCargo;

        public string BadgeClass => EstadoVisible switch
        {
            "Pendiente" => "badge-pendiente",
            "Vencido" => "badge-vencido",
            "En revisión" => "badge-revision",
            "Pagado" => "badge-pagado",
            "Rechazado" => "badge-rechazado",
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