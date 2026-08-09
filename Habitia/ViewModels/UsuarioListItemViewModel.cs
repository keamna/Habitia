namespace Habitia.ViewModels
{
    public class UsuarioListItemViewModel
    {
        public string Id { get; set; }
        public string TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Estado { get; set; }
        public List<string> Roles { get; set; }
        public List<ViviendaResumenViewModel> Viviendas { get; set; }
        public List<int> TiposMantenimientoIds { get; set; } = new();
        public List<string> TiposMantenimientoNombres { get; set; } = new();
    }
}