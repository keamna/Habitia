// /Models/Financiero/CAT_TipoCargo.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models.Financiero
{
    [Table("THBT_CAT_TipoCargo")]
    public class THBT_CAT_TipoCargo
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string TC_Nombre { get; set; }
    }
}