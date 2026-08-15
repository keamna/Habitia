using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Habitia.ViewModels.Mantenimiento
{
    public class MantenimientoCreateViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar una incidencia.")]
        public int IdIncidencia { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un tipo de mantenimiento.")]
        [Display(Name = "Tipo de mantenimiento")]
        public int? IdTipoMantenimiento { get; set; }
        [Required(ErrorMessage = "Debe asignar al menos una persona de mantenimiento.")]
        [Display(Name = "Personal asignado")]
        public string IdPersonalAsignado { get; set; }
        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "La descripción debe tener entre {2} y {1} caracteres.")]
        [Display(Name = "Descripción de la tarea")]
        public string Descripcion { get; set; }
        public List<SelectListItem>? IncidenciasElegibles { get; set; }
        public List<SelectListItem>? TiposMantenimiento { get; set; }
        public List<SelectListItem>? PersonalMantenimiento { get; set; }
    }
}