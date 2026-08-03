using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Models.Documentos;

namespace Habitia.Models.Catalogos
{
    [Table("THBT_CAT_CategoriaDocumento")]
    public class CategoriaDocumento
    {
        [Key]
        public int TN_Id { get; set; }


        [Required]
        [MaxLength(50)]
        public string TC_Nombre { get; set; }


        [Required]
        public bool TB_Estado { get; set; }


        // Relaciones
        public ICollection<Documento> Documentos { get; set; }
            = new List<Documento>();
    }
}