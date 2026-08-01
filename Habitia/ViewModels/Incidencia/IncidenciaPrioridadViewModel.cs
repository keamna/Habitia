using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels.Incidencias
{
    public class IncidenciaPrioridadViewModel
    {
        [Required]
        public int Id { get; set; }

        // Datos de solo lectura para mostrar contexto en la vista
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string NombreUsuarioReporta { get; set; }
        public DateTime FechaRegistro { get; set; }
        public ResponsabilidadEnum Responsabilidad { get; set; }

        [Required(ErrorMessage = "Debe asignar la prioridad antes de continuar.")]
        [Display(Name = "Prioridad")]
        public PrioridadEnum Prioridad { get; set; }
    }
}