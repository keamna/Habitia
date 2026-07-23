namespace Habitia.ViewModels.Marketplace
{
    public class PerfilVendedorVM
    {
        public string Id { get; set; } = "";

        public string NombreCompleto { get; set; } = "";

        public string? FotoPerfil { get; set; }


        public double PromedioEstrellas { get; set; }

        public int TotalResenas { get; set; }

        public int TotalPublicaciones { get; set; }


        public int EstrellasLlenas => (int)Math.Round(PromedioEstrellas);
    }
}