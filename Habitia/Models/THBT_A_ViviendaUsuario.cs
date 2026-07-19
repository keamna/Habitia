using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Enums;

namespace Habitia.Models
{
    [Table("THBT_A_ViviendaUsuario")]
    public class ViviendaUsuario
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        public int TN_IdVivienda { get; set; }

        [Required]
        public string TC_IdUsuario { get; set; }

        [Required]
        public TipoRelacionEnum TN_TipoRelacion { get; set; }

        [Required]
        public EstadoUsuarioEnum TN_Estado { get; set; }

        // Indica si realmente reside en la vivienda
        [Required]
        public bool TB_ViveAhi { get; set; }

        [Required]
        public DateTime TF_FechaRegistro { get; set; }

        // Relaciones

        [ForeignKey(nameof(TN_IdVivienda))]
        public Vivienda Vivienda { get; set; }

        [ForeignKey(nameof(TC_IdUsuario))]
        public ApplicationUser Usuario { get; set; }
    }
}