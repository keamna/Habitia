namespace Habitia.ViewModels.Marketplace
{
    public class DetallePublicacionVM
    {
        public Habitia.Models.Publicacion Publicacion { get; set; }


        public bool EsPropia { get; set; }


        // Perfil del vendedor (dueño de la publicación)

        public PerfilVendedorVM Vendedor { get; set; } = new PerfilVendedorVM();


        // Reseñas de esta publicación

        public List<Habitia.Models.ResenaPublicacion> Resenas { get; set; }
            = new List<Habitia.Models.ResenaPublicacion>();


        public double PromedioPublicacion { get; set; }


        // Reseña que ya dejó el usuario actual (si existe)

        public Habitia.Models.ResenaPublicacion? MiResena { get; set; }
    }
}