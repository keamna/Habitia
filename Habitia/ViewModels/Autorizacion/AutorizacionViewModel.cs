using Habitia.Enums;

namespace Habitia.ViewModels.Autorizacion
{
    public class AutorizacionViewModel
    {
        public int Id { get; set; }

        public string Codigo { get; set; }

        public string NombreVisitante { get; set; }

        public string IdentificacionVisitante { get; set; }

        public DateTime FechaVisita { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string Motivo { get; set; }

        public EstadoAutorizacionEnum Estado { get; set; }

        // Solo lo usa la vista de Admin (invalidar antes de que se use)
        public bool PuedeInvalidar { get; set; }

        // Solo se llena para la vista de Admin (5.3: consulta con datos del residente)
        public string? NombreResidente { get; set; }

        public string? NumeroVivienda { get; set; }
    }
}