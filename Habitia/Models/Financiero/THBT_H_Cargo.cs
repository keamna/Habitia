// /Models/Financiero/H_Cargo.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models.Financiero
{
    // Snapshot de THBT_A_Cargo cada vez que cambia de estado
    // (Pendiente -> En revisión -> Pagado / de vuelta a Pendiente si se rechaza).
    [Table("THBT_H_Cargo")]
    public class THBT_H_Cargo
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        public int TN_IdCargo { get; set; }

        [Required]
        public int TN_IdEstadoCargoAnterior { get; set; }

        [Required]
        public int TN_IdEstadoCargoNuevo { get; set; }

        [Required]
        public DateTime TF_FechaCambio { get; set; }

        [Required]
        public string TC_IdUsuarioCambio { get; set; }
    }
}