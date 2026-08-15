// /Models/Financiero/A_Cargo.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Models;
namespace Habitia.Models.Financiero
{
    // Cargo administrativo asignado a un residente (US-10, punto 1).
    [Table("THBT_A_Cargo")]
    public class THBT_A_Cargo
    {
        [Key]
        public int TN_Id { get; set; }
        [Required]
        public string TC_IdResidente { get; set; }
        [Required]
        public int TN_IdTipoCargo { get; set; }

        // --- recargo programado al crear el cargo ---
        public bool TB_RecargoProgramado { get; set; }
        public int? TN_IdTipoRecargo { get; set; }
        public decimal? TN_ValorRecargo { get; set; }
        public bool TB_RecargoAplicado { get; set; }

        // NUEVO: "Unico" (se aplica una sola vez al vencer) o "PorDia" (se aplica cada día que permanezca vencido)
        [MaxLength(10)]
        public string? TC_FrecuenciaRecargo { get; set; }

        // NUEVO: evita que el recargo "PorDia" se aplique dos veces el mismo día
        public DateTime? TF_UltimaAplicacionRecargo { get; set; }

        [Required]
        public int TN_IdEstadoCargo { get; set; }
        [MaxLength(200)]
        public string? TC_Descripcion { get; set; }
        [Required]
        public decimal TN_MontoBase { get; set; }
        [Required]
        public decimal TN_MontoIva { get; set; }
        // Calculado automáticamente: TN_MontoBase + TN_MontoIva 
        [Required]
        public decimal TN_MontoTotal { get; set; }
        [Required]
        public DateTime TF_FechaEmision { get; set; }
        [Required]
        public DateTime TF_FechaVencimiento { get; set; }
        public bool TB_AplicaIva { get; set; }
        [Required]
        public bool TB_Estado { get; set; }
        // Relaciones
        [ForeignKey(nameof(TC_IdResidente))]
        public ApplicationUser Residente { get; set; }
        [ForeignKey(nameof(TN_IdTipoCargo))]
        public THBT_CAT_TipoCargo TipoCargo { get; set; }
        [ForeignKey(nameof(TN_IdEstadoCargo))]
        public THBT_CAT_EstadoCargo EstadoCargo { get; set; }
        [ForeignKey(nameof(TN_IdTipoRecargo))]
        public THBT_CAT_TipoRecargo? TipoRecargo { get; set; }
    }
}