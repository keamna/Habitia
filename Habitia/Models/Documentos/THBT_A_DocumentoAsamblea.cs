using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models.Documentos
{
    [Table("THBT_A_DocumentoAsamblea")]
    public class DocumentoAsamblea
    {
        [Key]
        public int TN_Id { get; set; }


        [Required]
        public int TN_IdAsamblea { get; set; }


        [Required]
        public int TN_IdDocumento { get; set; }


        public DateTime TF_FechaAsociacion { get; set; }


        // ===== Relaciones =====

        [ForeignKey(nameof(TN_IdAsamblea))]
        public Asamblea Asamblea { get; set; }


        [ForeignKey(nameof(TN_IdDocumento))]
        public Documento Documento { get; set; }
    }
}