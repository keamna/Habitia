using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels.Pago
{
    public class PagoViewModel
    {
        public int Id { get; set; }


        [Required]
        public int IdCargo { get; set; }


        [Required]
        public string IdUsuario { get; set; }


        [Required]
        public int IdMetodoPago { get; set; }


        [Required]
        public decimal Monto { get; set; }


        [MaxLength(300)]
        public string Comprobante { get; set; }


        public EstadoPagoEnum Estado { get; set; }


        public DateTime FechaRegistro { get; set; }


        [MaxLength(300)]
        public string Observacion { get; set; }
    }
}