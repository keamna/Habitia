// /Models/Financiero/H_Pago.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models.Financiero
{
    // Registro histórico de cada intento de pago procesado (aprobado o
    // rechazado), para conservar el rastro cuando el residente reintenta.
    [Table("THBT_H_Pago")]
    public class THBT_H_Pago
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        public int TN_IdPago { get; set; }

        // Indica si ese intento fue aprobado o rechazado.
        [Required]
        public bool TB_Aprobado { get; set; }

        [MaxLength(300)]
        public string? TC_MotivoRechazo { get; set; }

        [Required]
        public DateTime TF_FechaCambio { get; set; }

        [Required]
        public string TC_IdUsuarioCambio { get; set; }
    }
}