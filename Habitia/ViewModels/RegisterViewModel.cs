using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels
{
    public class RegisterViewModel
    {


        // ==========================
        // DATOS PERSONALES
        // ==========================


        [Required(ErrorMessage = "Seleccione el tipo de identificación")]
        public TipoIdentificacionEnum? TC_TipoIdentificacion { get; set; }



        [Required(ErrorMessage = "Ingrese el número de identificación")]
        [MaxLength(20)]
        public string TC_NumeroIdentificacion { get; set; }



        [Required(ErrorMessage = "Ingrese el nombre completo")]
        [MaxLength(200)]
        public string TC_NombreCompleto { get; set; }



        [Required(ErrorMessage = "Ingrese el teléfono")]
        public string TC_Telefono { get; set; }



        [Required(ErrorMessage = "Ingrese el correo")]
        [EmailAddress]
        public string Email { get; set; }



        [Required(ErrorMessage = "Ingrese una contraseña")]
        public string Password { get; set; }



        [Required(ErrorMessage = "Confirme la contraseña")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }






        // ==========================
        // RELACIÓN VIVIENDA
        // ==========================


        [Required(ErrorMessage = "Seleccione la relación con la vivienda")]
        public TipoRelacionEnum? TC_TipoRelacion { get; set; }




        public bool TB_ViveAhi { get; set; }







        // ==========================
        // DATOS VIVIENDA
        // ==========================


        [Required(ErrorMessage = "Seleccione el tipo de vivienda")]
        public TipoViviendaEnum? TC_TipoVivienda { get; set; }




        public int? TN_ViviendaId { get; set; }


    }
}