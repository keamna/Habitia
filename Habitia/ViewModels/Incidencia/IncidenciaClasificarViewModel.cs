using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels.Incidencias
{
    public class IncidenciaClasificarViewModel
    {
        [Required]
        public int Id { get; set; }

        // Datos de solo lectura para mostrar contexto en la vista de clasificación
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string NombreUsuarioReporta { get; set; }
        public DateTime FechaRegistro { get; set; }

        [Required(ErrorMessage = "Debe clasificar el tipo de responsabilidad.")]
        [Display(Name = "Tipo de responsabilidad")]
        public ResponsabilidadEnum Responsabilidad { get; set; }
    }
}