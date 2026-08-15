using Habitia.Data;
using Habitia.Enums;
using Habitia.ViewModels.Reserva;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Helpers
{
    // Lógica compartida entre los ReservasController de Residente, Admin y Seguridad.
    public static class ReservaHelper
    {
        // Marca como "Finalizada" cualquier reserva Activa cuyo horario ya terminó.
        public static async Task FinalizarReservasVencidasAsync(ApplicationDbContext context)
        {
            var ahora = DateTime.Now;
            var activas = await context.Reservas
                .Include(r => r.Disponibilidad)
                .Where(r => r.TN_Estado == EstadoReservaEnum.Activa)
                .ToListAsync();
            bool huboCambios = false;
            foreach (var r in activas)
            {
                var fin = r.Disponibilidad.TF_Fecha.Date.Add(r.Disponibilidad.TF_HoraFin);
                if (fin <= ahora)
                {
                    r.TN_Estado = EstadoReservaEnum.Finalizada;
                    huboCambios = true;
                }
            }
            if (huboCambios)
                await context.SaveChangesAsync();
        }

        // Convierte una Reserva (entidad) en ReservaViewModel (para las vistas).
        // mostrarDatosResidente: incluye datos del residente (Admin y Seguridad, 5.3.3).
        // esAdmin: habilita cancelar sin restricción de anticipación (solo Admin, 5.4.4). Seguridad nunca cancela.
        public static ReservaViewModel MapToVM(Habitia.Models.Reserva r, bool mostrarDatosResidente, bool esAdmin)
        {
            var inicio = r.Disponibilidad.TF_Fecha.Date.Add(r.Disponibilidad.TF_HoraInicio);

            // La anticipación mínima ahora vive en el horario, no en el área común.
            var minutosAnticipacion = r.Disponibilidad.TN_AnticipacionMinima;

            bool puedeCancelar;
            if (esAdmin)
            {
                // El administrador puede cancelar cualquier reserva Activa, sin restricción de tiempo (5.4)
                puedeCancelar = r.TN_Estado == EstadoReservaEnum.Activa;
            }
            else
            {
                // El residente solo puede cancelar si aún se cumple la anticipación mínima (4.3)
                puedeCancelar = r.TN_Estado == EstadoReservaEnum.Activa
                    && DateTime.Now.AddMinutes(minutosAnticipacion) <= inicio;
            }
            // Fotos del área común reservada, activas y ordenadas (la principal primero).
            var fotos = r.Disponibilidad.AreaComun.Fotos == null
                ? new List<string>()
                : r.Disponibilidad.AreaComun.Fotos
                    .Where(f => f.TB_Estado)
                    .OrderByDescending(f => f.TB_EsPrincipal)
                    .ThenBy(f => f.TN_Orden)
                    .Select(f => f.TC_Url)
                    .ToList();
            return new ReservaViewModel
            {
                Id = r.TN_Id,
                IdUsuario = r.TC_IdUsuario,
                IdVivienda = r.TN_IdVivienda,
                IdDisponibilidad = r.TN_IdDisponibilidad,
                Cantidad = r.TN_Cantidad,
                Motivo = r.TC_Motivo,
                Estado = r.TN_Estado,
                FechaRegistro = r.TF_FechaRegistro,
                FechaCancelacion = r.TF_FechaCancelacion,
                NombreAreaComun = r.Disponibilidad.AreaComun.TC_Nombre,
                FechaHorario = r.Disponibilidad.TF_Fecha,
                HoraInicio = r.Disponibilidad.TF_HoraInicio,
                HoraFin = r.Disponibilidad.TF_HoraFin,
                FotosAreaComun = fotos,
                NombreResidente = mostrarDatosResidente ? $"{r.Usuario.TC_Nombre} {r.Usuario.TC_Apellido}" : null,
                NumeroVivienda = mostrarDatosResidente ? r.Vivienda?.TC_Numero : null,
                CorreoResidente = mostrarDatosResidente ? r.Usuario.Email : null,
                TelefonoResidente = mostrarDatosResidente ? r.Usuario.TC_Telefono : null,
                IdentificacionResidente = mostrarDatosResidente ? r.Usuario.TC_Identificacion : null,
                TipoVivienda = mostrarDatosResidente ? r.Vivienda?.TN_Tipo.ToString() : null,
                PuedeCancelar = puedeCancelar
            };
        }
    }
}