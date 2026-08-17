using System.ComponentModel.DataAnnotations;

namespace Habitia.Enums
{
    public enum EstadoIncidenciaEnum
    {
        [Display(Name = "Pendiente")]
        Pendiente = 1,

        [Display(Name = "En proceso")]
        EnProceso = 2,

        [Display(Name = "Resuelta")]
        Resuelta = 3
    }
}