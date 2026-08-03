using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels.Asamblea
{
    public class AsambleaFormVM
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "El título es obligatorio.")]
        [MaxLength(150, ErrorMessage = "Máximo 150 caracteres.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; }


        [Required(ErrorMessage = "Indique la fecha y hora.")]
        [Display(Name = "Fecha y hora")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FechaHora { get; set; }
            = DateTime.Today.AddDays(7).AddHours(18);


        [Required(ErrorMessage = "Seleccione la modalidad.")]
        [Display(Name = "Modalidad")]
        public ModalidadAsambleaEnum Modalidad { get; set; }
            = ModalidadAsambleaEnum.Presencial;


        [MaxLength(300, ErrorMessage = "Máximo 300 caracteres.")]
        [Display(Name = "Lugar o enlace")]
        public string? Lugar { get; set; }


        [MaxLength(500, ErrorMessage = "Máximo 500 caracteres.")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
    }
}