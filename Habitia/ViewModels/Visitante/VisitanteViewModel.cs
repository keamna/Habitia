using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels.Visitante
{
    public class VisitanteViewModel
    {
        public int Id { get; set; }


        [Required]
        [MaxLength(20)]
        public string Identificacion { get; set; }


        [Required]
        public TipoIdentificacionEnum TipoIdentificacion { get; set; }


        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }


        [MaxLength(20)]
        public string Telefono { get; set; }


        public DateTime FechaRegistro { get; set; }
    }
}