using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Catalogos
{
    public class TipoMantenimientoCreateViewModel
    {
        [Required(ErrorMessage = "El nombre del tipo de mantenimiento es obligatorio.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre {2} y {1} caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
    }
}