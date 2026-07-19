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
        public int TN_Id { get; set; }

        [Required]
        public int TN_IdIncidencia { get; set; }

        [Required]
        public int TN_IdTipo { get; set; }

        public int? TN_IdAreaComun { get; set; }

        [Required]
        public string TC_IdPersonalAsignado { get; set; }

        [Required]
        [MaxLength(300)]
        public string TC_Descripcion { get; set; }

        [Required]
        public DateTime TF_FechaProgramada { get; set; }

        public DateTime? TF_FechaInicio { get; set; }
        public DateTime? TF_FechaFin { get; set; }

        [Required]
        public EstadoMantenimientoEnum TN_Estado { get; set; }

        [MaxLength(300)]
        public string? TC_Observaciones { get; set; }

        // Relaciones

        [ForeignKey(nameof(TN_IdIncidencia))]
        public Incidencia Incidencia { get; set; }

        [ForeignKey(nameof(TN_IdTipo))]
        public TipoMantenimiento Tipo { get; set; }

        [ForeignKey(nameof(TN_IdAreaComun))]
        public AreaComun? AreaComun { get; set; }

        [ForeignKey(nameof(TC_IdPersonalAsignado))]
        public ApplicationUser PersonalAsignado { get; set; }
    }
}