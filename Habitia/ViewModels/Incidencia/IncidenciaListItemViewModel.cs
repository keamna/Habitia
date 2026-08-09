using Habitia.Enums;

namespace Habitia.ViewModels.Incidencias
{
    public class IncidenciaListItemViewModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Ubicacion { get; set; }
        public EstadoIncidenciaEnum Estado { get; set; }
        public ResponsabilidadEnum? Responsabilidad { get; set; }
        public PrioridadEnum? Prioridad { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Datos del usuario que reportó la incidencia
        public string ReportanteNombreCompleto { get; set; }
        public string? ReportanteIdentificacion { get; set; }
        public string? ReportanteTelefono { get; set; }
        public string? ReportanteCorreo { get; set; }

        // Resumen embebido de la tarea de mantenimiento, si existe
        public bool TieneMantenimientoAsociado { get; set; }
        public string? MantenimientoTipoNombre { get; set; }
        public EstadoMantenimientoEnum? MantenimientoEstado { get; set; }
        public int? MantenimientoId { get; set; }

        // Datos del personal de mantenimiento asignado
        public string? PersonalAsignadoNombreCompleto { get; set; }
        public string? PersonalAsignadoIdentificacion { get; set; }
        public string? PersonalAsignadoTelefono { get; set; }
        public string? PersonalAsignadoCorreo { get; set; }
        public List<string> PersonalAsignadoTipos { get; set; } = new();
    }
}