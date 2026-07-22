using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Models.Catalogos;

namespace Habitia.Models
{
    [Table("THBT_A_AreaComun")]
    public class AreaComun
    {
        [Key]
        public int TN_Id { get; set; }


        [Required]
        [MaxLength(100)]
        public string TC_Nombre { get; set; }


        [Required]
        [MaxLength(20)]
        public string TC_Codigo { get; set; }


        [Required]
        public int TN_IdTipo { get; set; }


        [Required]
        [Range(1, 1000, ErrorMessage = "La capacidad debe ser mayor a 0")]
        public int TN_Capacidad { get; set; }


        [Required]
        public bool TB_Estado { get; set; }


        // Relaciones

        [ForeignKey(nameof(TN_IdTipo))]
        public TipoArea Tipo { get; set; }


        public ICollection<DisponibilidadArea> Disponibilidades { get; set; }
            = new List<DisponibilidadArea>();

        public ICollection<AreaComunFoto> Fotos { get; set; }
            = new List<AreaComunFoto>();
    }
}