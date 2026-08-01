namespace Habitia.ViewModels.Acceso
{
    public class AccesoViewModel
    {
        public int Id { get; set; }

        public string NombreVisitante { get; set; }

        public string IdentificacionVisitante { get; set; }

        public string NombreResidente { get; set; }

        public string NumeroVivienda { get; set; }

        public string Motivo { get; set; }

        public DateTime FechaIngreso { get; set; }

        public DateTime? FechaSalida { get; set; }

        public bool EstaDentro => FechaSalida == null;

        // "Manual" o "QR", según si TN_IdAutorizacion tenía valor
        public string OrigenAcceso { get; set; }

        public string? PlacaVehiculo { get; set; }

        public string? TipoVehiculo { get; set; }

        public string? ObservacionesVehiculo { get; set; }

        public bool TieneVehiculo => !string.IsNullOrWhiteSpace(PlacaVehiculo);
    }
}