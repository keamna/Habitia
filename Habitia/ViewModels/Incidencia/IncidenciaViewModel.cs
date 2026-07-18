using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels.Incidencia
{
    public class IncidenciaViewModel
    {
        public int Id { get; set; }


        [Required]
        public string IdUsuario { get; set; }


        // Puede estar asociada a una vivienda o área común

        public int? IdVivienda { get; set; }


        public int? IdAreaComun { get; set; }


        [Required]
        [MaxLength(500)]
        public string Descripcion { get; set; }


        [Required]
        public int IdTipoResponsabilidad { get; set; }


        public EstadoIncidenciaEnum Estado { get; set; }


        [MaxLength(300)]
        public string Evidencia { get; set; }


        public DateTime FechaRegistro { get; set; }
    }
}