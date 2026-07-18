using Habitia.Enums;
using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Vivienda
{
    public class ViviendaViewModel
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "El número de vivienda es obligatorio.")]
        public string Numero { get; set; }



        [Required(ErrorMessage = "Seleccione el tipo de vivienda.")]
        public TipoViviendaEnum Tipo { get; set; }



        [Required(ErrorMessage = "Indique la cantidad de inquilinos.")]
        [Range(0, 10, ErrorMessage = "La cantidad debe estar entre 0 y 10.")]
        public int CantidadInquilinos { get; set; }
    }
}