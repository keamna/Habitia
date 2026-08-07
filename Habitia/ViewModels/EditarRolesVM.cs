namespace Habitia.ViewModels
{
    public class EditarRolesVM
    {
        public string Id { get; set; }
        public string Rol { get; set; }
        public List<int>? TiposMantenimientoIds { get; set; }
    }
}