// /Models/Financiero/A_ConfiguracionPago.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models.Financiero
{
    // Configuración global de pagos del condominio (US-10, punto 1).
    // Se asume un único registro activo (TB_Estado = true) por condominio.
    [Table("THBT_A_ConfiguracionPago")]
    public class THBT_A_ConfiguracionPago
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        public bool TB_EfectivoHabilitado { get; set; }

        [Required]
        public bool TB_TarjetaHabilitado { get; set; }

        [Required]
        public bool TB_SinpeHabilitado { get; set; }

        // Datos de cobro para el método Tarjeta 
        [MaxLength(150)]
        public string TC_TitularTarjeta { get; set; }

        [MaxLength(34)] // largo máximo estándar de un IBAN
        public string TC_IbanTarjeta { get; set; }

        // Datos de cobro para el método SINPE (US-10, punto 2).
        [MaxLength(150)]
        public string TC_TitularSinpe { get; set; }

        [MaxLength(15)]
        public string TC_NumeroSinpe { get; set; }

        [Required]
        public DateTime TF_FechaActualizacion { get; set; }

        [Required]
        public bool TB_Estado { get; set; }
    }
}