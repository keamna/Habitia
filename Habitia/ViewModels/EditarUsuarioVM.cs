using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels
{
    public class EditarUsuarioVM
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(20)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [MaxLength(50)]
        public string Email { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [MaxLength(20)]
        public string Telefono { get; set; }
    }
}