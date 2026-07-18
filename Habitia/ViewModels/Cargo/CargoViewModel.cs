using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels.Cargo
{
    public class CargoViewModel
    {
        public int Id { get; set; }


        [Required]
        public string IdUsuario { get; set; }


        [Required]
        public int IdTipoCargo { get; set; }


        [Required]
        [MaxLength(300)]
        public string Descripcion { get; set; }


        [Required]
        public decimal MontoBase { get; set; }


        public decimal IVA { get; set; }


        public decimal Total { get; set; }


        public DateTime FechaEmision { get; set; }


        public DateTime FechaLimite { get; set; }


        public EstadoCargoEnum Estado { get; set; }


        public TipoOrigenCargoEnum Origen { get; set; }
    }
}