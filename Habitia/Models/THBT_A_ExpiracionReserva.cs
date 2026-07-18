using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Enums;

namespace Habitia.Models
{
    [Table("THBT_A_ExpiracionReserva")]
    public class ExpiracionReserva
    {
        [Key]
        public int Id { get; set; }


        // Tiempo mínimo requerido antes de cancelar
        public int Cantidad { get; set; }


        // 1 = Horas, 2 = Días, 3 = Minutos
        public TipoTiempoEnum Tipo { get; set; }


        public bool Estado { get; set; }
    }
}