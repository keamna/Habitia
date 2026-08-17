// /ViewModels/Vivienda/RegistrarViviendaViewModel.cs
using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels.Vivienda
{
    public class RegistrarViviendaViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar el tipo de vivienda.")]
        public TipoViviendaEnum? TipoVivienda { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una vivienda.")]
        public int? IdVivienda { get; set; }
    }
}