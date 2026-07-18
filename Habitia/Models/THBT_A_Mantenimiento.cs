using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Models.Catalogos;

namespace Habitia.Models
{
    [Table("THBT_A_Mantenimiento")]
    public class Mantenimiento
    {
        [Key]
        public int Id { get; set; }


        public int IdTipo { get; set; }


        public int? IdAreaComun { get; set; }


        [Required]
        [MaxLength(200)]
        public string Descripcion { get; set; }


        public DateTime FechaInicio { get; set; }


        public DateTime? FechaFin { get; set; }


        public bool Estado { get; set; }



        // Relaciones

        [ForeignKey(nameof(IdTipo))]
        public TipoMantenimiento Tipo { get; set; }


        [ForeignKey(nameof(IdAreaComun))]
        public AreaComun AreaComun { get; set; }
    }
}