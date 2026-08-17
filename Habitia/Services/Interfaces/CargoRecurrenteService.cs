using Habitia.Data;
using Habitia.Enums;
using Habitia.Models.Financiero;
using Habitia.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Services
{
    public class CargoRecurrenteService : ICargoRecurrenteService
    {
        private const decimal PORCENTAJE_IVA = 0.13m;
        private readonly ApplicationDbContext _context;

        public CargoRecurrenteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task GenerarCargosPendientesAsync()
        {
            var hoy = DateTime.Today;

            var plantillas = await _context.CargosRecurrentes
                .Include(cr => cr.Residentes)
                .Where(cr => cr.TB_Estado && cr.TF_FechaInicio.Date <= hoy)
                .ToListAsync();

            foreach (var plantilla in plantillas)
            {
                if (!CorrespondeGenerarHoy(plantilla, hoy))
                    continue;

                await GenerarCargosParaPlantillaAsync(plantilla, hoy);
            }

            await _context.SaveChangesAsync();
        }

        // NUEVO: genera el primer cargo de inmediato al crear la plantilla,
        // sin esperar a la corrida del job periódico. Se llama una sola vez
        // desde el Create del controlador.
        public async Task GenerarCargoInmediatoAsync(int idCargoRecurrente)
        {
            var hoy = DateTime.Today;

            var plantilla = await _context.CargosRecurrentes
                .Include(cr => cr.Residentes)
                .FirstOrDefaultAsync(cr => cr.TN_Id == idCargoRecurrente);

            if (plantilla == null || !plantilla.TB_Estado)
                return;

            // Solo genera si de verdad no se ha generado nada aún para esta
            // plantilla (evita duplicar si por alguna razón ya corrió el job).
            if (plantilla.TF_UltimaGeneracion != null)
                return;

            await GenerarCargosParaPlantillaAsync(plantilla, hoy);
            await _context.SaveChangesAsync();
        }

        // Lógica común de generación para una sola plantilla (extraída para
        // reutilizarse tanto en el job periódico como en la generación inmediata).
        private async Task GenerarCargosParaPlantillaAsync(THBT_A_CargoRecurrente plantilla, DateTime hoy)
        {
            var residentesDestino = plantilla.TB_AplicarATodos
                ? await _context.Users
                    .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id &&
                        _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Residente")))
                    .Select(u => u.Id)
                    .ToListAsync()
                : plantilla.Residentes.Select(r => r.TC_IdResidente).ToList();

            var estadoPendiente = await _context.EstadosCargo
                .FirstAsync(e => e.TN_Id == (int)EstadoCargoEnum.Pendiente);

            var montoIva = plantilla.TB_AplicaIva
                ? Math.Round(plantilla.TN_MontoBase * PORCENTAJE_IVA, 2)
                : 0m;
            var montoTotal = plantilla.TN_MontoBase + montoIva;

            foreach (var idResidente in residentesDestino)
            {
                var viviendasDelResidente = await _context.ViviendaUsuarios
                    .Where(vu => vu.TC_IdUsuario == idResidente && vu.TN_Estado == EstadoUsuarioEnum.Activo)
                    .Select(vu => (int?)vu.TN_IdVivienda)
                    .Distinct()
                    .ToListAsync();

                var destinosVivienda = viviendasDelResidente.Count > 0
                    ? viviendasDelResidente
                    : new List<int?> { null };

                foreach (var idVivienda in destinosVivienda)
                {
                    _context.Cargos.Add(new THBT_A_Cargo
                    {
                        TC_IdResidente = idResidente,
                        TN_IdVivienda = idVivienda,
                        TN_IdTipoCargo = plantilla.TN_IdTipoCargo,
                        TN_IdEstadoCargo = estadoPendiente.TN_Id,
                        TC_Descripcion = plantilla.TC_Descripcion,
                        TN_MontoBase = plantilla.TN_MontoBase,
                        TB_AplicaIva = plantilla.TB_AplicaIva,
                        TN_MontoIva = montoIva,
                        TN_MontoTotal = montoTotal,
                        TF_FechaEmision = DateTime.Now,
                        TF_FechaVencimiento = hoy,
                        TB_Estado = true,
                        TB_RecargoProgramado = plantilla.TB_RecargoProgramado,
                        TN_IdTipoRecargo = plantilla.TN_IdTipoRecargo,
                        TN_ValorRecargo = plantilla.TN_ValorRecargo,
                        TC_FrecuenciaRecargo = plantilla.TC_FrecuenciaRecargo
                    });
                }
            }

            plantilla.TF_UltimaGeneracion = hoy;
        }

        private static bool CorrespondeGenerarHoy(THBT_A_CargoRecurrente plantilla, DateTime hoy)
        {
            if (plantilla.TF_UltimaGeneracion == null)
                return true; // primera ejecución desde la fecha de inicio

            var ultima = plantilla.TF_UltimaGeneracion.Value.Date;

            return plantilla.TC_Frecuencia switch
            {
                "Diario" => ultima < hoy,
                "Semanal" => (hoy - ultima).Days >= 7,
                "Mensual" => hoy >= ultima.AddMonths(1),
                _ => false
            };
        }
    }
}