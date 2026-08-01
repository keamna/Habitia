using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Habitia.ViewModels.Vivienda
{
    public class RegistrarViviendaViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar una vivienda.")]
        public int IdVivienda { get; set; }

        public List<SelectListItem> ViviendasDisponibles { get; set; } = new();
    }
}