using Habitia.Enums;

namespace Habitia.ViewModels.Autorizacion
{
    public class AutorizacionViewModel
    {
        public int Id { get; set; }

        public string Codigo { get; set; }

        // ===== Datos del visitante (lo único visible en la fila principal del listado) =====
        public string NombreVisitante { get; set; }

        public string IdentificacionVisitante { get; set; }

        public DateTime FechaVisita { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string Motivo { get; set; }

        public EstadoAutorizacionEnum Estado { get; set; }

        // Solo lo usa la vista de Admin (invalidar antes de que se use)
        public bool PuedeInvalidar { get; set; }

        // ===== Datos del residente (solo visibles al expandir la fila) =====
        public string? NombreResidente { get; set; }

        public string? NumeroVivienda { get; set; }

        public string? CorreoResidente { get; set; }

        public string? TelefonoResidente { get; set; }

        public string? IdentificacionResidente { get; set; }

        // Texto tipo "Vence en 3 horas" / "Vencido", calculado sobre FechaVencimiento.
        // Solo tiene sentido mientras la autorización sigue Pendiente.
        public string? TextoVencimiento
        {
            get
            {
                if (Estado != EstadoAutorizacionEnum.Pendiente)
                    return null;

                var restante = FechaVencimiento - DateTime.Now;

                if (restante <= TimeSpan.Zero)
                    return "Vencido";

                var horas = (int)Math.Ceiling(restante.TotalHours);
                return horas <= 1 ? "Vence en menos de 1 hora" : $"Vence en {horas} horas";
            }
        }
    }
}