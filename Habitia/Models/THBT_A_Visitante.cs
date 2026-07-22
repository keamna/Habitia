using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Enums;

namespace Habitia.Models
{
    [Table("THBT_A_Visitante")]
    public class Visitante
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        public TipoIdentificacionEnum TN_TipoIdentificacion { get; set; }

        [Required]
        [MaxLength(20)]
        public string TC_Identificacion { get; set; }

        [Required]
        [MaxLength(150)]
        public string TC_Nombre { get; set; }

        [MaxLength(20)]
        public string? TC_Telefono { get; set; }

        [Required]
        public bool TB_Estado { get; set; }

        // Relaciones

        public ICollection<Autorizacion> Autorizaciones { get; set; }

        public ICollection<Vehiculo> Vehiculos { get; set; }
    }
}