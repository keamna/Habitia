using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Enums;

namespace Habitia.Models
{
    [Table("THBT_A_ViviendaUsuario")]
    public class ViviendaUsuario
    {

        [Key]
        public int Id { get; set; }



        public int IdVivienda { get; set; }



        public string IdUsuario { get; set; }



        public TipoRelacionEnum TipoRelacion { get; set; }



        public EstadoUsuarioEnum Estado { get; set; }



        // Indica si el propietario vive actualmente en la vivienda
        public bool ViveAhi { get; set; }



        public DateTime FechaRegistro { get; set; }



        [ForeignKey(nameof(IdVivienda))]
        public Vivienda Vivienda { get; set; }



        [ForeignKey(nameof(IdUsuario))]
        public ApplicationUser Usuario { get; set; }

    }
}