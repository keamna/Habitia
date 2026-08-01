using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Enums;

namespace Habitia.Models.Acceso
{
    [Table("THBT_A_Autorizacion")]
    public class Autorizacion
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        public int TN_IdVisitante { get; set; }

        [Required]
        public string TC_IdUsuario { get; set; }

        [Required]
        public int TN_IdVivienda { get; set; }

        [Required]
        [MaxLength(100)]
        public string TC_Codigo { get; set; }

        [Required]
        public DateTime TF_FechaVisita { get; set; }

        [Required]
        public DateTime TF_FechaVencimiento { get; set; }

        [Required(ErrorMessage = "Debe indicar el motivo de la visita")]
        [MaxLength(200)]
        public string TC_Motivo { get; set; }

        [Required]
        public DateTime TF_FechaRegistro { get; set; }

        [Required]
        public EstadoAutorizacionEnum TN_Estado { get; set; }

        // Relaciones
        [ForeignKey(nameof(TN_IdVisitante))]
        public Visitante Visitante { get; set; }

        [ForeignKey(nameof(TC_IdUsuario))]
        public ApplicationUser Usuario { get; set; }

        [ForeignKey(nameof(TN_IdVivienda))]
        public Vivienda Vivienda { get; set; }

        public ICollection<Acceso> Accesos { get; set; }
            = new List<Acceso>();
    }
}