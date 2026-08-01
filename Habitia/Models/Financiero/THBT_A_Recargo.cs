// /Models/Financiero/A_Recargo.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models.Financiero
{
    // Recargo aplicado a un cargo vencido
    [Table("THBT_A_Recargo")]
    public class THBT_A_Recargo
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        public int TN_IdCargo { get; set; }

        [Required]
        public int TN_IdTipoRecargo { get; set; }

        // Valor base del recargo: monto fijo o porcentaje, según TN_IdTipoRecargo.
        [Required]
        public decimal TN_Valor { get; set; }

        // Monto final ya calculado y aplicado al cargo.
        [Required]
        public decimal TN_MontoAplicado { get; set; }

        [Required]
        public DateTime TF_FechaAplicacion { get; set; }

        [Required]
        public bool TB_Estado { get; set; }

        // Relaciones
        [ForeignKey(nameof(TN_IdCargo))]
        public THBT_A_Cargo Cargo { get; set; }

        [ForeignKey(nameof(TN_IdTipoRecargo))]
        public THBT_CAT_TipoRecargo TipoRecargo { get; set; }
    }
}