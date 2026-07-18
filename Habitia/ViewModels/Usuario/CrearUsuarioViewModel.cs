using Habitia.Enums;
using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Usuario
{
    public class CrearUsuarioViewModel
    {

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }



        [Required]
        [MaxLength(100)]
        public string Apellido { get; set; }



        [Required]
        public TipoIdentificacionEnum TipoIdentificacion { get; set; }



        [Required]
        [MaxLength(20)]
        public string Identificacion { get; set; }



        [Required]
        [EmailAddress]
        public string Email { get; set; }



        [Required]
        public string Telefono { get; set; }



        [Required]
        [MinLength(6)]
        public string Password { get; set; }



        [Required]
        public List<string> Roles { get; set; } = new();

    }
}