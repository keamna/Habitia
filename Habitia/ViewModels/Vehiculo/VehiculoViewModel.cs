using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Vehiculo
{
    public class VehiculoViewModel
    {
        public int Id { get; set; }


        [Required]
        public int IdVisitante { get; set; }


        [Required]
        [MaxLength(20)]
        public string Placa { get; set; }


        [MaxLength(50)]
        public string Tipo { get; set; }


        [MaxLength(200)]
        public string Observaciones { get; set; }
    }
}