using Habitia.Enums;
using Habitia.Models;
using Habitia.Models.Acceso;
using Habitia.Models.Catalogos;
using Habitia.Models.Documentos;
using Habitia.Models.Financiero;
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
        public DbSet<Acceso> Accesos { get; set; }
        public DbSet<AreaComun> AreasComunes { get; set; }
        public DbSet<DisponibilidadArea> Disponibilidades { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<ExpiracionReserva> ExpiracionesReserva { get; set; }
        public DbSet<Incidencia> Incidencias { get; set; }
        public DbSet<Mantenimiento> Mantenimientos { get; set; }
        public DbSet<ImagenPublicacion> ImagenesPublicacion { get; set; }
        public DbSet<Publicacion> Publicaciones { get; set; }
        public DbSet<THBT_CAT_CategoriaPublicacion> CategoriasPublicacion { get; set; }

        public DbSet<THBT_A_Pago> Pagos { get; set; }
        public DbSet<THBT_A_Cargo> Cargos { get; set; }
        public DbSet<THBT_A_ConfiguracionPago> ConfiguracionesPago { get; set; }
        public DbSet<THBT_CAT_EstadoCargo> EstadosCargo { get; set; }

        public DbSet<THBT_CAT_MetodoPago> MetodosPago { get; set; }
        public DbSet<THBT_CAT_TipoCargo> TiposCargo { get; set; }
        public DbSet<THBT_CAT_TipoRecargo> TiposRecargo { get; set; }
        public DbSet<THBT_CAT_TipoTarjeta> TiposTarjeta { get; set; }
        public DbSet<THBT_H_Cargo> HistorialCargos { get; set; }
        public DbSet<THBT_H_Pago> HistorialPagos { get; set; }
        public DbSet<THBT_A_Recargo> Recargos { get; set; }

        public DbSet<ResenaPublicacion> ResenasPublicacion { get; set; }

        public DbSet<AreaComunFoto> AreaComunFotos { get; set; }

        public DbSet<THBT_H_ConfiguracionPago> HistorialConfiguracionesPago { get; set; }

        // Catálogos
        public DbSet<TipoArea> TiposArea { get; set; }
        public DbSet<TipoMantenimiento> TiposMantenimiento { get; set; }

        // ===== US-11: Documentos y Asambleas =====
        public DbSet<Documento> Documentos { get; set; }
        public DbSet<Asamblea> Asambleas { get; set; }
        public DbSet<ParticipanteAsamblea> ParticipantesAsamblea { get; set; }
        public DbSet<DocumentoAsamblea> DocumentosAsamblea { get; set; }
        public DbSet<CategoriaDocumento> CategoriasDocumento { get; set; }

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

            // ==========================================================
            // Visitante / Vehiculo (catálogo reutilizable)
            // ==========================================================
            builder.Entity<Vehiculo>(entity =>
            {
                entity.HasOne(v => v.Visitante)
                    .WithMany(vi => vi.Vehiculos)
                    .HasForeignKey(v => v.TN_IdVisitante)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ==========================================================
            // Autorizacion (US-05: generada por el residente, código QR)
            // ==========================================================
            builder.Entity<Autorizacion>(entity =>
            {
                entity.HasOne(a => a.Visitante)
                    .WithMany(v => v.Autorizaciones)
                    .HasForeignKey(a => a.TN_IdVisitante)
                    .OnDelete(DeleteBehavior.Restrict);

                // Residente que autoriza
                entity.HasOne(a => a.Usuario)
                    .WithMany()
                    .HasForeignKey(a => a.TC_IdUsuario)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Vivienda)
                    .WithMany()
                    .HasForeignKey(a => a.TN_IdVivienda)
                    .OnDelete(DeleteBehavior.Restrict);

                // El código QR debe ser único para que la validación en garita no sea ambigua
                entity.HasIndex(a => a.TC_Codigo)
                    .IsUnique();

                // Acelera "consultar autorizaciones activas / vencidas" (US-05, punto 5)
                entity.HasIndex(a => a.TN_Estado);
                entity.HasIndex(a => a.TF_FechaVencimiento);
            });

            // ==========================================================
            // Acceso (US-04: registro manual o validación de QR)
            // ==========================================================
            builder.Entity<Acceso>(entity =>
            {
                entity.HasOne(a => a.Visitante)
                    .WithMany()
                    .HasForeignKey(a => a.TN_IdVisitante)
                    .OnDelete(DeleteBehavior.Restrict);

                // Residente que autoriza o recibe la visita
                entity.HasOne(a => a.Usuario)
                    .WithMany()
                    .HasForeignKey(a => a.TC_IdUsuario)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Vivienda)
                    .WithMany()
                    .HasForeignKey(a => a.TN_IdVivienda)
                    .OnDelete(DeleteBehavior.Restrict);

                // Opcional: solo si el ingreso nació de validar un QR (US-04, punto 3)
                entity.HasOne(a => a.Autorizacion)
                    .WithMany(au => au.Accesos)
                    .HasForeignKey(a => a.TN_IdAutorizacion)
                    .OnDelete(DeleteBehavior.Restrict);

                // Opcional: solo si el visitante ingresó en vehículo (US-04, punto 4)
                entity.HasOne(a => a.Vehiculo)
                    .WithMany()
                    .HasForeignKey(a => a.TN_IdVehiculo)
                    .OnDelete(DeleteBehavior.Restrict);

                // Evita usar la misma autorización para crear dos accesos distintos
                entity.HasIndex(a => a.TN_IdAutorizacion)
                    .IsUnique()
                    .HasFilter("[TN_IdAutorizacion] IS NOT NULL");

                // Acelera "quién está dentro ahora mismo" (TF_FechaSalida IS NULL) y el historial
                entity.HasIndex(a => a.TF_FechaIngreso);
                entity.HasIndex(a => a.TF_FechaSalida);
            });

            // ==========================================================
            // Financiero (US-10)
            // ==========================================================
            builder.Entity<THBT_A_Cargo>(entity =>
            {
                // Residente al que pertenece el cargo
                entity.HasOne(c => c.Residente)
                    .WithMany()
                    .HasForeignKey(c => c.TC_IdResidente)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.TipoCargo)
                    .WithMany()
                    .HasForeignKey(c => c.TN_IdTipoCargo)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.EstadoCargo)
                    .WithMany()
                    .HasForeignKey(c => c.TN_IdEstadoCargo)
                    .OnDelete(DeleteBehavior.Restrict);

                // Acelera "cargos pendientes del residente" y "cargos vencidos" (US-10, puntos 2.2 y 4.1)
                entity.HasIndex(c => c.TN_IdEstadoCargo);
                entity.HasIndex(c => c.TF_FechaVencimiento);
            });

            builder.Entity<THBT_A_Pago>(entity =>
            {
                entity.HasOne(p => p.Cargo)
                    .WithMany()
                    .HasForeignKey(p => p.TN_IdCargo)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.MetodoPago)
                    .WithMany()
                    .HasForeignKey(p => p.TN_IdMetodoPago)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<THBT_A_Recargo>(entity =>
            {
                entity.HasOne(r => r.Cargo)
                    .WithMany()
                    .HasForeignKey(r => r.TN_IdCargo)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.TipoRecargo)
                    .WithMany()
                    .HasForeignKey(r => r.TN_IdTipoRecargo)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ResenaPublicacion>(entity =>
            {
                entity.HasOne(r => r.Usuario)
                    .WithMany()
                    .HasForeignKey(r => r.TC_IdUsuario)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Publicacion)
                    .WithMany(p => p.Resenas) 
                    .HasForeignKey(r => r.TN_IdPublicacion)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Regla de negocio: no se permiten tipos de cargo duplicados (igual que TipoMantenimiento)
            builder.Entity<THBT_CAT_TipoCargo>(entity =>
            {
                entity.HasIndex(t => t.TC_Nombre)
                    .IsUnique();
            });

            // ==========================================================
            // US-11: Documentos y Asambleas
            // ==========================================================
            builder.Entity<Documento>(entity =>
            {
                entity.HasOne(d => d.Categoria)
                    .WithMany(c => c.Documentos)
                    .HasForeignKey(d => d.TN_IdCategoria)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Usuario)
                    .WithMany()
                    .HasForeignKey(d => d.TC_IdUsuario)
                    .OnDelete(DeleteBehavior.Restrict);

                // Acelera "documentos activos por categoría" (vista del residente)
                entity.HasIndex(d => d.TB_Estado);
                entity.HasIndex(d => d.TN_IdCategoria);
            });

            // Regla de negocio: no se permiten categorías duplicadas
            builder.Entity<CategoriaDocumento>(entity =>
            {
                entity.HasIndex(c => c.TC_Nombre)
                    .IsUnique();
            });

            builder.Entity<Asamblea>(entity =>
            {
                entity.HasOne(a => a.Usuario)
                    .WithMany()
                    .HasForeignKey(a => a.TC_IdUsuario)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(a => a.TN_Estado);
                entity.HasIndex(a => a.TF_FechaHora);
            });

            builder.Entity<ParticipanteAsamblea>(entity =>
            {
                entity.HasOne(p => p.Asamblea)
                    .WithMany(a => a.Participantes)
                    .HasForeignKey(p => p.TN_IdAsamblea)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Usuario)
                    .WithMany()
                    .HasForeignKey(p => p.TC_IdUsuario)
                    .OnDelete(DeleteBehavior.Restrict);

                // Un residente no puede confirmar dos veces la misma asamblea
                entity.HasIndex(p => new { p.TN_IdAsamblea, p.TC_IdUsuario })
                    .IsUnique();
            });

            builder.Entity<DocumentoAsamblea>(entity =>
            {
                entity.HasOne(da => da.Asamblea)
                    .WithMany(a => a.Documentos)
                    .HasForeignKey(da => da.TN_IdAsamblea)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(da => da.Documento)
                    .WithMany(d => d.Asambleas)
                    .HasForeignKey(da => da.TN_IdDocumento)
                    .OnDelete(DeleteBehavior.Restrict);

                // Evita asociar el mismo documento dos veces a una asamblea
                entity.HasIndex(da => new { da.TN_IdAsamblea, da.TN_IdDocumento })
                    .IsUnique();
            });

            // Seed de categorías de documento
            builder.Entity<CategoriaDocumento>().HasData(
                new CategoriaDocumento { TN_Id = 1, TC_Nombre = "Reglamentos", TB_Estado = true },
                new CategoriaDocumento { TN_Id = 2, TC_Nombre = "Actas", TB_Estado = true },
                new CategoriaDocumento { TN_Id = 3, TC_Nombre = "Comunicados", TB_Estado = true },
                new CategoriaDocumento { TN_Id = 4, TC_Nombre = "Otros", TB_Estado = true }
            );

            // ==========================================================
            // Seed de catálogos Financiero
            // ==========================================================
            builder.Entity<THBT_CAT_EstadoCargo>().HasData(
                new THBT_CAT_EstadoCargo { TN_Id = 1, TC_Nombre = "Pendiente" },
                new THBT_CAT_EstadoCargo { TN_Id = 2, TC_Nombre = "En revisión" },
                new THBT_CAT_EstadoCargo { TN_Id = 3, TC_Nombre = "Pagado" }
            );

            builder.Entity<THBT_CAT_MetodoPago>().HasData(
                new THBT_CAT_MetodoPago { TN_Id = 1, TC_Nombre = "Efectivo" },
                new THBT_CAT_MetodoPago { TN_Id = 2, TC_Nombre = "Tarjeta" },
                new THBT_CAT_MetodoPago { TN_Id = 3, TC_Nombre = "SINPE" }
            );

            builder.Entity<THBT_CAT_TipoRecargo>().HasData(
                new THBT_CAT_TipoRecargo { TN_Id = 1, TC_Nombre = "Fijo" },
                new THBT_CAT_TipoRecargo { TN_Id = 2, TC_Nombre = "Porcentaje" }
            );
        }
    }
}