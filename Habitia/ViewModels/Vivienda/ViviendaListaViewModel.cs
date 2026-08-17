using Habitia.Enums;
namespace Habitia.ViewModels.Vivienda
{
    public class ViviendaListaViewModel
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public TipoViviendaEnum Tipo { get; set; }
        public EstadoViviendaEnum Estado { get; set; }
        public int CantidadInquilinos { get; set; }
        public string? PropietarioNombre { get; set; }
        public int InquilinosActuales { get; set; }
        public bool ViveAhi { get; set; }
        public bool EsPropietario { get; set; }
        // "Familiares" o "Inquilinos" según si el propietario vive ahí.
        public string EtiquetaInquilinos { get; set; } = "Inquilinos";

        public EstadoUsuarioEnum EstadoRelacion { get; set; }
    }
}