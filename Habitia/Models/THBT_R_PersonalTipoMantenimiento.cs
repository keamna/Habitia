using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Models.Catalogos;

namespace Habitia.Models
{
    [Table("THBT_A_PersonalTipoMantenimiento")]
    public class PersonalTipoMantenimiento
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        public string TC_IdPersonal { get; set; }

        [Required]
        public int TN_IdTipo { get; set; }

        [ForeignKey(nameof(TC_IdPersonal))]
        public ApplicationUser Personal { get; set; }

        [ForeignKey(nameof(TN_IdTipo))]
        public TipoMantenimiento Tipo { get; set; }
    }
}