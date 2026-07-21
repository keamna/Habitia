using Habitia.Enums;
using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Usuario
{
    public class CrearUsuarioViewModel : IValidatableObject
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

        // Roles que el Admin puede asignar desde este formulario.
        // "Residente" se excluye a propósito: ese rol se asigna
        // automáticamente en UsuariosController.Aprobar() cuando
        // el Admin aprueba una solicitud vinculada a una vivienda.
        private static readonly string[] RolesPermitidos = { "Seguridad", "Mantenimiento" };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Roles == null || !Roles.Any())
            {
                yield return new ValidationResult(
                    "Debe seleccionar al menos un rol.",
                    new[] { nameof(Roles) });
                yield break;
            }

            var rolesNoPermitidos = Roles.Except(RolesPermitidos).ToList();

            if (rolesNoPermitidos.Any())
            {
                yield return new ValidationResult(
                    $"Desde este formulario solo se pueden asignar los roles Seguridad o Mantenimiento. " +
                    $"Rol(es) no permitido(s): {string.Join(", ", rolesNoPermitidos)}.",
                    new[] { nameof(Roles) });
            }
        }
    }
}