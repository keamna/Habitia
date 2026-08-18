using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Usuario
{
    public class SolicitarCambioEmailViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
        public string EmailNuevo { get; set; }
    }
}