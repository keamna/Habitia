using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models.Documentos
{
    [Table("THBT_A_ParticipanteAsamblea")]
    public class ParticipanteAsamblea
    {
        [Key]
        public int TN_Id { get; set; }


        [Required]
        public int TN_IdAsamblea { get; set; }


        [Required]
        public string TC_IdUsuario { get; set; }


        // El residente confirmó que asistirá
        public bool TB_Confirmado { get; set; }


        // El admin validó que efectivamente asistió
        public bool TB_Asistio { get; set; }


        public DateTime TF_FechaRegistro { get; set; }


        // ===== Relaciones =====

        [ForeignKey(nameof(TN_IdAsamblea))]
        public Asamblea Asamblea { get; set; }


        [ForeignKey(nameof(TC_IdUsuario))]
        public ApplicationUser Usuario { get; set; }
    }
}