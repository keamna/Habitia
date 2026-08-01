// /Helpers/ViviendaHelper.cs
using Habitia.Enums;

namespace Habitia.Helpers
{
    public static class ViviendaHelper
    {
        public static string ObtenerEtiquetaOcupantes(bool? propietarioViveAhi)
        {
            return propietarioViveAhi == true ? "Familiares" : "Inquilinos";
        }

        // Disponible <-> Ocupada se recalculan solos según si hay usuarios activos.
        // Inactiva NUNCA se pisa automáticamente: es decisión manual del Admin
        // (mantenimiento, baja temporal, etc.), así que esta función no la toca.
        public static EstadoViviendaEnum RecalcularEstado(
            EstadoViviendaEnum estadoActual,
            int cantidadUsuariosActivos)
        {
            if (estadoActual == EstadoViviendaEnum.Inactiva)
            {
                return EstadoViviendaEnum.Inactiva;
            }

            return cantidadUsuariosActivos > 0
                ? EstadoViviendaEnum.Ocupada
                : EstadoViviendaEnum.Disponible;
        }
    }
}