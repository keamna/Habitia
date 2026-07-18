using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels.Mantenimiento
{
    public class MantenimientoViewModel
    {
        public int Id { get; set; }


        [Required]
        public int IdIncidencia { get; set; }


        [Required]
        public int IdTipoMantenimiento { get; set; }


        // Personal asignado desde Identity

        public string IdUsuario { get; set; }


        [Required]
        public DateTime FechaProgramada { get; set; }


        public EstadoMantenimientoEnum Estado { get; set; }


        [MaxLength(500)]
        public string Observaciones { get; set; }


        public DateTime FechaRegistro { get; set; }
    }
}