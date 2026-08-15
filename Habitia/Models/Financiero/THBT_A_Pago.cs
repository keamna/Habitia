// /Models/Financiero/A_Pago.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models.Financiero
{
    // Intento de pago de un cargo, hecho por el residente (US-10, punto 2).
    // El estado del flujo (Pendiente / En revisión / Pagado) vive en A_Cargo;
    // este registro solo guarda los datos propios del intento de pago.
    [Table("THBT_A_Pago")]
    public class THBT_A_Pago
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        public int TN_IdCargo { get; set; }

        [Required]
        public int TN_IdMetodoPago { get; set; }

        // Ruta del comprobante subido por el residente (US-10, punto 2.10).
        [Required]
        [MaxLength(300)]
        public string TC_RutaComprobante { get; set; }

        [Required]
        public DateTime TF_FechaPago { get; set; }

        // Solo tiene valor si el administrador rechaza el pago (US-10, punto 3.4).
        [MaxLength(300)]
        public string? TC_MotivoRechazo { get; set; }

        [Required]
        public bool TB_Estado { get; set; }

        // Relaciones
        [ForeignKey(nameof(TN_IdCargo))]
        public THBT_A_Cargo Cargo { get; set; }

        [ForeignKey(nameof(TN_IdMetodoPago))]
        public THBT_CAT_MetodoPago MetodoPago { get; set; }
    }
}