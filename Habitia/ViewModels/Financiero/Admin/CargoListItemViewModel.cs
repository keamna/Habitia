// CargoListItemViewModel.cs
namespace Habitia.ViewModels.Financiero.Admin
{
    // fila de la tabla de cargos del administrador.
    public class CargoListItemViewModel
    {
        public int TN_Id { get; set; }
        public string NombreResidente { get; set; }
        public string TipoCargo { get; set; }
        public decimal TN_MontoTotal { get; set; }
        public DateTime TF_FechaEmision { get; set; }
        public DateTime TF_FechaVencimiento { get; set; }
        public string EstadoCargo { get; set; }
    }
}