using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Acceso
{
    public class ValidarCodigoVM
    {
        [Required(ErrorMessage = "Debe ingresar el código")]
        public string Codigo { get; set; }
    }
}