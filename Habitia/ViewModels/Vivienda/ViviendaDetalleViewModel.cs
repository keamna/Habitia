// /ViewModels/Vivienda/ViviendaDetalleViewModel.cs
using Habitia.Enums;

namespace Habitia.ViewModels.Vivienda
{
    public class ViviendaDetalleViewModel
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public TipoViviendaEnum Tipo { get; set; }
        public EstadoViviendaEnum Estado { get; set; }
        public int CantidadInquilinos { get; set; }
        public string EtiquetaInquilinos { get; set; } = "Inquilinos";

        // Antes solo se guardaba el nombre; ahora se guarda el usuario completo
        // para poder desplegar sus datos con la flecha.
        public UsuarioDetalleViewModel? Propietario { get; set; }

        public List<UsuarioDetalleViewModel> UsuariosAsociados { get; set; } = new();
    }

    public class UsuarioDetalleViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Identificacion { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? FotoPerfil { get; set; }
        public TipoRelacionEnum TipoRelacion { get; set; }
        public EstadoUsuarioEnum Estado { get; set; }
        public bool ViveAhi { get; set; }
    }
}