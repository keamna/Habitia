using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Enums;
using Habitia.Models.Catalogos;

namespace Habitia.Models
{
    [Table("THBT_A_Mantenimiento")]
    public class Mantenimiento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdIncidencia { get; set; }

        [Required]
        public int IdTipo { get; set; }

        public int? IdAreaComun { get; set; }

        [Required]
        public string IdPersonalAsignado { get; set; }

        [Required]
        [MaxLength(200)]
        public string Descripcion { get; set; }

        [Required]
        public DateTime FechaProgramada { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public EstadoMantenimientoEnum Estado { get; set; }

        [MaxLength(100)]
        public string? Observaciones { get; set; }

        // Relaciones
        [ForeignKey(nameof(IdIncidencia))]
        public Incidencia Incidencia { get; set; }

        [ForeignKey(nameof(IdTipo))]
        public TipoMantenimiento Tipo { get; set; }

        [ForeignKey(nameof(IdAreaComun))]
        public AreaComun AreaComun { get; set; }

        [ForeignKey(nameof(IdPersonalAsignado))]
        public ApplicationUser PersonalAsignado { get; set; }
    }
}