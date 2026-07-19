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
        public DateTime FechaRegistro { get; set; }
        public string NombreUsuarioReporta { get; set; }
        public bool TieneMantenimientoAsociado { get; set; }
    }
}