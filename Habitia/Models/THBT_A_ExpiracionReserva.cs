using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Enums;

namespace Habitia.Models
{
    [Table("THBT_A_ExpiracionReserva")]
    public class ExpiracionReserva
    {
        [Key]
        public int TN_Id { get; set; }


        // Tiempo mínimo requerido antes de cancelar
        [Required]
        [Range(1, 1000, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int TN_Cantidad { get; set; }


        // 1 = Horas, 2 = Días, 3 = Minutos
        [Required]
        public TipoTiempoEnum TN_Tipo { get; set; }


        [Required]
        public bool TB_Estado { get; set; }
    }
}