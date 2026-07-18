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


        public bool Asistencia { get; set; }


        public DateTime FechaRegistro { get; set; }
    }
}