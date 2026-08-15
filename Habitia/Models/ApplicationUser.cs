using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string TC_Nombre { get; set; }

        [Required]
        [MaxLength(100)]
        public string TC_Apellido { get; set; }

        public TipoIdentificacionEnum TN_TipoIdentificacion { get; set; }

        [Required]
        [MaxLength(20)]
        public string TC_Identificacion { get; set; }

        // Nullable: hay residentes sin teléfono registrado en la base de datos.
        [MaxLength(20)]
        public string? TC_Telefono { get; set; }


        public EstadoUsuarioEnum TN_Estado { get; set; }
        public DateTime TF_FechaRegistro { get; set; }

        // ===== MARKETPLACE =====
        [MaxLength(300)]
        public string? TC_FotoPerfil { get; set; }

        // Relaciones
        public ICollection<ViviendaUsuario> Viviendas { get; set; }
            = new List<ViviendaUsuario>();
        public ICollection<Reserva> Reservas { get; set; }
            = new List<Reserva>();
        public ICollection<Incidencia> Incidencias { get; set; }
            = new List<Incidencia>();
    }
}