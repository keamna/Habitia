// /Models/Financiero/CAT_EstadoCargo.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models.Financiero
{
    // Pendiente, En revisión, Pagado
    [Table("THBT_CAT_EstadoCargo")]
    public class THBT_CAT_EstadoCargo
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string TC_Nombre { get; set; }
    }
}