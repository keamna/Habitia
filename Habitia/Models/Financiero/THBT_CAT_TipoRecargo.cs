// /Models/Financiero/CAT_TipoRecargo.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models.Financiero
{
    // Fijo o Porcentaje (US-10, punto 4.2).
    [Table("THBT_CAT_TipoRecargo")]
    public class THBT_CAT_TipoRecargo
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string TC_Nombre { get; set; }
    }
}