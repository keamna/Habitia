using Habitia.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels
{
    public class AreaComunVM
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Codigo { get; set; }

        public int IdTipo { get; set; }

        public int Capacidad { get; set; }

        public bool Estado { get; set; }

        public List<IFormFile> Fotos { get; set; }

        public List<AreaComunFoto> FotosExistentes { get; set; }
    }
}