using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Usuario
{
    public class MiPerfilViewModel
    {
        // ── Solo lectura ──
        public string TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string Rol { get; set; }
        public string? FotoPerfil { get; set; }
        public List<ViviendaResumenViewModel> Viviendas { get; set; } = new();

        // ── Editable ──
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres.")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido.")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
        [Phone(ErrorMessage = "Debe ingresar un número de teléfono válido.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }
    }
}