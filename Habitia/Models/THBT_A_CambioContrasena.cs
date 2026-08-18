using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models
{
    [Table("THBT_A_CambioContrasena")]
    public class CambioContrasena
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        public string TC_IdUsuario { get; set; }

        [ForeignKey(nameof(TC_IdUsuario))]
        public ApplicationUser Usuario { get; set; }

        /// <summary>
        /// Contraseña temporal generada (sin encriptar, solo para mostrar al usuario)
        /// </summary>
        [Required, StringLength(12)]
        public string TC_ContrasenaTemporal { get; set; }

        public DateTime TF_FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime TF_FechaExpiracion { get; set; }

        /// <summary>
        /// True cuando el usuario cambia la contraseña desde Mi Perfil
        /// </summary>
        public bool TB_Utilizado { get; set; } = false;

        public DateTime? TF_FechaUtilizacion { get; set; }
    }
}