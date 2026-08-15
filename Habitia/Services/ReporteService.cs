using Habitia.Data;
using Habitia.Enums;
using Habitia.Services.Interfaces;
using Habitia.ViewModels.Reportes;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Services
{
    public class ReporteService : IReporteService
    {
        private readonly ApplicationDbContext _context;

        public ReporteService(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // REPORTE DE INCIDENCIAS Y MANTENIMIENTO (ya existía)
        // =====================================================
        public async Task<ReporteIncidenciasViewModel> GenerarReporteAsync(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var incidenciasQuery = _context.Incidencias
                .Include(i => i.Vivienda)
                .Include(i => i.AreaComun)
                .AsQueryable();

            if (fechaDesde.HasValue)
                incidenciasQuery = incidenciasQuery.Where(i => i.TF_FechaRegistro >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                incidenciasQuery = incidenciasQuery.Where(i => i.TF_FechaRegistro <= fechaHasta.Value.Date.AddDays(1).AddTicks(-1));

            var incidencias = await incidenciasQuery.ToListAsync();

            var reporte = new ReporteIncidenciasViewModel
            {
                TotalIncidencias = incidencias.Count,
                TotalPendientes = incidencias.Count(i => i.TN_Estado == EstadoIncidenciaEnum.Pendiente),
                TotalEnProceso = incidencias.Count(i => i.TN_Estado == EstadoIncidenciaEnum.EnProceso),
                TotalResueltas = incidencias.Count(i => i.TN_Estado == EstadoIncidenciaEnum.Resuelta)
            };

            reporte.PorResponsabilidad = incidencias
                .Where(i => i.TN_Responsabilidad != 0)
                .GroupBy(i => i.TN_Responsabilidad.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            reporte.PorTipoUbicacion = incidencias
                .GroupBy(i => i.TN_Tipo == TipoIncidenciaEnum.Vivienda ? "Vivienda" : "Área Común")
                .ToDictionary(g => g.Key, g => g.Count());

            reporte.UbicacionesMasFrecuentes = incidencias
                .GroupBy(i => i.TN_Tipo == TipoIncidenciaEnum.Vivienda
                    ? $"Vivienda: {i.Vivienda?.TC_Numero ?? "N/D"}"
                    : $"Área común: {i.AreaComun?.TC_Nombre ?? "N/D"}")
                .Select(g => new UbicacionFrecuenteViewModel
                {
                    Ubicacion = g.Key,
                    CantidadIncidencias = g.Count()
                })
                .OrderByDescending(x => x.CantidadIncidencias)
                .Take(5)
                .ToList();

            var mantenimientosQuery = _context.Mantenimientos
                .Include(m => m.Tipo)
                .AsQueryable();

            if (fechaDesde.HasValue)
                mantenimientosQuery = mantenimientosQuery.Where(m => m.TF_FechaInicio >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                mantenimientosQuery = mantenimientosQuery.Where(m => m.TF_FechaInicio <= fechaHasta.Value.Date.AddDays(1).AddTicks(-1));

            var mantenimientos = await mantenimientosQuery.ToListAsync();

            reporte.TotalMantenimientosRealizados = mantenimientos.Count(m => m.TN_Estado == EstadoMantenimientoEnum.Completado);
            reporte.TotalMantenimientosPendientes = mantenimientos.Count(m =>
                m.TN_Estado == EstadoMantenimientoEnum.Programado || m.TN_Estado == EstadoMantenimientoEnum.EnProceso);

            reporte.MantenimientosPorTipo = mantenimientos
                .GroupBy(m => m.Tipo?.TC_Nombre ?? "Sin tipo")
                .ToDictionary(g => g.Key, g => g.Count());

            return reporte;
        }


        // =====================================================
        // REPORTE GENERAL (todas las secciones)
        // =====================================================
        public async Task<ReporteGeneralViewModel> GenerarReporteGeneralAsync(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var desde = fechaDesde?.Date;
            var hasta = fechaHasta?.Date.AddDays(1).AddTicks(-1);

            var modelo = new ReporteGeneralViewModel
            {
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                Incidencias = await GenerarReporteAsync(fechaDesde, fechaHasta),
                Usuarios = await ReporteUsuariosAsync(desde, hasta),
                Viviendas = await ReporteViviendasAsync(),
                AreasComunes = await ReporteAreasComunesAsync(desde, hasta),
                Accesos = await ReporteAccesosAsync(desde, hasta)
            };

            // Tendencia mensual de incidencias (últimos 6 meses, sin filtro).
            var fechasIncidencias = await _context.Incidencias
                .Select(i => i.TF_FechaRegistro)
                .ToListAsync();

            modelo.IncidenciasPorMes = AgruparPorMes(fechasIncidencias);

            return modelo;
        }


        // ===================== USUARIOS =====================
        private async Task<ReporteUsuariosViewModel> ReporteUsuariosAsync(DateTime? desde, DateTime? hasta)
        {
            var usuarios = await _context.Users.ToListAsync();

            var r = new ReporteUsuariosViewModel
            {
                Total = usuarios.Count,
                Activos = usuarios.Count(u => u.TN_Estado == EstadoUsuarioEnum.Activo),
                Pendientes = usuarios.Count(u => u.TN_Estado == EstadoUsuarioEnum.Pendiente),
                Suspendidos = usuarios.Count(u => u.TN_Estado == EstadoUsuarioEnum.Suspendido),
                Rechazados = usuarios.Count(u => u.TN_Estado == EstadoUsuarioEnum.Rechazado)
            };

            r.RegistradosEnRango = usuarios.Count(u =>
                (!desde.HasValue || u.TF_FechaRegistro >= desde.Value) &&
                (!hasta.HasValue || u.TF_FechaRegistro <= hasta.Value));

            // Tendencia de altas: no depende del filtro, siempre últimos 6 meses.
            r.RegistrosPorMes = AgruparPorMes(usuarios.Select(u => u.TF_FechaRegistro));

            // Cantidad de usuarios por rol, uniendo las tablas de Identity.
            var roles = await _context.Roles.ToListAsync();
            var usuarioRoles = await _context.UserRoles.ToListAsync();

            r.PorRol = roles
                .Select(rol => new ConteoReporteViewModel
                {
                    Etiqueta = rol.Name,
                    Cantidad = usuarioRoles.Count(ur => ur.RoleId == rol.Id)
                })
                .OrderByDescending(x => x.Cantidad)
                .ToList();

            // Relación con la vivienda (propietario / inquilino), solo vínculos activos.
            var relaciones = await _context.ViviendaUsuarios
                .Where(v => v.TN_Estado == EstadoUsuarioEnum.Activo)
                .ToListAsync();

            r.PorTipoRelacion = relaciones
                .GroupBy(v => v.TN_TipoRelacion.ToString())
                .Select(g => new ConteoReporteViewModel
                {
                    Etiqueta = g.Key,
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .ToList();

            return r;
        }


        // ===================== VIVIENDAS =====================
        // No se filtra por fecha: es una foto del estado actual del condominio.
        private async Task<ReporteViviendasViewModel> ReporteViviendasAsync()
        {
            var viviendas = await _context.Viviendas
                .Include(v => v.Usuarios)
                .ToListAsync();

            var r = new ReporteViviendasViewModel
            {
                Total = viviendas.Count,
                Ocupadas = viviendas.Count(v => v.TN_Estado == EstadoViviendaEnum.Ocupada),
                Disponibles = viviendas.Count(v => v.TN_Estado == EstadoViviendaEnum.Disponible),
                Inactivas = viviendas.Count(v => v.TN_Estado == EstadoViviendaEnum.Inactiva),
                SinCupoConfigurado = viviendas.Count(v => v.TN_CantidadInquilinos == 0)
            };

            r.SinPropietario = viviendas.Count(v =>
                !v.Usuarios.Any(u => u.TN_TipoRelacion == TipoRelacionEnum.Propietario &&
                                     u.TN_Estado == EstadoUsuarioEnum.Activo));

            r.PorTipo = viviendas
                .GroupBy(v => v.TN_Tipo == TipoViviendaEnum.Apartamento ? "Departamento" : v.TN_Tipo.ToString())
                .Select(g => new ConteoReporteViewModel
                {
                    Etiqueta = g.Key,
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .ToList();

            r.PorcentajeOcupacion = r.Total == 0
                ? 0
                : Math.Round((double)r.Ocupadas * 100 / r.Total, 1);

            // Comparativa por tipo: cuántas están ocupadas y cuántas libres.
            var tipos = viviendas
                .Select(v => v.TN_Tipo == TipoViviendaEnum.Apartamento ? "Departamento" : v.TN_Tipo.ToString())
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            r.OcupadasPorTipo = tipos
                .Select(t => new ConteoReporteViewModel
                {
                    Etiqueta = t,
                    Cantidad = viviendas.Count(v =>
                        (v.TN_Tipo == TipoViviendaEnum.Apartamento ? "Departamento" : v.TN_Tipo.ToString()) == t &&
                        v.TN_Estado == EstadoViviendaEnum.Ocupada)
                })
                .ToList();

            r.DisponiblesPorTipo = tipos
                .Select(t => new ConteoReporteViewModel
                {
                    Etiqueta = t,
                    Cantidad = viviendas.Count(v =>
                        (v.TN_Tipo == TipoViviendaEnum.Apartamento ? "Departamento" : v.TN_Tipo.ToString()) == t &&
                        v.TN_Estado == EstadoViviendaEnum.Disponible)
                })
                .ToList();

            return r;
        }


        // ===================== ÁREAS COMUNES Y RESERVAS =====================
        private async Task<ReporteAreasComunesViewModel> ReporteAreasComunesAsync(DateTime? desde, DateTime? hasta)
        {
            var areas = await _context.AreasComunes.ToListAsync();

            var r = new ReporteAreasComunesViewModel
            {
                TotalAreas = areas.Count,
                AreasActivas = areas.Count(a => a.TB_Estado),
                AreasInactivas = areas.Count(a => !a.TB_Estado)
            };

            var horarios = await _context.Disponibilidades
                .Where(d => d.TB_Estado)
                .ToListAsync();

            r.HorariosPublicados = horarios.Count;
            r.HorariosReservados = horarios.Count(d => d.TB_Reservado);

            var reservas = await _context.Reservas
                .Include(x => x.Disponibilidad).ThenInclude(d => d.AreaComun)
                .Include(x => x.Vivienda)
                .ToListAsync();

            // El rango se aplica sobre la fecha del horario reservado.
            if (desde.HasValue)
                reservas = reservas.Where(x => x.Disponibilidad.TF_Fecha >= desde.Value).ToList();

            if (hasta.HasValue)
                reservas = reservas.Where(x => x.Disponibilidad.TF_Fecha <= hasta.Value).ToList();

            r.TotalReservas = reservas.Count;
            r.ReservasActivas = reservas.Count(x => x.TN_Estado == EstadoReservaEnum.Activa);
            r.ReservasFinalizadas = reservas.Count(x => x.TN_Estado == EstadoReservaEnum.Finalizada);
            r.ReservasCanceladas = reservas.Count(x => x.TN_Estado == EstadoReservaEnum.Cancelada);

            r.PorcentajeCancelacion = r.TotalReservas == 0
                ? 0
                : Math.Round((double)r.ReservasCanceladas * 100 / r.TotalReservas, 1);

            // Tendencia mensual: usa todas las reservas, no solo las del rango.
            var todasLasReservas = await _context.Reservas
                .Include(x => x.Disponibilidad)
                .ToListAsync();

            r.ReservasPorMes = AgruparPorMes(todasLasReservas.Select(x => x.Disponibilidad.TF_Fecha));

            r.AreasMasReservadas = reservas
                .GroupBy(x => x.Disponibilidad.AreaComun?.TC_Nombre ?? "N/D")
                .Select(g => new ConteoReporteViewModel
                {
                    Etiqueta = g.Key,
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .Take(5)
                .ToList();

            r.ViviendasMasReservan = reservas
                .GroupBy(x => x.Vivienda?.TC_Numero ?? "N/D")
                .Select(g => new ConteoReporteViewModel
                {
                    Etiqueta = "Vivienda " + g.Key,
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .Take(5)
                .ToList();

            return r;
        }


        // ===================== CONTROL DE ACCESOS =====================
        private async Task<ReporteAccesosViewModel> ReporteAccesosAsync(DateTime? desde, DateTime? hasta)
        {
            var accesosQuery = _context.Accesos
                .Include(a => a.Vivienda)
                .AsQueryable();

            if (desde.HasValue)
                accesosQuery = accesosQuery.Where(a => a.TF_FechaIngreso >= desde.Value);

            if (hasta.HasValue)
                accesosQuery = accesosQuery.Where(a => a.TF_FechaIngreso <= hasta.Value);

            var accesos = await accesosQuery.ToListAsync();

            var r = new ReporteAccesosViewModel
            {
                TotalAccesos = accesos.Count,
                AccesosConVehiculo = accesos.Count(a => a.TN_IdVehiculo != null)
            };

            // Los que siguen dentro se cuentan siempre, sin importar el rango.
            r.VisitantesDentro = await _context.Accesos
                .CountAsync(a => a.TF_FechaSalida == null && a.TB_Estado);

            r.ViviendasMasVisitadas = accesos
                .GroupBy(a => a.Vivienda?.TC_Numero ?? "N/D")
                .Select(g => new ConteoReporteViewModel
                {
                    Etiqueta = "Vivienda " + g.Key,
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .Take(5)
                .ToList();

            // Franjas horarias, para ver a qué hora entra más gente.
            r.IngresosPorFranja = accesos
                .GroupBy(a => FranjaHoraria(a.TF_FechaIngreso.Hour))
                .Select(g => new ConteoReporteViewModel
                {
                    Etiqueta = g.Key,
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .ToList();

            // Día de la semana con más movimiento.
            var diasSemana = new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };

            r.IngresosPorDiaSemana = diasSemana
                .Select((nombre, indice) => new ConteoReporteViewModel
                {
                    Etiqueta = nombre,
                    Cantidad = accesos.Count(a => ((int)a.TF_FechaIngreso.DayOfWeek + 6) % 7 == indice)
                })
                .ToList();

            var autorizacionesQuery = _context.Autorizaciones.AsQueryable();

            if (desde.HasValue)
                autorizacionesQuery = autorizacionesQuery.Where(a => a.TF_FechaRegistro >= desde.Value);

            if (hasta.HasValue)
                autorizacionesQuery = autorizacionesQuery.Where(a => a.TF_FechaRegistro <= hasta.Value);

            var autorizaciones = await autorizacionesQuery.ToListAsync();

            r.TotalAutorizaciones = autorizaciones.Count;

            r.AutorizacionesPorEstado = autorizaciones
                .GroupBy(a => a.TN_Estado.ToString())
                .Select(g => new ConteoReporteViewModel
                {
                    Etiqueta = g.Key,
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .ToList();

            return r;
        }


        // Últimos 6 meses en orden cronológico, con etiqueta "may 2026".
        private static List<(int Anio, int Mes, string Etiqueta)> UltimosSeisMeses()
        {
            var cultura = new System.Globalization.CultureInfo("es-ES");
            var lista = new List<(int, int, string)>();

            for (int i = 5; i >= 0; i--)
            {
                var fecha = DateTime.Today.AddMonths(-i);
                var etiqueta = fecha.ToString("MMM yyyy", cultura);
                lista.Add((fecha.Year, fecha.Month, etiqueta));
            }

            return lista;
        }


        private static List<ConteoReporteViewModel> AgruparPorMes(IEnumerable<DateTime> fechas)
        {
            var lista = fechas.ToList();

            return UltimosSeisMeses()
                .Select(m => new ConteoReporteViewModel
                {
                    Etiqueta = m.Etiqueta,
                    Cantidad = lista.Count(f => f.Year == m.Anio && f.Month == m.Mes)
                })
                .ToList();
        }


        private static string FranjaHoraria(int hora)
        {
            if (hora < 6) return "Madrugada (12 a 6 am)";
            if (hora < 12) return "Mañana (6 am a 12 md)";
            if (hora < 18) return "Tarde (12 md a 6 pm)";
            return "Noche (6 pm a 12 am)";
        }
    }
}