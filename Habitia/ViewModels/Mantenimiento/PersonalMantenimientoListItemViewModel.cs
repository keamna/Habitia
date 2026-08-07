namespace Habitia.ViewModels.Personal
{
    public class PersonalMantenimientoListItemViewModel
    {
        public string Id { get; set; }
        public string Identificacion { get; set; }
        public string NombreCompleto { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public bool Activo { get; set; }
        public List<string> TiposMantenimiento { get; set; } = new();
    }
}