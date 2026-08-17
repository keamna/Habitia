using Habitia.Data;
using Habitia.Enums;
using Habitia.Models;
using Habitia.Services.Interfaces;
using Habitia.ViewModels.Incidencias;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Services
{
    public class IncidenciaService : IIncidenciaService
    {
        private readonly ApplicationDbContext _context;

        public IncidenciaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Incidencia> CrearAsync(IncidenciaCreateViewModel model, string idUsuario, string? imagenUrl)
        {
            var responsabilidad = model.Responsabilidad!.Value;

            var tipo = responsabilidad switch
            {
                ResponsabilidadEnum.Privado => TipoIncidenciaEnum.Vivienda,
                ResponsabilidadEnum.Comun => TipoIncidenciaEnum.AreaComun,
                ResponsabilidadEnum.Mixto => TipoIncidenciaEnum.Mixto,
                _ => throw new InvalidOperationException("Tipo de responsabilidad no válido.")
            };

            var incidencia = new Incidencia
            {
                TC_IdUsuario = idUsuario,
                TN_Tipo = tipo,
                TN_IdVivienda = responsabilidad is ResponsabilidadEnum.Privado or ResponsabilidadEnum.Mixto
                    ? model.IdVivienda
                    : null,
                TN_IdAreaComun = responsabilidad is ResponsabilidadEnum.Comun or ResponsabilidadEnum.Mixto
                    ? model.IdAreaComun
                    : null,
                TN_Responsabilidad = responsabilidad,
                TC_Titulo = model.Titulo.Trim(),
                TC_Descripcion = model.Descripcion.Trim(),
                TC_ImagenUrl = imagenUrl,
                TN_Estado = EstadoIncidenciaEnum.Pendiente,
                TF_FechaRegistro = DateTime.Now
            };

            _context.Incidencias.Add(incidencia);
            await _context.SaveChangesAsync();

            return incidencia;
        }

        public async Task<List<IncidenciaListItemViewModel>> ObtenerTodasAsync(IncidenciaFiltroViewModel? filtro = null)
        {
            var query = _context.Incidencias
                .Include(i => i.Usuario)
                .Include(i => i.Vivienda)
                .Include(i => i.AreaComun)
                .Include(i => i.Mantenimiento)
                    .ThenInclude(m => m!.Tipo)
                .Include(i => i.Mantenimiento)
                    .ThenInclude(m => m!.PersonalAsignado)
                .AsQueryable();

            if (filtro != null)
            {
                if (filtro.Estado.HasValue)
                    query = query.Where(i => i.TN_Estado == filtro.Estado.Value);

                if (filtro.Responsabilidad.HasValue)
                    query = query.Where(i => i.TN_Responsabilidad == filtro.Responsabilidad.Value);

                if (filtro.FechaDesde.HasValue)
                    query = query.Where(i => i.TF_FechaRegistro >= filtro.FechaDesde.Value.Date);

                if (filtro.FechaHasta.HasValue)
                    query = query.Where(i => i.TF_FechaRegistro <= filtro.FechaHasta.Value.Date.AddDays(1).AddTicks(-1));
            }

            var incidencias = await query
                .OrderByDescending(i => i.TF_FechaRegistro)
                .ToListAsync();

            var tiposPorPersonal = await ObtenerTiposPorPersonalAsync(incidencias);

            return incidencias.Select(i => MapearAListItem(i, tiposPorPersonal)).ToList();
        }

        public async Task<List<IncidenciaListItemViewModel>> ObtenerPorUsuarioAsync(string idUsuario, IncidenciaFiltroViewModel? filtro = null)
        {
            var query = _context.Incidencias
                .Include(i => i.Usuario)
                .Include(i => i.Vivienda)
                .Include(i => i.AreaComun)
                .Include(i => i.Mantenimiento)
                    .ThenInclude(m => m!.Tipo)
                .Include(i => i.Mantenimiento)
                    .ThenInclude(m => m!.PersonalAsignado)
                .Where(i => i.TC_IdUsuario == idUsuario)
                .AsQueryable();

            if (filtro != null)
            {
                if (filtro.Estado.HasValue)
                    query = query.Where(i => i.TN_Estado == filtro.Estado.Value);

                if (filtro.Responsabilidad.HasValue)
                    query = query.Where(i => i.TN_Responsabilidad == filtro.Responsabilidad.Value);

                if (filtro.FechaDesde.HasValue)
                    query = query.Where(i => i.TF_FechaRegistro >= filtro.FechaDesde.Value.Date);

                if (filtro.FechaHasta.HasValue)
                    query = query.Where(i => i.TF_FechaRegistro <= filtro.FechaHasta.Value.Date.AddDays(1).AddTicks(-1));
            }

            var incidencias = await query
                .OrderByDescending(i => i.TF_FechaRegistro)
                .ToListAsync();

            var tiposPorPersonal = await ObtenerTiposPorPersonalAsync(incidencias);

            return incidencias.Select(i => MapearAListItem(i, tiposPorPersonal)).ToList();
        }

        public async Task<Incidencia?> ObtenerPorIdAsync(int id)
        {
            return await _context.Incidencias
                .Include(i => i.Usuario)
                .Include(i => i.Vivienda)
                .Include(i => i.AreaComun)
                .Include(i => i.Mantenimiento)
                    .ThenInclude(m => m!.Tipo)
                .Include(i => i.Mantenimiento)
                    .ThenInclude(m => m!.PersonalAsignado)
                .FirstOrDefaultAsync(i => i.TN_Id == id);
        }

        public async Task AsignarPrioridadAsync(IncidenciaPrioridadViewModel model)
        {
            var incidencia = await _context.Incidencias.FirstOrDefaultAsync(i => i.TN_Id == model.Id);

            if (incidencia == null)
                throw new InvalidOperationException("La incidencia no existe.");

            incidencia.TN_Prioridad = model.Prioridad;
            await _context.SaveChangesAsync();
        }

        public async Task CambiarEstadoAsync(int idIncidencia, EstadoIncidenciaEnum nuevoEstado)
        {
            var incidencia = await _context.Incidencias.FirstOrDefaultAsync(i => i.TN_Id == idIncidencia);

            if (incidencia == null)
                throw new InvalidOperationException("La incidencia no existe.");

            incidencia.TN_Estado = nuevoEstado;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TieneMantenimientoAsociadoAsync(int idIncidencia)
        {
            return await _context.Mantenimientos.AnyAsync(m => m.TN_IdIncidencia == idIncidencia);
        }

        public async Task MarcarComoResueltaAsync(int idIncidencia, string idUsuarioQueResuelve, bool esAdmin)
        {
            var incidencia = await _context.Incidencias.FirstOrDefaultAsync(i => i.TN_Id == idIncidencia);

            if (incidencia == null)
                throw new InvalidOperationException("La incidencia no existe.");

            if (incidencia.TN_Responsabilidad != ResponsabilidadEnum.Privado)
                throw new InvalidOperationException("No fue posible completar la operación. Intente nuevamente.");

            if (!esAdmin && incidencia.TC_IdUsuario != idUsuarioQueResuelve)
                throw new UnauthorizedAccessException("No tiene permiso para modificar esta incidencia.");

            incidencia.TN_Estado = EstadoIncidenciaEnum.Resuelta;
            incidencia.TF_FechaResolucion = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task<List<Incidencia>> ObtenerElegiblesParaMantenimientoAsync()
        {
            var idsConMantenimiento = await _context.Mantenimientos
                .Select(m => m.TN_IdIncidencia)
                .ToListAsync();

            return await _context.Incidencias
                .Include(i => i.Vivienda)
                .Include(i => i.AreaComun)
                .Where(i =>
                    (i.TN_Responsabilidad == ResponsabilidadEnum.Comun || i.TN_Responsabilidad == ResponsabilidadEnum.Mixto) &&
                    !idsConMantenimiento.Contains(i.TN_Id))
                .OrderByDescending(i => i.TF_FechaRegistro)
                .ToListAsync();
        }

        // Construye un diccionario "idPersonal -> nombres de tipos de mantenimiento que maneja"
        // solo para el personal que aparece asignado en el lote de incidencias dado.
        private async Task<Dictionary<string, List<string>>> ObtenerTiposPorPersonalAsync(List<Incidencia> incidencias)
        {
            var idsPersonal = incidencias
                .Where(i => i.Mantenimiento != null)
                .Select(i => i.Mantenimiento!.TC_IdPersonalAsignado)
                .Distinct()
                .ToList();

            if (!idsPersonal.Any())
                return new Dictionary<string, List<string>>();

            var asignaciones = await _context.PersonalTipoMantenimiento
                .Where(x => idsPersonal.Contains(x.TC_IdPersonal))
                .ToListAsync();

            var tiposIds = asignaciones.Select(a => a.TN_IdTipo).Distinct().ToList();

            var tipos = await _context.TiposMantenimiento
                .Where(t => tiposIds.Contains(t.TN_Id))
                .ToDictionaryAsync(t => t.TN_Id, t => t.TC_Nombre);

            return asignaciones
                .GroupBy(a => a.TC_IdPersonal)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(a => tipos.GetValueOrDefault(a.TN_IdTipo, "N/D")).ToList());
        }

        private static IncidenciaListItemViewModel MapearAListItem(Incidencia i, Dictionary<string, List<string>> tiposPorPersonal)
        {
            var reportante = i.Usuario;
            var personal = i.Mantenimiento?.PersonalAsignado;

            return new IncidenciaListItemViewModel
            {
                Id = i.TN_Id,
                Titulo = i.TC_Titulo,
                Ubicacion = i.TN_Tipo switch
                {
                    TipoIncidenciaEnum.Vivienda => $"Vivienda: {i.Vivienda?.TC_Numero ?? "N/D"}",
                    TipoIncidenciaEnum.AreaComun => $"Área común: {i.AreaComun?.TC_Nombre ?? "N/D"}",
                    TipoIncidenciaEnum.Mixto => $"Vivienda: {i.Vivienda?.TC_Numero ?? "N/D"} · Área común: {i.AreaComun?.TC_Nombre ?? "N/D"}",
                    _ => "N/D"
                },
                Estado = i.TN_Estado,
                Responsabilidad = i.TN_Responsabilidad,
                Prioridad = i.TN_Prioridad,
                FechaRegistro = i.TF_FechaRegistro,

                ReportanteNombreCompleto = reportante != null ? $"{reportante.TC_Nombre} {reportante.TC_Apellido}" : "N/D",
                ReportanteIdentificacion = reportante?.TC_Identificacion,
                ReportanteTelefono = reportante?.TC_Telefono,
                ReportanteCorreo = reportante?.Email,

                TieneMantenimientoAsociado = i.Mantenimiento != null,
                MantenimientoTipoNombre = i.Mantenimiento?.Tipo?.TC_Nombre,
                MantenimientoEstado = i.Mantenimiento?.TN_Estado,
                MantenimientoId = i.Mantenimiento?.TN_Id,

                PersonalAsignadoNombreCompleto = personal != null ? $"{personal.TC_Nombre} {personal.TC_Apellido}" : null,
                PersonalAsignadoIdentificacion = personal?.TC_Identificacion,
                PersonalAsignadoTelefono = personal?.TC_Telefono,
                PersonalAsignadoCorreo = personal?.Email,
                PersonalAsignadoTipos = personal != null
                    ? tiposPorPersonal.GetValueOrDefault(personal.Id, new List<string>())
                    : new List<string>()
            };
        }
    }
}