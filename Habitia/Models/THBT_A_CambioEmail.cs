using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models
{
    [Table("THBT_A_CambioEmail")]
    public class CambioEmail
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        public string TC_IdUsuario { get; set; }

        [ForeignKey(nameof(TC_IdUsuario))]
        public ApplicationUser Usuario { get; set; }

        /// <summary>
        /// El email NUEVO que quiere confirmar
        /// </summary>
        [Required, StringLength(256)]
        public string TC_EmailNuevo { get; set; }

        /// <summary>
        /// Código de 6 dígitos para confirmar el email
        /// </summary>
        [Required, StringLength(6)]
        public string TC_Codigo { get; set; }

        public DateTime TF_FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime TF_FechaExpiracion { get; set; }

        public int TN_Intentos { get; set; } = 0;

        public bool TB_Confirmado { get; set; } = false;

        public bool TB_Invalidado { get; set; } = false;

        public DateTime? TF_UltimoReenvioUtc { get; set; }
    }
}