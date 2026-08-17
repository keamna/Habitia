// /Models/Financiero/A_CargoRecurrenteResidente.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Models;

namespace Habitia.Models.Financiero
{
    // Solo se usa si TB_AplicarATodos == false
    [Table("THBT_A_CargoRecurrenteResidente")]
    public class THBT_A_CargoRecurrenteResidente
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        public int TN_IdCargoRecurrente { get; set; }

        [Required]
        public string TC_IdResidente { get; set; }

        public int? TN_IdVivienda { get; set; }

        [ForeignKey(nameof(TN_IdCargoRecurrente))]
        public THBT_A_CargoRecurrente CargoRecurrente { get; set; }

        [ForeignKey(nameof(TC_IdResidente))]
        public ApplicationUser Residente { get; set; }

        [ForeignKey(nameof(TN_IdVivienda))]
        public Vivienda? Vivienda { get; set; }
    }
}