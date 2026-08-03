using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Asamblea
{
    public class ParticipanteAsambleaViewModel
    {
        public int Id { get; set; }


        [Required]
        public int IdAsamblea { get; set; }


        [Required]
        public string IdUsuario { get; set; }


        public bool Confirmado { get; set; }


        public bool Asistio { get; set; }


        public DateTime FechaRegistro { get; set; }
    }
}