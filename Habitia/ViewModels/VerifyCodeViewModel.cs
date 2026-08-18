using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels
{
    public class VerifyCodeViewModel
    {
        [Required(ErrorMessage = "Ingresa el código de verificación.")]
        public string Codigo { get; set; }

        public bool RememberMe { get; set; }
    }
}