namespace Habitia.ViewModels.Reportes
{
    public class ReporteIncidenciasViewModel
    {
        public int TotalIncidencias { get; set; }
        public int TotalPendientes { get; set; }
        public int TotalEnProceso { get; set; }
        public int TotalResueltas { get; set; }

        public Dictionary<string, int> PorResponsabilidad { get; set; } = new();
        public Dictionary<string, int> PorTipoUbicacion { get; set; } = new();

        // Top 5 ubicaciones con más incidencias reportadas
        public List<UbicacionFrecuenteViewModel> UbicacionesMasFrecuentes { get; set; } = new();

        public int TotalMantenimientosRealizados { get; set; }
        public int TotalMantenimientosPendientes { get; set; }
        public Dictionary<string, int> MantenimientosPorTipo { get; set; } = new();
    }

    public class UbicacionFrecuenteViewModel
    {
        public string Ubicacion { get; set; }
        public int CantidadIncidencias { get; set; }
    }
}