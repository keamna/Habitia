using Habitia.Data;
using Habitia.Models.Catalogos;
using Habitia.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Services
{
    public class TipoMantenimientoService : ITipoMantenimientoService
    {
        private readonly ApplicationDbContext _context;

        public TipoMantenimientoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TipoMantenimiento>> ObtenerActivosAsync()
        {
            return await _context.TiposMantenimiento
                .Where(t => t.Estado)
                .OrderBy(t => t.Nombre)
                .ToListAsync();
        }

        public async Task<bool> ExisteNombreAsync(string nombre)
        {
            var nombreNormalizado = nombre.Trim().ToLower();

            return await _context.TiposMantenimiento
                .AnyAsync(t => t.Nombre.ToLower() == nombreNormalizado);
        }

        /// <summary>
        /// Devuelve un tipo de mantenimiento existente por Id, o crea uno nuevo si se
        /// proporciona un nombre. Aplica la regla de negocio: no se permiten tipos duplicados.
        /// </summary>
        public async Task<TipoMantenimiento> ObtenerOCrearAsync(int? idExistente, string? nombreNuevo)
        {
            // Caso 1: se seleccionó un tipo existente
            if (idExistente.HasValue)
            {
                var tipo = await _context.TiposMantenimiento
                    .FirstOrDefaultAsync(t => t.Id == idExistente.Value);

                if (tipo == null)
                {
                    throw new InvalidOperationException(
                        "El tipo de mantenimiento seleccionado no existe.");
                }

                return tipo;
            }

            // Caso 2: se ingresó un nombre nuevo
            if (string.IsNullOrWhiteSpace(nombreNuevo))
            {
                throw new InvalidOperationException(
                    "Debe seleccionar un tipo existente o ingresar uno nuevo.");
            }

            var nombreLimpio = nombreNuevo.Trim();

            if (await ExisteNombreAsync(nombreLimpio))
            {
                throw new InvalidOperationException(
                    $"Ya existe un tipo de mantenimiento llamado '{nombreLimpio}'. " +
                    "Selecciónelo de la lista en lugar de crear uno nuevo.");
            }

            var nuevoTipo = new TipoMantenimiento
            {
                Nombre = nombreLimpio,
                Estado = true
            };

            _context.TiposMantenimiento.Add(nuevoTipo);
            await _context.SaveChangesAsync();

            return nuevoTipo;
        }
    }
}