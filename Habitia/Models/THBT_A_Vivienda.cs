using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Enums;

namespace Habitia.Models
{
    [Table("THBT_A_Vivienda")]
    public class Vivienda
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string TC_Numero { get; set; }

        [Required]
        public TipoViviendaEnum TN_Tipo { get; set; }

        [Required]
        public EstadoViviendaEnum TN_Estado { get; set; }

        // cantidad máxima de personas que pueden vivir
        [Range(1, 20, ErrorMessage = "Cantidad inválida")]
        public int TN_CantidadInquilinos { get; set; }

        [Required]
        public DateTime TF_FechaRegistro { get; set; }

        // Relaciones
        public ICollection<ViviendaUsuario> Usuarios { get; set; }
    }
}