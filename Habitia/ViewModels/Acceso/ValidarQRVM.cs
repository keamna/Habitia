using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Acceso
{
    public class ValidarQRVM
    {
        [Required(ErrorMessage = "Debe ingresar o escanear un código")]
        public string Codigo { get; set; }
    }
}