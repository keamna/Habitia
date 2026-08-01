// ConfiguracionPagoViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Habitia.ViewModels.Financiero.Admin
{
    // configuración de métodos de pago habilitados y sus datos de cobro.
    public class ConfiguracionPagoViewModel
    {
        public int TN_Id { get; set; }

        public bool TB_EfectivoHabilitado { get; set; }
        public bool TB_TarjetaHabilitado { get; set; }
        public bool TB_SinpeHabilitado { get; set; }

        // Solo obligatorios si TB_TarjetaHabilitado = true (se valida en el controller,
        // porque depende de otro campo del mismo form).
        [MaxLength(150, ErrorMessage = "Debe completar los campos obligatorios")]
        public string TC_TitularTarjeta { get; set; }

        [MaxLength(34, ErrorMessage = "Debe completar los campos obligatorios")]
        public string TC_IbanTarjeta { get; set; }

        // Nombres de tarjeta habilitados (US-10, punto 2: "tipo de tarjetas").
        public List<string> TC_TiposTarjeta { get; set; } = new();

        // Solo obligatorios si TB_SinpeHabilitado = true.
        [MaxLength(150, ErrorMessage = "Debe completar los campos obligatorios")]
        public string TC_TitularSinpe { get; set; }

        [MaxLength(15, ErrorMessage = "Debe completar los campos obligatorios")]
        public string TC_NumeroSinpe { get; set; }
    }
}