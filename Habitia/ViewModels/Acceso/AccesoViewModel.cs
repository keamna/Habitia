namespace Habitia.ViewModels.Acceso
{
    public class AccesoViewModel
    {
        public int Id { get; set; }

        // ===== Datos del visitante (lo único visible en la fila principal del listado) =====
        public string NombreVisitante { get; set; }

        public string IdentificacionVisitante { get; set; }

        public string Motivo { get; set; }

        public DateTime FechaIngreso { get; set; }

        public DateTime? FechaSalida { get; set; }

        public bool EstaDentro => FechaSalida == null;

        // "Manual" o "Código", según si TN_IdAutorizacion tenía valor
        public string OrigenAcceso { get; set; }

        public string? PlacaVehiculo { get; set; }

        public string? TipoVehiculo { get; set; }

        public string? ObservacionesVehiculo { get; set; }

        public bool TieneVehiculo => !string.IsNullOrWhiteSpace(PlacaVehiculo);

        // ===== Datos del residente (solo visibles al expandir la fila) =====
        public string NombreResidente { get; set; }

        public string NumeroVivienda { get; set; }

        public string? CorreoResidente { get; set; }

        public string? TelefonoResidente { get; set; }

        public string? IdentificacionResidente { get; set; }
    }
}