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
                .Where(t => t.TB_Estado)
                .OrderBy(t => t.TC_Nombre)
                .ToListAsync();
        }

        public async Task<bool> ExisteNombreAsync(string nombre)
        {
            var nombreNormalizado = nombre.Trim().ToLower();

            return await _context.TiposMantenimiento
                .AnyAsync(t => t.TC_Nombre.ToLower() == nombreNormalizado);
        }

        public async Task<TipoMantenimiento> ObtenerOCrearAsync(int? idExistente, string? nombreNuevo)
        {
            if (idExistente.HasValue)
            {
                var tipo = await _context.TiposMantenimiento
                    .FirstOrDefaultAsync(t => t.TN_Id == idExistente.Value);

                if (tipo == null)
                    throw new InvalidOperationException("El tipo de mantenimiento seleccionado no existe.");

                return tipo;
            }

            if (string.IsNullOrWhiteSpace(nombreNuevo))
                throw new InvalidOperationException("Debe seleccionar un tipo existente o ingresar uno nuevo.");

            var nombreLimpio = nombreNuevo.Trim();

            if (await ExisteNombreAsync(nombreLimpio))
            {
                throw new InvalidOperationException(
                    $"Ya existe un tipo de mantenimiento llamado '{nombreLimpio}'. Selecciónelo de la lista.");
            }

            var nuevoTipo = new TipoMantenimiento
            {
                TC_Nombre = nombreLimpio,
                TB_Estado = true
            };

            _context.TiposMantenimiento.Add(nuevoTipo);
            await _context.SaveChangesAsync();

            return nuevoTipo;
        }
    }
}