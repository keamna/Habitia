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
        private readonly ApplicationDbContext _context;

        public CargoVencimientoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ProcesarCargosVencidosAsync()
        {
            var hoy = DateTime.Today;

            var cargosVencidos = await _context.Cargos
                .Where(c => c.TB_Estado
                    && c.TN_IdEstadoCargo == (int)EstadoCargoEnum.Pendiente
                    && c.TF_FechaVencimiento.Date < hoy)
                .ToListAsync();

            if (cargosVencidos.Count == 0) return;

            foreach (var cargo in cargosVencidos)
            {
                cargo.TN_IdEstadoCargo = (int)EstadoCargoEnum.Vencido;

                if (cargo.TB_RecargoProgramado
                    && !cargo.TB_RecargoAplicado
                    && cargo.TN_IdTipoRecargo.HasValue
                    && cargo.TN_ValorRecargo.HasValue)
                {
                    var tipoRecargo = await _context.TiposRecargo
                        .FirstAsync(t => t.TN_Id == cargo.TN_IdTipoRecargo.Value);

                    var montoAplicado = tipoRecargo.TC_Nombre == "Porcentaje"
                        ? Math.Round(cargo.TN_MontoTotal * (cargo.TN_ValorRecargo.Value / 100m), 2)
                        : cargo.TN_ValorRecargo.Value;

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
                    cargo.TB_RecargoAplicado = true;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}