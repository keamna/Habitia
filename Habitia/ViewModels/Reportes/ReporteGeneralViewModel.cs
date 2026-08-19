namespace Habitia.ViewModels.Reportes
{
    // Fila genérica "etiqueta / cantidad" para los desgloses de los reportes.
    public class ConteoReporteViewModel
    {
        public string Etiqueta { get; set; }
        public int Cantidad { get; set; }
    }


    // =====================================================
    // USUARIOS
    // =====================================================
    public class ReporteUsuariosViewModel
    {
        public int Total { get; set; }
        public int Activos { get; set; }
        public int Pendientes { get; set; }
        public int Suspendidos { get; set; }
        public int Rechazados { get; set; }

        public List<ConteoReporteViewModel> PorRol { get; set; } = new();
        public List<ConteoReporteViewModel> PorTipoRelacion { get; set; } = new();

        // Altas dentro del rango de fechas consultado
        public int RegistradosEnRango { get; set; }

        // Altas mes a mes (últimos 6 meses) para el gráfico de tendencia
        public List<ConteoReporteViewModel> RegistrosPorMes { get; set; } = new();
    }


    // =====================================================
    // VIVIENDAS
    // =====================================================
    public class ReporteViviendasViewModel
    {
        public int Total { get; set; }
        public int Ocupadas { get; set; }
        public int Disponibles { get; set; }
        public int Inactivas { get; set; }

        public int SinPropietario { get; set; }
        public int SinCupoConfigurado { get; set; }

        public List<ConteoReporteViewModel> PorTipo { get; set; } = new();

        // Porcentaje de viviendas ocupadas sobre el total
        public double PorcentajeOcupacion { get; set; }

        // Ocupación comparada por tipo de vivienda (para el gráfico)
        public List<ConteoReporteViewModel> OcupadasPorTipo { get; set; } = new();
        public List<ConteoReporteViewModel> DisponiblesPorTipo { get; set; } = new();
    }


    // =====================================================
    // ÁREAS COMUNES Y RESERVAS
    // =====================================================
    public class ReporteAreasComunesViewModel
    {
        public int TotalAreas { get; set; }
        public int AreasActivas { get; set; }
        public int AreasInactivas { get; set; }

        public int HorariosPublicados { get; set; }
        public int HorariosReservados { get; set; }

        public int TotalReservas { get; set; }
        public int ReservasActivas { get; set; }
        public int ReservasFinalizadas { get; set; }
        public int ReservasCanceladas { get; set; }

        // Top 5 áreas con más reservas en el rango
        public List<ConteoReporteViewModel> AreasMasReservadas { get; set; } = new();

        // Top 5 viviendas que más reservaron en el rango
        public List<ConteoReporteViewModel> ViviendasMasReservan { get; set; } = new();

        // Porcentaje de reservas canceladas sobre el total
        public double PorcentajeCancelacion { get; set; }

        // Reservas mes a mes (últimos 6 meses) para el gráfico de tendencia
        public List<ConteoReporteViewModel> ReservasPorMes { get; set; } = new();
    }


    // =====================================================
    // CONTROL DE ACCESOS
    // =====================================================
    public class ReporteAccesosViewModel
    {
        public int TotalAccesos { get; set; }
        public int VisitantesDentro { get; set; }
        public int AccesosConVehiculo { get; set; }

        public int TotalAutorizaciones { get; set; }
        public List<ConteoReporteViewModel> AutorizacionesPorEstado { get; set; } = new();

        // Top 5 viviendas que más visitas recibieron en el rango
        public List<ConteoReporteViewModel> ViviendasMasVisitadas { get; set; } = new();

        // Ingresos agrupados por franja horaria
        public List<ConteoReporteViewModel> IngresosPorFranja { get; set; } = new();

        // Ingresos por día de la semana: sirve para ver qué días hay más movimiento
        public List<ConteoReporteViewModel> IngresosPorDiaSemana { get; set; } = new();
    }


    // =====================================================
    // MANTENIMIENTO
    // =====================================================
    public class ReporteMantenimientoViewModel
    {
        public int Total { get; set; }
        public int Programados { get; set; }
        public int EnProceso { get; set; }
        public int Completados { get; set; }
        public int Cancelados { get; set; }

        // Trabajos que ya deberían haber arrancado y siguen sin iniciar
        public int Atrasados { get; set; }

        // Promedio de días entre el inicio y el cierre de los completados
        public double PromedioDiasResolucion { get; set; }

        public List<ConteoReporteViewModel> PorTipo { get; set; } = new();
        public List<ConteoReporteViewModel> PorPersonal { get; set; } = new();
        public List<ConteoReporteViewModel> PorUbicacion { get; set; } = new();
        public List<ConteoReporteViewModel> PorMes { get; set; } = new();
    }


    // =====================================================
    // FINANCIERO
    // =====================================================
    public class ReporteFinancieroViewModel
    {
        // Cantidad de cargos por estado
        public int TotalCargos { get; set; }
        public int CargosPendientes { get; set; }
        public int CargosEnRevision { get; set; }
        public int CargosPagados { get; set; }
        public int CargosVencidos { get; set; }

        // Dinero
        public decimal MontoEmitido { get; set; }
        public decimal MontoCobrado { get; set; }
        public decimal MontoPorCobrar { get; set; }
        public decimal MontoVencido { get; set; }
        public decimal MontoRecargos { get; set; }

        // Porcentaje del monto emitido que ya se cobró
        public double PorcentajeCobrado { get; set; }

        // Comprobantes rechazados por el administrador
        public int PagosRechazados { get; set; }

        public List<ConteoReporteViewModel> CargosPorTipo { get; set; } = new();
        public List<ConteoReporteViewModel> PagosPorMetodo { get; set; } = new();
        public List<ConteoReporteViewModel> ViviendasConMasDeuda { get; set; } = new();
        public List<ConteoReporteViewModel> CargosPorMes { get; set; } = new();
    }


    // =====================================================
    // CONTENEDOR DE TODOS LOS REPORTES
    // =====================================================
    public class ReporteGeneralViewModel
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }

        public ReporteUsuariosViewModel Usuarios { get; set; } = new();
        public ReporteViviendasViewModel Viviendas { get; set; } = new();
        public ReporteAreasComunesViewModel AreasComunes { get; set; } = new();
        public ReporteAccesosViewModel Accesos { get; set; } = new();

        // El reporte de incidencias y mantenimiento ya existía; se conserva igual.
        public ReporteIncidenciasViewModel Incidencias { get; set; } = new();

        // Va acá y no dentro de ReporteIncidenciasViewModel para no modificar
        // esa clase, que pertenece al módulo de incidencias.
        public List<ConteoReporteViewModel> IncidenciasPorMes { get; set; } = new();

        public ReporteMantenimientoViewModel Mantenimiento { get; set; } = new();

        public ReporteFinancieroViewModel Financiero { get; set; } = new();
    }
}