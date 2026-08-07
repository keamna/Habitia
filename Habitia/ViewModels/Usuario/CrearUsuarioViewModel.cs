using System.ComponentModel.DataAnnotations;
using Habitia.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Habitia.ViewModels.Usuario
{
    public class CrearUsuarioViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [MaxLength(100)]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido.")]
        [Display(Name = "Correo")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Debe seleccionar el tipo de identificación.")]
        [Display(Name = "Tipo de identificación")]
        public TipoIdentificacionEnum? TipoIdentificacion { get; set; }

        [Required(ErrorMessage = "La identificación es obligatoria.")]
        [MaxLength(20)]
        [Display(Name = "Identificación")]
        public string Identificacion { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [MaxLength(20)]
        [Phone]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol.")]
        [Display(Name = "Rol")]
        public string Rol { get; set; }

        // Solo obligatorio cuando Rol == "Mantenimiento" (se valida en el controller)
        [Display(Name = "Tipos de mantenimiento")]
        public List<int>? TiposMantenimientoIds { get; set; }

        public List<SelectListItem>? TiposMantenimientoDisponibles { get; set; }
    }
}