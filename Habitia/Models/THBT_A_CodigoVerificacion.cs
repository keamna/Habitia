using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models
{
    [Table("THBT_A_CodigoVerificacion")]
    public class CodigoVerificacion
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        public string TC_IdUsuario { get; set; }

        [ForeignKey(nameof(TC_IdUsuario))]
        public ApplicationUser Usuario { get; set; }

        [Required, StringLength(6)]
        public string TC_Codigo { get; set; }

        public DateTime TF_FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime TF_FechaExpiracion { get; set; }

        public int TN_Intentos { get; set; } = 0;

        public bool TB_Usado { get; set; } = false;

        public bool TB_Invalidado { get; set; } = false;

        public DateTime? TF_UltimoReenvioUtc { get; set; }

        /// <summary>
        /// Contador de ciclos fallidos completos (cuando el usuario agota los 5 intentos).
        /// - Ciclo 1: 5 intentos fallidos → cooldown 1 min
        /// - Ciclo 2: 5 intentos fallidos más → cooldown 5 min
        /// - Ciclo 3: 5 intentos fallidos más → cooldown 15 min
        /// - Ciclo 4+: cooldown 30 min (tope máximo)
        /// 
        /// Se resetea a 0 cuando el usuario verifica correctamente.
        /// </summary>
        public int TN_CiclosFallidos { get; set; } = 0;
    }
}