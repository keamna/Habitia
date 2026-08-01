using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Enums;

namespace Habitia.Models
{
    [Table("THBT_A_Incidencia")]
    public class Incidencia
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        public string TC_IdUsuario { get; set; }

        public int? TN_IdVivienda { get; set; }
        public int? TN_IdAreaComun { get; set; }

        [Required]
        public TipoIncidenciaEnum TN_Tipo { get; set; }

        [Required]
        public EstadoIncidenciaEnum TN_Estado { get; set; }

        // Ahora se define AL CREAR la incidencia, ya no en un paso posterior
        [Required]
        public ResponsabilidadEnum TN_Responsabilidad { get; set; }

        // El Admin la asigna después, por eso es nullable
        public PrioridadEnum? TN_Prioridad { get; set; }

        [Required]
        [MaxLength(100)]
        public string TC_Titulo { get; set; }

        [MaxLength(500)]
        public string? TC_Descripcion { get; set; }

        [Required]
        public DateTime TF_FechaRegistro { get; set; }

        [MaxLength(250)]
        public string? TC_ImagenUrl { get; set; }

        // Relaciones
        [ForeignKey(nameof(TC_IdUsuario))]
        public ApplicationUser Usuario { get; set; }

        [ForeignKey(nameof(TN_IdVivienda))]
        public Vivienda? Vivienda { get; set; }

        [ForeignKey(nameof(TN_IdAreaComun))]
        public AreaComun? AreaComun { get; set; }

        public Mantenimiento? Mantenimiento { get; set; }

        public DateTime? TF_FechaResolucion { get; set; }
    }
}