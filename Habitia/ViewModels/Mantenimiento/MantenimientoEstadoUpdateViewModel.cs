using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels.Mantenimiento
{
    public class MantenimientoEstadoUpdateViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un estado.")]
        [Display(Name = "Estado")]
        public EstadoMantenimientoEnum Estado { get; set; }

        [StringLength(1000, ErrorMessage = "Las observaciones no pueden superar los {1} caracteres.")]
        [Display(Name = "Observaciones / Resultados")]
        public string? Observaciones { get; set; }
    }
}