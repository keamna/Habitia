// HistorialFinancieroViewModel.cs
namespace Habitia.ViewModels.Financiero.Admin
{
    // historial de todos los residentes.
    public class HistorialFinancieroViewModel
    {
        public List<HistorialItemViewModel> Registros { get; set; } = new();
    }

    public class HistorialItemViewModel
    {
        public string NombreResidente { get; set; }
        public string TipoRegistro { get; set; } // Cargo / Pago / Recargo
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
    }
}