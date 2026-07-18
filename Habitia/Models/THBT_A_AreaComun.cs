using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Models.Catalogos;

namespace Habitia.Models
{
    [Table("THBT_A_AreaComun")]
    public class AreaComun
    {
        [Key]
        public int Id { get; set; }



        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }



        [Required]
        [MaxLength(20)]
        public string Codigo { get; set; }



        public int IdTipo { get; set; }



        public int Capacidad { get; set; }



        public bool Estado { get; set; }



        // Relaciones


        [ForeignKey(nameof(IdTipo))]
        public TipoArea Tipo { get; set; }



        public ICollection<DisponibilidadArea> Disponibilidades { get; set; }

        public ICollection<AreaComunFoto> Fotos { get; set; }
    }
}