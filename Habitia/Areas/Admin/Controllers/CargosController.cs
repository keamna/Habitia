using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Habitia.Data;
using Habitia.Models;
using Habitia.Models.Financiero;
using Habitia.ViewModels.Financiero.Admin;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CargosController : Controller
    {
        private const decimal PORCENTAJE_IVA = 0.13m;
        private const string ESTADO_PENDIENTE = "Pendiente";
        private readonly ApplicationDbContext _context;

        public CargosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string estado)
        {
            var cargos = await _context.Cargos
                .Include(c => c.Residente)
                .Include(c => c.TipoCargo)
                .Include(c => c.EstadoCargo)
                .Include(c => c.Vivienda)
                .Include(c => c.TipoRecargo) // <-- NUEVO: para mostrar el desglose del recargo
                .Where(c => c.TB_Estado)
                .OrderByDescending(c => c.TF_FechaEmision)
                .Select(c => new CargoListItemViewModel
                {
                    TN_Id = c.TN_Id,
                    ResidenteId = c.Residente.Id,
                    NombreResidente = c.Residente.TC_Nombre + " " + c.Residente.TC_Apellido,
                    IdentificacionResidente = c.Residente.TC_Identificacion,
                    CorreoResidente = c.Residente.Email,
                    TelefonoResidente = c.Residente.TC_Telefono,
                    TipoCargo = c.TipoCargo.TC_Nombre,
                    TN_MontoBase = c.TN_MontoBase,
                    TB_AplicaIva = c.TB_AplicaIva,
                    TN_MontoIva = c.TN_MontoIva,
                    TN_MontoTotal = c.TN_MontoTotal,
                    TF_FechaEmision = c.TF_FechaEmision,
                    TF_FechaVencimiento = c.TF_FechaVencimiento,
                    EstadoCargo = c.EstadoCargo.TC_Nombre,
                    TB_RecargoAplicado = c.TB_RecargoAplicado,
                    TB_RecargoProgramado = c.TB_RecargoProgramado,
                    NumeroVivienda = c.Vivienda != null ? c.Vivienda.TC_Numero : null,
                    NombreTipoRecargo = c.TipoRecargo != null ? c.TipoRecargo.TC_Nombre : null,
                    TC_FrecuenciaRecargo = c.TC_FrecuenciaRecargo,
                    TN_ValorRecargo = c.TN_ValorRecargo,
                    TC_IdLote = c.TC_IdLote // <-- NUEVO: agrupamiento confiable en el listado
                })
                .ToListAsync();

            ViewData["EstadoInicial"] = estado;

            return View(cargos);
        }

        // ============ CREATE ============

        public async Task<IActionResult> Create()
        {
            var vm = new CargoCreateViewModel
            {
                TiposCargo = await ObtenerTiposCargo(),
                Residentes = await ObtenerResidentesBusqueda(),
                TiposRecargo = await ObtenerTiposRecargo(),
                ViviendasResidentes = await ObtenerViviendasResidentesActivas() // <-- NUEVO
            };
            return View(vm);
        }

        // NUEVO: AJAX — devuelve las viviendas activas de un residente específico,
        // usado por el modo individual cuando el residente elegido tiene 2+ viviendas.
        [HttpGet]
        public async Task<IActionResult> ObtenerViviendasDeResidente(string idResidente)
        {
            if (string.IsNullOrWhiteSpace(idResidente))
                return Json(new List<ViviendaSimpleViewModel>());

            // IMPORTANTE: el Distinct() debe aplicarse sobre un tipo cuya igualdad se compare
            // por VALOR (tipo anónimo), no sobre el ViewModel directamente. Si se hiciera
            // .Distinct() después de proyectar a ViviendaSimpleViewModel, no eliminaría
            // duplicados (comparación por referencia) y podrían llegar 2 checkboxes con el
            // mismo id/valor al front, rompiendo la selección individual.
            var viviendas = await _context.ViviendaUsuarios
                .Include(vu => vu.Vivienda)
                .Where(vu => vu.TC_IdUsuario == idResidente
                             && vu.TN_Estado == EstadoUsuarioEnum.Activo)
                .Select(vu => new { vu.Vivienda.TN_Id, vu.Vivienda.TC_Numero })
                .Distinct()
                .OrderBy(v => v.TC_Numero)
                .Select(v => new ViviendaSimpleViewModel
                {
                    TN_Id = v.TN_Id,
                    TC_Numero = v.TC_Numero
                })
                .ToListAsync();

            return Json(viviendas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CargoCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.TiposCargo = await ObtenerTiposCargo();
                vm.Residentes = await ObtenerResidentesBusqueda();
                vm.TiposRecargo = await ObtenerTiposRecargo();
                vm.ViviendasResidentes = await ObtenerViviendasResidentesActivas(); // <-- NUEVO

                // IMPORTANTE: igual que en Edit, el campo visible del residente no tiene "name"
                // (es solo texto en pantalla), así que si falla otro campo del formulario y se
                // vuelve a mostrar la vista, hay que reconstruir el texto para que no se vea
                // como si el residente elegido se hubiera perdido.
                if (!string.IsNullOrWhiteSpace(vm.TC_IdResidente))
                {
                    var residenteSeleccionado = await _context.Users
                        .FirstOrDefaultAsync(u => u.Id == vm.TC_IdResidente);

                    if (residenteSeleccionado != null)
                    {
                        ViewData["ResidenteDisplaySeleccionado"] =
                            $"{residenteSeleccionado.TC_Identificacion} - {residenteSeleccionado.TC_Nombre} {residenteSeleccionado.TC_Apellido}";
                    }
                }

                return View(vm);
            }

            try
            {
                var tipoCargo = await ObtenerOCrearTipoCargo(vm.TC_TipoCargoTexto);

                var montoBase = vm.TN_MontoBase!.Value;
                var montoIva = vm.TB_AplicaIva ? Math.Round(montoBase * PORCENTAJE_IVA, 2) : 0m;
                var montoTotal = montoBase + montoIva;

                var estadoPendiente = await _context.EstadosCargo
                    .FirstAsync(e => e.TC_Nombre == ESTADO_PENDIENTE);

                // --- NUEVO: lista de pares (residente, vivienda) destino ---
                var paresDestino = new List<(string IdResidente, int? IdVivienda)>();

                if (vm.TB_AplicarATodos)
                {
                    // Modo masivo: cada checkbox marcado es un vínculo THBT_A_ViviendaUsuario
                    var vinculos = await _context.ViviendaUsuarios
                        .Where(vu => vm.ViviendaUsuarioSeleccionados.Contains(vu.TN_Id))
                        .Select(vu => new { vu.TC_IdUsuario, vu.TN_IdVivienda })
                        .ToListAsync();

                    paresDestino.AddRange(vinculos.Select(v => (v.TC_IdUsuario, (int?)v.TN_IdVivienda)));
                }
                else
                {
                    // Modo individual: un residente, 1 o varias viviendas
                    var idResidente = vm.TC_IdResidente!;
                    var viviendasDelResidente = await _context.ViviendaUsuarios
                        .Where(vu => vu.TC_IdUsuario == idResidente
                                     && vu.TN_Estado == EstadoUsuarioEnum.Activo)
                        .Select(vu => vu.TN_IdVivienda)
                        .Distinct()
                        .ToListAsync();

                    if (viviendasDelResidente.Count <= 1)
                    {
                        // Sin vivienda registrada o solo una: se asigna automáticamente (o null si no tiene)
                        paresDestino.Add((idResidente, viviendasDelResidente.FirstOrDefault() is int v && v != 0 ? v : (int?)null));
                    }
                    else if (vm.ViviendasSeleccionadas != null && vm.ViviendasSeleccionadas.Count > 0)
                    {
                        // Tiene varias: usa las que el admin seleccionó (todas por defecto en el front)
                        foreach (var idVivienda in vm.ViviendasSeleccionadas.Distinct())
                        {
                            if (viviendasDelResidente.Contains(idVivienda))
                                paresDestino.Add((idResidente, idVivienda));
                        }
                    }
                    else
                    {
                        // Salvaguarda: si por algún motivo no llegó ninguna selección, aplica a todas
                        paresDestino.AddRange(viviendasDelResidente.Select(v => (idResidente, (int?)v)));
                    }
                }

                // IMPORTANTE: se captura UNA sola vez para que todos los cargos generados en esta
                // misma operación (mismo residente, varias viviendas) compartan el mismo timestamp
                // exacto.
                var fechaEmision = DateTime.Now;

                // NUEVO: identificador de lote explícito. Es lo único que se usa para agrupar
                // cargos en el listado — no depende de que los montos/fechas/residente coincidan,
                // así que editar un cargo individual después nunca puede fusionarlo con otro
                // por casualidad ni separarlo de su lote original.
                var idLote = Guid.NewGuid().ToString();

                foreach (var (idResidente, idVivienda) in paresDestino)
                {
                    var cargo = new THBT_A_Cargo
                    {
                        TC_IdResidente = idResidente,
                        TN_IdVivienda = idVivienda, // <-- NUEVO
                        TN_IdTipoCargo = tipoCargo.TN_Id,
                        TN_IdEstadoCargo = estadoPendiente.TN_Id,
                        TC_Descripcion = vm.TC_Descripcion,
                        TN_MontoBase = montoBase,
                        TB_AplicaIva = vm.TB_AplicaIva,
                        TN_MontoIva = montoIva,
                        TN_MontoTotal = montoTotal,
                        TF_FechaEmision = fechaEmision,
                        TF_FechaVencimiento = vm.TF_FechaVencimiento!.Value,
                        TB_Estado = true,
                        TC_IdLote = idLote, // <-- NUEVO

                        TB_RecargoProgramado = vm.TB_RecargoProgramado,
                        TN_IdTipoRecargo = vm.TB_RecargoProgramado ? vm.TN_IdTipoRecargo : null,
                        TN_ValorRecargo = vm.TB_RecargoProgramado ? vm.TN_ValorRecargo : null,
                        TC_FrecuenciaRecargo = vm.TB_RecargoProgramado ? vm.TC_FrecuenciaRecargo : null
                    };

                    _context.Cargos.Add(cargo);
                }

                await _context.SaveChangesAsync();

                TempData["Mensaje"] = paresDestino.Count > 1
                    ? $"Se crearon {paresDestino.Count} cargos correctamente"
                    : "Cargo creado correctamente";

                // NUEVO: datos para el modal de confirmación en Index
                TempData["CargoCreado"] = true;
                TempData["TipoCargoResumen"] = tipoCargo.TC_Nombre;
                TempData["MontoTotalResumen"] = montoTotal.ToString("C");
                TempData["FechaVencimientoResumen"] = vm.TF_FechaVencimiento!.Value.ToString("dd/MM/yyyy");
                TempData["CantidadCargosResumen"] = paresDestino.Count;
            }
            catch
            {
                TempData["Error"] = "No fue posible completar la operación";
            }

            return RedirectToAction(nameof(Index));
        }

        // ============ DELETE (lógico) ============

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var cargo = await _context.Cargos
                .Include(c => c.EstadoCargo)
                .FirstOrDefaultAsync(c => c.TN_Id == id && c.TB_Estado);

            if (cargo == null)
            {
                TempData["Error"] = "El cargo no existe.";
                return RedirectToAction(nameof(Index));
            }

            if (cargo.EstadoCargo.TC_Nombre != ESTADO_PENDIENTE)
            {
                TempData["Error"] = $"No se puede eliminar un cargo en estado \"{cargo.EstadoCargo.TC_Nombre}\".";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                cargo.TB_Estado = false;
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Cargo eliminado correctamente";
            }
            catch
            {
                TempData["Error"] = "No fue posible completar la operación";
            }

            return RedirectToAction(nameof(Index));
        }

        // ============ Helpers ============

        private async Task<THBT_CAT_TipoCargo> ObtenerOCrearTipoCargo(string texto)
        {
            var tipoTexto = texto.Trim();
            var tipoCargo = await _context.TiposCargo
                .FirstOrDefaultAsync(t => t.TC_Nombre.ToLower() == tipoTexto.ToLower());

            if (tipoCargo == null)
            {
                tipoCargo = new THBT_CAT_TipoCargo { TC_Nombre = tipoTexto };
                _context.TiposCargo.Add(tipoCargo);
                await _context.SaveChangesAsync();
            }

            return tipoCargo;
        }

        private async Task<List<SelectListItem>> ObtenerTiposCargo()
        {
            return await _context.TiposCargo
                .OrderBy(t => t.TC_Nombre)
                .Select(t => new SelectListItem { Value = t.TC_Nombre, Text = t.TC_Nombre })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> ObtenerTiposRecargo()
        {
            return await _context.TiposRecargo
                .Select(t => new SelectListItem { Value = t.TN_Id.ToString(), Text = t.TC_Nombre })
                .ToListAsync();
        }

        private async Task<List<ResidenteBusquedaViewModel>> ObtenerResidentesBusqueda()
        {
            return await _context.Users
                .Where(u => _context.UserRoles
                    .Any(ur => ur.UserId == u.Id &&
                        _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Residente")))
                .Select(u => new ResidenteBusquedaViewModel
                {
                    Id = u.Id,
                    Nombre = u.TC_Nombre + " " + u.TC_Apellido,
                    Identificacion = u.TC_Identificacion
                })
                .ToListAsync();
        }

        // pares vivienda-residente activos, para la tabla del modo masivo
        private async Task<List<ViviendaResidenteViewModel>> ObtenerViviendasResidentesActivas()
        {
            return await _context.ViviendaUsuarios
                .Include(vu => vu.Vivienda)
                .Include(vu => vu.Usuario)
                .Where(vu => vu.TN_Estado == EstadoUsuarioEnum.Activo
                             && _context.UserRoles.Any(ur => ur.UserId == vu.TC_IdUsuario &&
                                 _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Residente")))
                .OrderBy(vu => vu.Vivienda.TC_Numero)
                .Select(vu => new ViviendaResidenteViewModel
                {
                    TN_IdViviendaUsuario = vu.TN_Id,
                    TN_IdVivienda = vu.TN_IdVivienda,
                    NumeroVivienda = vu.Vivienda.TC_Numero,
                    TC_IdResidente = vu.TC_IdUsuario,
                    NombreResidente = vu.Usuario.TC_Nombre + " " + vu.Usuario.TC_Apellido,
                    IdentificacionResidente = vu.Usuario.TC_Identificacion,
                    TipoRelacion = vu.TN_TipoRelacion.ToString()
                })
                .ToListAsync();
        }
    }
}