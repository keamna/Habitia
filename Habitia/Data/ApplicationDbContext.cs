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
        public DbSet<ImagenPublicacion> ImagenesPublicacion { get; set; }
        public DbSet<AreaComunFoto> AreaComunFotos { get; set; }

        // Catálogos
        public DbSet<TipoArea> TiposArea { get; set; }
        public DbSet<TipoMantenimiento> TiposMantenimiento { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ==========================================================
            // Identity personalizado
            // ==========================================================
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

            // ==========================================================
            // Incidencia
            // ==========================================================
            builder.Entity<Incidencia>(entity =>
            {
                // Usuario que reporta la incidencia (residente o seguridad)
                entity.HasOne(i => i.Usuario)
                    .WithMany()
                    .HasForeignKey(i => i.TC_IdUsuario)
                    .OnDelete(DeleteBehavior.Restrict);

                // Vivienda (opcional: solo si Tipo == Vivienda)
                entity.HasOne(i => i.Vivienda)
                    .WithMany()
                    .HasForeignKey(i => i.TN_IdVivienda)
                    .OnDelete(DeleteBehavior.Restrict);

                // Área común (opcional: solo si Tipo == AreaComun)
                entity.HasOne(i => i.AreaComun)
                    .WithMany()
                    .HasForeignKey(i => i.TN_IdAreaComun)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices para acelerar filtros de reportes (estado, responsabilidad, fecha)
                entity.HasIndex(i => i.TN_Estado);
                entity.HasIndex(i => i.TN_Responsabilidad);
                entity.HasIndex(i => i.TF_FechaRegistro);
            });

            // ==========================================================
            // Mantenimiento
            // ==========================================================
            builder.Entity<Mantenimiento>(entity =>
            {
                // Trazabilidad: una incidencia genera, a lo sumo, una tarea de mantenimiento
                entity.HasOne(m => m.Incidencia)
                    .WithMany()
                    .HasForeignKey(m => m.TN_IdIncidencia)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(m => m.TN_IdIncidencia)
                    .IsUnique(); // evita generar más de una tarea por incidencia

                // Tipo de mantenimiento (catálogo dinámico)
                entity.HasOne(m => m.Tipo)
                    .WithMany(t => t.Mantenimientos)
                    .HasForeignKey(m => m.TN_IdTipo)
                    .OnDelete(DeleteBehavior.Restrict);

                // Área común afectada (opcional)
                entity.HasOne(m => m.AreaComun)
                    .WithMany()
                    .HasForeignKey(m => m.TN_IdAreaComun)
                    .OnDelete(DeleteBehavior.Restrict);

                // Personal de mantenimiento asignado
                entity.HasOne(m => m.PersonalAsignado)
                    .WithMany()
                    .HasForeignKey(m => m.TC_IdPersonalAsignado)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(m => m.TN_Estado);
                entity.HasIndex(m => m.TF_FechaProgramada);
            });

            // ==========================================================
            // TipoMantenimiento (catálogo)
            // ==========================================================
            builder.Entity<TipoMantenimiento>(entity =>
            {
                // Regla de negocio: "el sistema evitará duplicidad de tipos"
                // Se refuerza a nivel de BD además de validarse en el Service.
                entity.HasIndex(t => t.TC_Nombre)
                    .IsUnique();
            });

            builder.Entity<Vivienda>(entity =>
            {
                entity.HasIndex(v => v.TC_Numero)
                    .IsUnique();
            });
        }
    }
}