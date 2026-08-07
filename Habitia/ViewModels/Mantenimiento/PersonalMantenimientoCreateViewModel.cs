using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Habitia.ViewModels.Personal
{
    public class PersonalMantenimientoCreateViewModel
    {
        [Required(ErrorMessage = "La identificación es obligatoria.")]
        [Display(Name = "Cédula o pasaporte")]
        public string Identificacion { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [Phone]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Debe asignar al menos un tipo de mantenimiento.")]
        [Display(Name = "Tipos de mantenimiento")]
        public List<int> TiposMantenimientoIds { get; set; } = new();

        public List<SelectListItem>? TiposMantenimientoDisponibles { get; set; }
    }
}