using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Enums;

namespace Habitia.Models
{
    [Table("THBT_A_Autorizacion")]
    public class Autorizacion
    {
        [Key]
        public int Id { get; set; }

        public int IdVisitante { get; set; }

        [Required]
        public string IdUsuario { get; set; }

        public int IdVivienda { get; set; }

        [Required]
        [MaxLength(100)]
        public string Codigo { get; set; }

        public DateTime Inicio { get; set; }

        public DateTime Fin { get; set; }

        [MaxLength(200)]
        public string Motivo { get; set; }

        public DateTime FechaRegistro { get; set; }

        public EstadoAutorizacionEnum Estado { get; set; }

        // Relaciones


        [ForeignKey(nameof(IdVisitante))]
        public Visitante Visitante { get; set; }


        [ForeignKey(nameof(IdUsuario))]
        public ApplicationUser Usuario { get; set; }


        [ForeignKey(nameof(IdVivienda))]
        public Vivienda Vivienda { get; set; }


        public ICollection<Acceso> Accesos { get; set; }
    }
}