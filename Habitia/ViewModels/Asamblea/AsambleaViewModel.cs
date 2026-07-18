using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels.Asamblea
{
    public class AsambleaViewModel
    {
        public int Id { get; set; }


        [Required]
        [MaxLength(150)]
        public string Titulo { get; set; }


        [Required]
        public DateTime FechaHora { get; set; }


        [Required]
        public int IdModalidad { get; set; }


        [MaxLength(500)]
        public string Descripcion { get; set; }


        public EstadoAsambleaEnum Estado { get; set; }


        public DateTime FechaRegistro { get; set; }
    }
}