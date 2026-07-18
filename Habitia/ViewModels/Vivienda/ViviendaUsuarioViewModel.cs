using Habitia.Enums;
using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Vivienda
{
    public class ViviendaUsuarioViewModel
    {

        public int Id { get; set; }


        [Required]
        public string IdUsuario { get; set; }


        [Required]
        public int IdVivienda { get; set; }


        [Required]
        public TipoRelacionEnum TipoRelacion { get; set; }


        public bool ViveAhi { get; set; }


        public DateTime FechaRegistro { get; set; }

    }
}