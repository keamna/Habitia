using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Enums;

namespace Habitia.Models
{
    [Table("THBT_A_Incidencia")]
    public class Incidencia
    {
        [Key]
        public int Id { get; set; }


        [Required]
        public string IdUsuario { get; set; }


        public int? IdVivienda { get; set; }


        public int? IdAreaComun { get; set; }


        public TipoIncidenciaEnum Tipo { get; set; }


        public EstadoIncidenciaEnum Estado { get; set; }


        public ResponsabilidadEnum Responsabilidad { get; set; }


        [Required]
        [MaxLength(200)]
        public string Titulo { get; set; }


        [MaxLength(1000)]
        public string Descripcion { get; set; }


        public DateTime FechaRegistro { get; set; }



        // Relaciones

        [ForeignKey(nameof(IdUsuario))]
        public ApplicationUser Usuario { get; set; }


        [ForeignKey(nameof(IdVivienda))]
        public Vivienda Vivienda { get; set; }


        [ForeignKey(nameof(IdAreaComun))]
        public AreaComun AreaComun { get; set; }
    }
}