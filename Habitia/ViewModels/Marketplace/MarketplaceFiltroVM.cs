using Habitia.Enums;

namespace Habitia.ViewModels.Marketplace
{
    public class MarketplaceFiltroVM
    {
        public string? Busqueda { get; set; }

        public TipoPublicacionEnum? Tipo { get; set; }

        public int? IdCategoria { get; set; }

        public bool SoloMias { get; set; }


        // Filtro por vendedor
        public string? IdVendedor { get; set; }

        public PerfilVendedorVM? VendedorFiltrado { get; set; }


        // Vendedores que coinciden con la búsqueda
        public List<PerfilVendedorVM> Vendedores { get; set; }
            = new List<PerfilVendedorVM>();


        public List<Habitia.Models.Publicacion> Publicaciones { get; set; }
            = new List<Habitia.Models.Publicacion>();


        public PerfilVendedorVM MiPerfil { get; set; } = new PerfilVendedorVM();
    }
}