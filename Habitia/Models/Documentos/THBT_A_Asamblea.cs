using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Enums;

namespace Habitia.Models.Documentos
{
    [Table("THBT_A_Asamblea")]
    public class Asamblea
    {
        [Key]
        public int TN_Id { get; set; }


        [Required]
        [MaxLength(150)]
        public string TC_Titulo { get; set; }


        [Required]
        public DateTime TF_FechaHora { get; set; }


        public ModalidadAsambleaEnum TN_Modalidad { get; set; }


        // Si es presencial: dirección o salón.
        // Si es virtual: enlace de la reunión.
        [MaxLength(300)]
        public string? TC_Lugar { get; set; }


        [MaxLength(500)]
        public string? TC_Descripcion { get; set; }


        public EstadoAsambleaEnum TN_Estado { get; set; }


        // Admin que la creó
        [Required]
        public string TC_IdUsuario { get; set; }


        public DateTime TF_FechaRegistro { get; set; }


        // ===== Relaciones =====

        [ForeignKey(nameof(TC_IdUsuario))]
        public ApplicationUser Usuario { get; set; }


        public ICollection<ParticipanteAsamblea> Participantes { get; set; }
            = new List<ParticipanteAsamblea>();


        public ICollection<DocumentoAsamblea> Documentos { get; set; }
            = new List<DocumentoAsamblea>();
    }
}