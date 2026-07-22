using System.ComponentModel.DataAnnotations;

namespace Habitia.Enums
{
    public enum TipoViviendaEnum
    {
        [Display(Name = "Casa")]
        Casa = 1,

        [Display(Name = "Departamento")]
        Apartamento = 2
    }
}