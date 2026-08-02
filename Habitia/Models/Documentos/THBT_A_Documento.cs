using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Models.Catalogos;

namespace Habitia.Models.Documentos
{
    [Table("THBT_A_Documento")]
    public class Documento
    {
        [Key]
        public int TN_Id { get; set; }


        [Required]
        [MaxLength(100)]
        public string TC_Nombre { get; set; }


        [MaxLength(300)]
        public string? TC_Descripcion { get; set; }


        [Required]
        public int TN_IdCategoria { get; set; }


        // Ruta relativa del archivo: /uploads/documentos/xxx.pdf
        [Required]
        [MaxLength(300)]
        public string TC_Archivo { get; set; }


        // Nombre original con que el admin lo subió (para la descarga)
        [Required]
        [MaxLength(200)]
        public string TC_NombreOriginal { get; set; }


        // Tamaño en bytes, para mostrarlo en el listado
        public long TN_Tamano { get; set; }


        // Usuario (admin) que lo cargó
        [Required]
        public string TC_IdUsuario { get; set; }


        // true = activo (visible para residentes), false = inactivo
        public bool TB_Estado { get; set; }


        public DateTime TF_FechaCarga { get; set; }


        // ===== Relaciones =====

        [ForeignKey(nameof(TN_IdCategoria))]
        public CategoriaDocumento Categoria { get; set; }


        [ForeignKey(nameof(TC_IdUsuario))]
        public ApplicationUser Usuario { get; set; }


        public ICollection<DocumentoAsamblea> Asambleas { get; set; }
            = new List<DocumentoAsamblea>();
    }
}