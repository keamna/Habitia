using Habitia.Data;
using Habitia.Enums;
using Habitia.Models.Acceso;
using Habitia.ViewModels.Acceso;
using Habitia.ViewModels.Autorizacion;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Helpers
{
    // Lógica compartida entre AutorizacionesController (Residente/Admin)
    // y AccesosController (Seguridad/Admin).
    public static class AccesoHelper
    {
        // Genera un código único para el QR. Reintenta por si acaso choca (extremadamente improbable).
        public static async Task<string> GenerarCodigoUnicoAsync(ApplicationDbContext context)
        {
            string codigo;
            bool existe;

            do
            {
                codigo = "AUTH-" + Guid.NewGuid().ToString("N")[..12].ToUpper();
                existe = await context.Autorizaciones.AnyAsync(a => a.TC_Codigo == codigo);
            }
            while (existe);

            return codigo;
        }

        // Marca como "Expirada" cualquier autorización Pendiente cuya fecha de vencimiento ya pasó.
        public static async Task ExpirarAutorizacionesVencidasAsync(ApplicationDbContext context)
        {
            var ahora = DateTime.Now;

            var pendientesVencidas = await context.Autorizaciones
                .Where(a => a.TN_Estado == EstadoAutorizacionEnum.Pendiente
                         && a.TF_FechaVencimiento <= ahora)
                .ToListAsync();

            if (pendientesVencidas.Count == 0)
                return;

            foreach (var a in pendientesVencidas)
                a.TN_Estado = EstadoAutorizacionEnum.Expirada;

            await context.SaveChangesAsync();
        }

        // Búsqueda combinada de residente + vivienda (por nombre del residente o número de vivienda).
        // Cada resultado ya trae el par listo (IdUsuario + IdVivienda) para el registro manual (US-04, punto 2).
        public static async Task<List<object>> BuscarResidentesAsync(ApplicationDbContext context, string termino)
        {
            if (string.IsNullOrWhiteSpace(termino) || termino.Length < 2)
                return new List<object>();

            var t = termino.Trim().ToLower();

            var query =
                from vu in context.ViviendaUsuarios
                join u in context.Users on vu.TC_IdUsuario equals u.Id
                join v in context.Viviendas on vu.TN_IdVivienda equals v.TN_Id
                where vu.TB_ViveAhi
                    && ((u.TC_Nombre + " " + u.TC_Apellido).ToLower().Contains(t)
                        || v.TC_Numero.ToLower().Contains(t)
                        || u.TC_Identificacion.ToLower().Contains(t))
                select new
                {
                    idUsuario = u.Id,
                    idVivienda = v.TN_Id,
                    nombreResidente = u.TC_Nombre + " " + u.TC_Apellido,
                    identificacionResidente = u.TC_Identificacion,
                    numeroVivienda = v.TC_Numero
                };

            var resultados = await query
                .OrderBy(r => r.numeroVivienda)
                .Take(10)
                .ToListAsync();

            return resultados.Cast<object>().ToList();
        }

        // Búsqueda de visitantes activos por nombre o identificación, para reutilizar datos (US-05, punto 2).
        public static async Task<List<object>> BuscarVisitantesAsync(ApplicationDbContext context, string termino)
        {
            if (string.IsNullOrWhiteSpace(termino) || termino.Length < 2)
                return new List<object>();

            var t = termino.Trim().ToLower();

            return await context.Visitantes
                .Where(v => v.TB_Estado &&
                    (v.TC_Nombre.ToLower().Contains(t) || v.TC_Identificacion.ToLower().Contains(t)))
                .OrderBy(v => v.TC_Nombre)
                .Take(10)
                .Select(v => new
                {
                    id = v.TN_Id,
                    nombre = v.TC_Nombre,
                    identificacion = v.TC_Identificacion,
                    tipoIdentificacion = v.TN_TipoIdentificacion,
                    telefono = v.TC_Telefono
                })
                .Cast<object>()
                .ToListAsync();
        }

        public static AccesoViewModel MapAccesoToVM(Acceso a)
        {
            return new AccesoViewModel
            {
                Id = a.TN_Id,
                NombreVisitante = a.Visitante.TC_Nombre,
                IdentificacionVisitante = a.Visitante.TC_Identificacion,
                NombreResidente = $"{a.Usuario.TC_Nombre} {a.Usuario.TC_Apellido}",
                NumeroVivienda = a.Vivienda.TC_Numero,
                Motivo = a.TC_Motivo,
                FechaIngreso = a.TF_FechaIngreso,
                FechaSalida = a.TF_FechaSalida,
                OrigenAcceso = a.TN_IdAutorizacion.HasValue ? "QR" : "Manual",
                PlacaVehiculo = a.Vehiculo?.TC_Placa,
                TipoVehiculo = a.Vehiculo?.TC_Tipo,
                ObservacionesVehiculo = a.Vehiculo?.TC_Observaciones
            };
        }

        // mostrarDatosResidente: Admin (5.3.3 pide ver residente/vivienda en la consulta).
        // puedeInvalidar: solo Admin, y solo si la autorización sigue Pendiente (aún no se usó).
        public static AutorizacionViewModel MapAutorizacionToVM(
            Autorizacion a,
            bool mostrarDatosResidente,
            bool puedeInvalidar)
        {
            return new AutorizacionViewModel
            {
                Id = a.TN_Id,
                Codigo = a.TC_Codigo,
                NombreVisitante = a.Visitante.TC_Nombre,
                IdentificacionVisitante = a.Visitante.TC_Identificacion,
                FechaVisita = a.TF_FechaVisita,
                FechaVencimiento = a.TF_FechaVencimiento,
                FechaRegistro = a.TF_FechaRegistro,
                Motivo = a.TC_Motivo,
                Estado = a.TN_Estado,
                PuedeInvalidar = puedeInvalidar && a.TN_Estado == EstadoAutorizacionEnum.Pendiente,
                NombreResidente = mostrarDatosResidente ? $"{a.Usuario.TC_Nombre} {a.Usuario.TC_Apellido}" : null,
                NumeroVivienda = mostrarDatosResidente ? a.Vivienda.TC_Numero : null
            };
        }
    }
}