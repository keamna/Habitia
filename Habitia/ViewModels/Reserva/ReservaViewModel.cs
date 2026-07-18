using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels.Reserva
{
    public class ReservaViewModel
    {

        public int Id { get; set; }


        [Required]
        public string IdUsuario { get; set; }


        [Required]
        public int IdVivienda { get; set; }


        [Required]
        public int IdDisponibilidad { get; set; }


        [Required]
        [Range(1, 100)]
        public int Cantidad { get; set; }


        public EstadoReservaEnum Estado { get; set; }


        public DateTime FechaRegistro { get; set; }

    }
}