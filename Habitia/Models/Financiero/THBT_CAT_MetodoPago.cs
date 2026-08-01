// /Models/Financiero/CAT_MetodoPago.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models.Financiero
{
    // Catálogo fijo: Efectivo, Tarjeta, SINPE 
    [Table("THBT_CAT_MetodoPago")]
    public class THBT_CAT_MetodoPago
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string TC_Nombre { get; set; }
    }
}