// /Models/Financiero/CAT_TipoTarjeta.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models.Financiero
{
    // Nombres de tarjeta que el administrador registra en Configuración de pagos
    [Table("THBT_CAT_TipoTarjeta")]
    public class THBT_CAT_TipoTarjeta
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string TC_Nombre { get; set; }
    }
}