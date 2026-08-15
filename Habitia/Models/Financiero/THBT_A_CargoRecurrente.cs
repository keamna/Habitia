// /Models/Financiero/A_CargoRecurrente.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Models;

namespace Habitia.Models.Financiero
{
    [Table("THBT_A_CargoRecurrente")]
    public class THBT_A_CargoRecurrente
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        public int TN_IdTipoCargo { get; set; }

        [Required]
        public decimal TN_MontoBase { get; set; }

        public bool TB_AplicaIva { get; set; }

        [MaxLength(200)]
        public string? TC_Descripcion { get; set; }

        // "Diario", "Semanal", "Mensual"
        [Required]
        [MaxLength(20)]
        public string TC_Frecuencia { get; set; }

        [Required]
        public bool TB_AplicarATodos { get; set; }

        // Recargo programado, opcional, se hereda a cada cargo generado
        public bool TB_RecargoProgramado { get; set; }
        public int? TN_IdTipoRecargo { get; set; }
        public decimal? TN_ValorRecargo { get; set; }

        [MaxLength(10)]
        public string? TC_FrecuenciaRecargo { get; set; }

        [Required]
        public DateTime TF_FechaInicio { get; set; }
        public DateTime? TF_UltimaGeneracion { get; set; }


        [Required]
        public bool TB_Estado { get; set; } // activa / pausada

        // Relaciones
        [ForeignKey(nameof(TN_IdTipoCargo))]
        public THBT_CAT_TipoCargo TipoCargo { get; set; }

        [ForeignKey(nameof(TN_IdTipoRecargo))]
        public THBT_CAT_TipoRecargo? TipoRecargo { get; set; }

        public List<THBT_A_CargoRecurrenteResidente> Residentes { get; set; } = new();
    }
}