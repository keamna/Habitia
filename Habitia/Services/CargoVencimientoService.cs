// /Services/CargoVencimientoService.cs
using Habitia.Data;
using Habitia.Enums;
using Habitia.Models.Financiero;
using Habitia.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Services
{
    public class CargoVencimientoService : ICargoVencimientoService
    {
        private const string FRECUENCIA_UNICO = "Unico";
        private const string FRECUENCIA_POR_DIA = "PorDia";

        private readonly ApplicationDbContext _context;

        public CargoVencimientoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ProcesarCargosVencidosAsync()
        {
            var hoy = DateTime.Today;

            // ============ PASO 1: Pendiente -> Vencido (+ recargo único al momento de vencer) ============
            var cargosPorVencer = await _context.Cargos
                .Where(c => c.TB_Estado
                    && c.TN_IdEstadoCargo == (int)EstadoCargoEnum.Pendiente
                    && c.TF_FechaVencimiento.Date < hoy)
                .ToListAsync();

            foreach (var cargo in cargosPorVencer)
            {
                cargo.TN_IdEstadoCargo = (int)EstadoCargoEnum.Vencido;

                // Recargo "único": se aplica en el instante en que el cargo pasa a Vencido
                var esRecargoUnico = cargo.TB_RecargoProgramado
                    && !cargo.TB_RecargoAplicado
                    && cargo.TN_IdTipoRecargo.HasValue
                    && cargo.TN_ValorRecargo.HasValue
                    && (cargo.TC_FrecuenciaRecargo == FRECUENCIA_UNICO || cargo.TC_FrecuenciaRecargo == null);
                // null se trata como "Unico" por compatibilidad con cargos creados antes de este cambio

                if (esRecargoUnico)
                {
                    await AplicarRecargoAsync(cargo);
                    cargo.TB_RecargoAplicado = true;
                    cargo.TF_UltimaAplicacionRecargo = DateTime.Now;
                }
            }

            // ============ PASO 2: recargo "por día" sobre cargos que YA estaban Vencido ============
            var cargosVencidosConRecargoPorDia = await _context.Cargos
                .Where(c => c.TB_Estado
                    && c.TN_IdEstadoCargo == (int)EstadoCargoEnum.Vencido
                    && c.TB_RecargoProgramado
                    && c.TC_FrecuenciaRecargo == FRECUENCIA_POR_DIA
                    && c.TN_IdTipoRecargo != null
                    && c.TN_ValorRecargo != null)
                .ToListAsync();

            foreach (var cargo in cargosVencidosConRecargoPorDia)
            {
                // Ya se procesó hoy (evita duplicar si el job corre más de una vez el mismo día,
                // o si el cargo acaba de venir del Paso 1 en esta misma ejecución)
                if (cargo.TF_UltimaAplicacionRecargo?.Date == hoy)
                    continue;

                await AplicarRecargoAsync(cargo);
                cargo.TF_UltimaAplicacionRecargo = DateTime.Now;
                // No se marca TB_RecargoAplicado = true aquí: "por día" debe poder repetirse.
            }

            if (cargosPorVencer.Count > 0 || cargosVencidosConRecargoPorDia.Count > 0)
            {
                await _context.SaveChangesAsync();
            }
        }

        // ============ Helper: crea el registro de recargo y actualiza el total del cargo ============
        private async Task AplicarRecargoAsync(THBT_A_Cargo cargo)
        {
            var tipoRecargo = await _context.TiposRecargo
                .FirstAsync(t => t.TN_Id == cargo.TN_IdTipoRecargo!.Value);

            var montoAplicado = tipoRecargo.TC_Nombre == "Porcentaje"
                ? Math.Round(cargo.TN_MontoTotal * (cargo.TN_ValorRecargo!.Value / 100m), 2)
                : cargo.TN_ValorRecargo!.Value;

            _context.Recargos.Add(new THBT_A_Recargo
            {
                TN_IdCargo = cargo.TN_Id,
                TN_IdTipoRecargo = tipoRecargo.TN_Id,
                TN_Valor = cargo.TN_ValorRecargo.Value,
                TN_MontoAplicado = montoAplicado,
                TF_FechaAplicacion = DateTime.Now,
                TB_Estado = true
            });

            cargo.TN_MontoTotal += montoAplicado;
        }
    }
}