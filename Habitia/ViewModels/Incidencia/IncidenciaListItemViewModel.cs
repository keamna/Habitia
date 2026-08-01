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
        public string NombreUsuarioReporta { get; set; }

        // Resumen embebido de la tarea de mantenimiento, si existe
        public bool TieneMantenimientoAsociado { get; set; }
        public string? MantenimientoTipoNombre { get; set; }
        public EstadoMantenimientoEnum? MantenimientoEstado { get; set; }
        public int? MantenimientoId { get; set; }
    }
}