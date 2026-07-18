using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.Models
{
    public class ApplicationUser : IdentityUser
    {

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }



        [Required]
        [MaxLength(100)]
        public string Apellido { get; set; }



        public TipoIdentificacionEnum TipoIdentificacion { get; set; }



        [Required]
        [MaxLength(20)]
        public string Identificacion { get; set; }



        [MaxLength(20)]
        public string Telefono { get; set; }



        public EstadoUsuarioEnum Estado { get; set; }



        public DateTime FechaRegistro { get; set; }




        // Relaciones

        public ICollection<ViviendaUsuario> Viviendas { get; set; }
            = new List<ViviendaUsuario>();



        public ICollection<Reserva> Reservas { get; set; }
            = new List<Reserva>();



        public ICollection<Publicacion> Publicaciones { get; set; }
            = new List<Publicacion>();



        public ICollection<Incidencia> Incidencias { get; set; }
            = new List<Incidencia>();

    }
}