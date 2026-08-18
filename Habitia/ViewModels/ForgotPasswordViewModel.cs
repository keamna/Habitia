using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Ingrese su correo")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
        public string Email { get; set; }
    }
}