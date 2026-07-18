using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Documento
{
    public class DocumentoViewModel
    {
        public int Id { get; set; }


        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }


        [MaxLength(300)]
        public string Descripcion { get; set; }


        [Required]
        public int IdCategoria { get; set; }


        [Required]
        [MaxLength(300)]
        public string Archivo { get; set; }


        [Required]
        public string IdUsuario { get; set; }


        public bool Estado { get; set; }


        public DateTime FechaCarga { get; set; }
    }
}