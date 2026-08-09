namespace Habitia.ViewModels.Usuario
{
    public class EditarPersonalMantenimientoVM
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public List<int>? TiposMantenimientoIds { get; set; }
    }
}