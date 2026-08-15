using Habitia.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels
{
    public class AreaComunViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El código es obligatorio")]
        [StringLength(20, ErrorMessage = "El código no puede superar los 20 caracteres")]
        public string Codigo { get; set; }

        // Nullable + mensaje propio: si se deja en "-- Seleccione --" el binder ya no
        // genera el error en inglés "The IdTipo field is required.", sino este mensaje.
        [Required(ErrorMessage = "Seleccione el tipo de área")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione el tipo de área")]
        [Display(Name = "Tipo de área")]
        public int? IdTipo { get; set; }

        [Required(ErrorMessage = "La capacidad es obligatoria")]
        [Range(1, 10000, ErrorMessage = "La capacidad debe ser mayor a 0")]
        public int Capacidad { get; set; }

        public bool Estado { get; set; }

        // La anticipación mínima ya no se pide acá: ahora se define al crear
        // cada horario de reserva (ver DisponibilidadFormVM).

        [ValidateNever]
        public List<IFormFile> Fotos { get; set; }

        [ValidateNever]
        public List<AreaComunFoto> FotosExistentes { get; set; }
    }
}