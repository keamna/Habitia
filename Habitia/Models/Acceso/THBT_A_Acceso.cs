using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitia.Models.Acceso
{
    [Table("THBT_A_Acceso")]
    public class Acceso
    {
        [Key]
        public int TN_Id { get; set; }

        // Siempre presentes, sin importar si el ingreso nació de un QR o fue manual.
        // Si vino de una Autorizacion, estos valores se copian de ahí al confirmar el ingreso.
        [Required]
        public int TN_IdVisitante { get; set; }

        [Required]
        public string TC_IdUsuario { get; set; }

        [Required]
        public int TN_IdVivienda { get; set; }

        [Required(ErrorMessage = "Debe indicar el motivo de la visita")]
        [MaxLength(200)]
        public string TC_Motivo { get; set; }

        // Solo tiene valor si el ingreso se originó validando una autorización por QR (US-04, punto 3).
        // Es null en un registro manual (US-04, punto 2).
        public int? TN_IdAutorizacion { get; set; }

        // Solo tiene valor si el visitante ingresó en vehículo (US-04, punto 4).
        public int? TN_IdVehiculo { get; set; }

        [Required]
        public DateTime TF_FechaIngreso { get; set; }

        public DateTime? TF_FechaSalida { get; set; }

        [Required]
        public bool TB_Estado { get; set; }

        // Relaciones
        [ForeignKey(nameof(TN_IdVisitante))]
        public Visitante Visitante { get; set; }

        [ForeignKey(nameof(TC_IdUsuario))]
        public ApplicationUser Usuario { get; set; }

        [ForeignKey(nameof(TN_IdVivienda))]
        public Vivienda Vivienda { get; set; }

        [ForeignKey(nameof(TN_IdAutorizacion))]
        public Autorizacion? Autorizacion { get; set; }

        [ForeignKey(nameof(TN_IdVehiculo))]
        public Vehiculo? Vehiculo { get; set; }
    }
}