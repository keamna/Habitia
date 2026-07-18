using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels.Autorizacion
{
    public class AutorizacionViewModel
    {

        public int Id { get; set; }


        [Required]
        public int IdVisitante { get; set; }


        [Required]
        public string IdUsuario { get; set; }


        [Required]
        public int IdVivienda { get; set; }


        [Required]
        [MaxLength(200)]
        public string Motivo { get; set; }


        [Required]
        public DateTime Inicio { get; set; }


        [Required]
        public DateTime Fin { get; set; }


        public EstadoAutorizacionEnum Estado { get; set; }


        public string Codigo { get; set; }


        public DateTime FechaRegistro { get; set; }
    }
}