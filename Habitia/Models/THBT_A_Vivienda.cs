using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Enums;

namespace Habitia.Models
{

    [Table("THBT_A_Vivienda")]
    public class Vivienda
    {

        [Key]
        public int Id { get; set; }



        [Required]
        [MaxLength(10)]
        public string Numero { get; set; }



        public TipoViviendaEnum Tipo { get; set; }



        public EstadoViviendaEnum Estado { get; set; }



        // cantidad máxima de personas que pueden alquilar
        public int CantidadInquilinos { get; set; }



        public DateTime FechaRegistro { get; set; }



        public ICollection<ViviendaUsuario> Usuarios { get; set; }

    }

}