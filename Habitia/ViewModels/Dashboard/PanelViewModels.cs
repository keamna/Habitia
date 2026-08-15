namespace Habitia.ViewModels.Dashboard
{
    // Fila genérica para las listas cortas que muestran los paneles
    // (últimas solicitudes, próximas reservas, visitantes dentro, etc.).
    public class PanelItemViewModel
    {
        public string Titulo { get; set; }
        public string Detalle { get; set; }
        public string Etiqueta { get; set; }

        // Clase de color del badge: success, warning, danger, info, secondary
        public string EtiquetaEstilo { get; set; } = "secondary";

        public DateTime? Fecha { get; set; }
    }


    // Modelo del partial _PanelLista: encabezado + filas + estado vacío.
    public class PanelListaViewModel
    {
        public string Titulo { get; set; }
        public string? VerTodoUrl { get; set; }
        public string TextoVacio { get; set; } = "No hay información para mostrar.";
        public string IconoVacio { get; set; } = "bi-inbox";

        public List<PanelItemViewModel> Items { get; set; } = new();
    }


    // =====================================================
    // PANEL ADMINISTRADOR
    // =====================================================
    public class PanelAdminViewModel
    {
        public string NombreUsuario { get; set; }

        // Usuarios
        public int UsuariosActivos { get; set; }
        public int SolicitudesPendientes { get; set; }

        // Viviendas
        public int TotalViviendas { get; set; }
        public int ViviendasOcupadas { get; set; }
        public int ViviendasDisponibles { get; set; }

        // Áreas comunes y reservas
        public int AreasComunesActivas { get; set; }
        public int ReservasActivas { get; set; }
        public int ReservasHoy { get; set; }

        // Accesos
        public int VisitantesDentro { get; set; }
        public int AccesosHoy { get; set; }

        // Incidencias (solo lectura, la sección la mantiene el otro módulo)
        public int IncidenciasPendientes { get; set; }

        // Listas
        public List<PanelItemViewModel> SolicitudesRecientes { get; set; } = new();
        public List<PanelItemViewModel> ProximasReservas { get; set; } = new();
    }


    // =====================================================
    // PANEL RESIDENTE
    // =====================================================
    public class PanelResidenteViewModel
    {
        public string NombreUsuario { get; set; }

        public int ReservasActivas { get; set; }
        public int AutorizacionesVigentes { get; set; }
        public int IncidenciasAbiertas { get; set; }

        public List<PanelItemViewModel> MisViviendas { get; set; } = new();
        public List<PanelItemViewModel> ProximasReservas { get; set; } = new();
        public List<PanelItemViewModel> ProximasVisitas { get; set; } = new();
    }


    // =====================================================
    // PANEL SEGURIDAD
    // =====================================================
    public class PanelSeguridadViewModel
    {
        public string NombreUsuario { get; set; }

        public int VisitantesDentro { get; set; }
        public int IngresosHoy { get; set; }
        public int SalidasHoy { get; set; }
        public int AutorizacionesVigentesHoy { get; set; }
        public int ReservasHoy { get; set; }

        public List<PanelItemViewModel> DentroAhora { get; set; } = new();
        public List<PanelItemViewModel> AutorizacionesDeHoy { get; set; } = new();
    }


    // =====================================================
    // PANEL MANTENIMIENTO
    // =====================================================
    public class PanelMantenimientoViewModel
    {
        public string NombreUsuario { get; set; }

        public int TareasProgramadas { get; set; }
        public int TareasEnProceso { get; set; }
        public int TareasCompletadasMes { get; set; }

        public List<PanelItemViewModel> TareasPendientes { get; set; } = new();
    }
}