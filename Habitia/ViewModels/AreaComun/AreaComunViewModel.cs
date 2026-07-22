using Habitia.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels
{
    public class AreaComunVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El código es obligatorio")]
        [StringLength(20, ErrorMessage = "El código no puede superar los 20 caracteres")]
        public string Codigo { get; set; }

        public int IdTipo { get; set; }

        [Required(ErrorMessage = "La capacidad es obligatoria")]
        [Range(1, 10000, ErrorMessage = "La capacidad debe ser mayor a 0")]
        public int Capacidad { get; set; }

        public bool Estado { get; set; }

        [ValidateNever]
        public List<IFormFile> Fotos { get; set; }

        [ValidateNever]
        public List<AreaComunFoto> FotosExistentes { get; set; }
    }
}