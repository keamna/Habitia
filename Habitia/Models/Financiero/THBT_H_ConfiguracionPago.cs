// /Models/Financiero/H_ConfiguracionPago.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models.Financiero
{
    // Snapshot de A_ConfiguracionPago antes de cada actualización (auditoría).
    [Table("THBT_H_ConfiguracionPago")]
    public class THBT_H_ConfiguracionPago
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        public int TN_IdConfiguracionPago { get; set; }

        public bool TB_EfectivoHabilitado { get; set; }
        public bool TB_TarjetaHabilitado { get; set; }
        public bool TB_SinpeHabilitado { get; set; }

        [MaxLength(150)]
        public string TC_TitularTarjeta { get; set; }

        [MaxLength(34)]
        public string TC_IbanTarjeta { get; set; }

        [MaxLength(150)]
        public string TC_TitularSinpe { get; set; }

        [MaxLength(15)]
        public string TC_NumeroSinpe { get; set; }

        [Required]
        public DateTime TF_FechaCambio { get; set; }

        [Required]
        public string TC_IdUsuarioCambio { get; set; }
    }
}