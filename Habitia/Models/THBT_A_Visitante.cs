using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Habitia.Enums;

namespace Habitia.Models
{
    [Table("THBT_A_Visitante")]
    public class Visitante
    {
        [Key]
        public int Id { get; set; }


        [Required]
        [MaxLength(20)]
        public string Identificacion { get; set; }


        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }


        [MaxLength(20)]
        public string Telefono { get; set; }



        // Relaciones

        public ICollection<Autorizacion> Autorizaciones { get; set; }

        public TipoIdentificacionEnum TipoIdentificacion { get; set; }

        public ICollection<Vehiculo> Vehiculos { get; set; }
    }
}