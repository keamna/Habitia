using System.ComponentModel.DataAnnotations;
using Habitia.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Habitia.ViewModels.Incidencias
{
    public class IncidenciaPrioridadViewModel
    {
        [Required]
        public int Id { get; set; }

        // Estos campos son solo de lectura en esta pantalla: se cargan desde
        // la BD (GET, o al reconstruir el modelo tras un POST inválido) y
        // nunca vienen del formulario. [ValidateNever] evita que el binder
        // los marque como requeridos cuando el POST no los incluye.
        [ValidateNever]
        public string Titulo { get; set; }

        [ValidateNever]
        public string Descripcion { get; set; }

        [ValidateNever]
        public string NombreCompletoReporta { get; set; }

        public string? IdentificacionReporta { get; set; }

        [ValidateNever]
        public DateTime FechaRegistro { get; set; }

        [ValidateNever]
        public ResponsabilidadEnum Responsabilidad { get; set; }

        [Required(ErrorMessage = "Debe asignar la prioridad antes de continuar.")]
        [Display(Name = "Prioridad")]
        public PrioridadEnum? Prioridad { get; set; }
    }
}