namespace Habitia.ViewModels.Usuario
{
    public class PersonalMantenimientoListItemViewModel
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string NombreCompleto { get; set; }
        public string Identificacion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Estado { get; set; }
        public List<int> TiposIds { get; set; } = new();
        public List<string> TiposNombres { get; set; } = new();
    }
}