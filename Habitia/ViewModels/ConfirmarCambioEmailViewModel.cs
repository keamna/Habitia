using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Usuario
{
    public class ConfirmarCambioEmailViewModel
    {
        [Required(ErrorMessage = "Ingrese el código")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "El código debe tener 6 dígitos")]
        public string Codigo { get; set; }
    }
}