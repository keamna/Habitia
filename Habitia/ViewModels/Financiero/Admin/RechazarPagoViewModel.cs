// /ViewModels/Financiero/Admin/RechazarPagoViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Financiero.Admin
{
    public class RechazarPagoViewModel
    {
        [Required]
        public int TN_IdPago { get; set; }

        [Required(ErrorMessage = "Debe indicar el motivo del rechazo")]
        [MaxLength(300)]
        public string TC_MotivoRechazo { get; set; }
    }
}