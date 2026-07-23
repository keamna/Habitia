namespace Habitia.ViewModels.Marketplace
{
    public class SugerenciaVM
    {
        // "publicacion", "categoria" o "residente"
        public string Tipo { get; set; } = "";

        public string Id { get; set; } = "";

        public string Texto { get; set; } = "";

        public string Extra { get; set; } = "";

        public string? Foto { get; set; }
    }
}