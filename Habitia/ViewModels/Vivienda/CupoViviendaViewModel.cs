using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Vivienda
{
    public class CupoViviendaViewModel
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public int InquilinosActuales { get; set; }
        public string EtiquetaInquilinos { get; set; } = "Inquilinos";

        [Required(ErrorMessage = "Indique la cantidad.")]
        [Range(0, 10, ErrorMessage = "La cantidad debe estar entre 0 y 10.")]
        public int CantidadInquilinos { get; set; }
    }
}