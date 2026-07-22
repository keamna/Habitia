using Habitia.Data;
using Habitia.Models.Catalogos;

namespace Habitia.Data.Seed
{
    public static class DbSeeder
    {
        public static void SeedTiposArea(ApplicationDbContext context)
        {
            if (context.TiposArea.Any())
                return;

            var tipos = new List<TipoArea>
            {
                new TipoArea { Nombre = "Piscina", Estado = true },
                new TipoArea { Nombre = "Gimnasio", Estado = true },
                new TipoArea { Nombre = "Salón de eventos", Estado = true },
                new TipoArea { Nombre = "Cancha deportiva", Estado = true },
                new TipoArea { Nombre = "Zona de juegos infantiles", Estado = true }
            };

            context.TiposArea.AddRange(tipos);
            context.SaveChanges();
        }
    }
}