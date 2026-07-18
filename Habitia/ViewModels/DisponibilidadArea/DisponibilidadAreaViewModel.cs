using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.DisponibilidadArea
{
    public class DisponibilidadAreaViewModel
    {
        public int Id { get; set; }


        [Required]
        public int IdAreaComun { get; set; }


        [Required]
        public TimeSpan Inicio { get; set; }


        [Required]
        public TimeSpan Fin { get; set; }


        public bool Estado { get; set; }


        public DateTime FechaRegistro { get; set; }
    }
}