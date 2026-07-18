using Habitia.Models;
using Habitia.Models.Catalogos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }


        public DbSet<Vivienda> Viviendas { get; set; }

        public DbSet<ViviendaUsuario> ViviendaUsuarios { get; set; }

        public DbSet<Visitante> Visitantes { get; set; }

        public DbSet<Vehiculo> Vehiculos { get; set; }

        public DbSet<Autorizacion> Autorizaciones { get; set; }

        public DbSet<AreaComun> AreasComunes { get; set; }

        public DbSet<DisponibilidadArea> Disponibilidades { get; set; }

        public DbSet<Reserva> Reservas { get; set; }

        public DbSet<ExpiracionReserva> ExpiracionesReserva { get; set; }

        public DbSet<Incidencia> Incidencias { get; set; }

        public DbSet<Mantenimiento> Mantenimientos { get; set; }

        public DbSet<Publicacion> Publicaciones { get; set; }

        public DbSet<ImagenPublicacion> ImagenesPublicacion { get; set; }

        public DbSet<AreaComunFoto> AreaComunFotos { get; set; }


        // Catálogos

        public DbSet<TipoArea> TiposArea { get; set; }

        public DbSet<TipoMantenimiento> TiposMantenimiento { get; set; }

        public DbSet<THBT_CAT_CategoriaPublicacion> CategoriasPublicacion { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);



            // Identity personalizado

            builder.Entity<ApplicationUser>()
                .ToTable("THBT_A_Usuario");


            builder.Entity<IdentityRole>()
                .ToTable("THBT_A_Rol");


            builder.Entity<IdentityUserRole<string>>()
                .ToTable("THBT_A_UsuarioRol");


            builder.Entity<IdentityUserClaim<string>>()
                .ToTable("THBT_A_UsuarioClaim");


            builder.Entity<IdentityUserLogin<string>>()
                .ToTable("THBT_A_UsuarioLogin");


            builder.Entity<IdentityRoleClaim<string>>()
                .ToTable("THBT_A_RolClaim");


            builder.Entity<IdentityUserToken<string>>()
                .ToTable("THBT_A_UsuarioToken");



            // Relación usuario - roles

            builder.Entity<IdentityUserRole<string>>()
                .HasKey(x => new
                {
                    x.UserId,
                    x.RoleId
                });
        }
    }
}