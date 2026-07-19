using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class _01Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "THBT_A_ExpiracionReserva",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_ExpiracionReserva", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Rol",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Rol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Usuario",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoIdentificacion = table.Column<int>(type: "int", nullable: false),
                    Identificacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Usuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Visitante",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identificacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TipoIdentificacion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Visitante", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Vivienda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    CantidadInquilinos = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Vivienda", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_CAT_CategoriaPublicacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_CAT_CategoriaPublicacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_CAT_TipoArea",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_CAT_TipoArea", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_CAT_TipoMantenimiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_CAT_TipoMantenimiento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_RolClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_RolClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_RolClaim_THBT_A_Rol_RoleId",
                        column: x => x.RoleId,
                        principalTable: "THBT_A_Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_UsuarioClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_UsuarioClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_UsuarioClaim_THBT_A_Usuario_UserId",
                        column: x => x.UserId,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_UsuarioLogin",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_UsuarioLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_THBT_A_UsuarioLogin_THBT_A_Usuario_UserId",
                        column: x => x.UserId,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_UsuarioRol",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_UsuarioRol", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_THBT_A_UsuarioRol_THBT_A_Rol_RoleId",
                        column: x => x.RoleId,
                        principalTable: "THBT_A_Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_THBT_A_UsuarioRol_THBT_A_Usuario_UserId",
                        column: x => x.UserId,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_UsuarioToken",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_UsuarioToken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_THBT_A_UsuarioToken_THBT_A_Usuario_UserId",
                        column: x => x.UserId,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Vehiculo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdVisitante = table.Column<int>(type: "int", nullable: false),
                    Placa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Vehiculo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Vehiculo_THBT_A_Visitante_IdVisitante",
                        column: x => x.IdVisitante,
                        principalTable: "THBT_A_Visitante",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Autorizacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdVisitante = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdVivienda = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Inicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Autorizacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Autorizacion_THBT_A_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_THBT_A_Autorizacion_THBT_A_Visitante_IdVisitante",
                        column: x => x.IdVisitante,
                        principalTable: "THBT_A_Visitante",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_THBT_A_Autorizacion_THBT_A_Vivienda_IdVivienda",
                        column: x => x.IdVivienda,
                        principalTable: "THBT_A_Vivienda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_ViviendaUsuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdVivienda = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TipoRelacion = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    ViveAhi = table.Column<bool>(type: "bit", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_ViviendaUsuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_ViviendaUsuario_THBT_A_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_THBT_A_ViviendaUsuario_THBT_A_Vivienda_IdVivienda",
                        column: x => x.IdVivienda,
                        principalTable: "THBT_A_Vivienda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Publicacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdCategoria = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Publicacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Publicacion_THBT_A_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_THBT_A_Publicacion_THBT_CAT_CategoriaPublicacion_IdCategoria",
                        column: x => x.IdCategoria,
                        principalTable: "THBT_CAT_CategoriaPublicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_AreaComun",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IdTipo = table.Column<int>(type: "int", nullable: false),
                    Capacidad = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_AreaComun", x => x.Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_AreaComun_THBT_CAT_TipoArea_IdTipo",
                        column: x => x.IdTipo,
                        principalTable: "THBT_CAT_TipoArea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Acceso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAutorizacion = table.Column<int>(type: "int", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaSalida = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Acceso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Acceso_THBT_A_Autorizacion_IdAutorizacion",
                        column: x => x.IdAutorizacion,
                        principalTable: "THBT_A_Autorizacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_ImagenPublicacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPublicacion = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_ImagenPublicacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_ImagenPublicacion_THBT_A_Publicacion_IdPublicacion",
                        column: x => x.IdPublicacion,
                        principalTable: "THBT_A_Publicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_AreaComunFoto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAreaComun = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    EsPrincipal = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_AreaComunFoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_AreaComunFoto_THBT_A_AreaComun_IdAreaComun",
                        column: x => x.IdAreaComun,
                        principalTable: "THBT_A_AreaComun",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_DisponibilidadArea",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAreaComun = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_DisponibilidadArea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_DisponibilidadArea_THBT_A_AreaComun_IdAreaComun",
                        column: x => x.IdAreaComun,
                        principalTable: "THBT_A_AreaComun",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Incidencia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdVivienda = table.Column<int>(type: "int", nullable: true),
                    IdAreaComun = table.Column<int>(type: "int", nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Responsabilidad = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImagenUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ComentarioAdicional = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Incidencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Incidencia_THBT_A_AreaComun_IdAreaComun",
                        column: x => x.IdAreaComun,
                        principalTable: "THBT_A_AreaComun",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Incidencia_THBT_A_Usuario_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_THBT_A_Incidencia_THBT_A_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Incidencia_THBT_A_Vivienda_IdVivienda",
                        column: x => x.IdVivienda,
                        principalTable: "THBT_A_Vivienda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Reserva",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdVivienda = table.Column<int>(type: "int", nullable: false),
                    IdDisponibilidad = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Reserva", x => x.Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Reserva_THBT_A_DisponibilidadArea_IdDisponibilidad",
                        column: x => x.IdDisponibilidad,
                        principalTable: "THBT_A_DisponibilidadArea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_THBT_A_Reserva_THBT_A_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_THBT_A_Reserva_THBT_A_Vivienda_IdVivienda",
                        column: x => x.IdVivienda,
                        principalTable: "THBT_A_Vivienda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Mantenimiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdIncidencia = table.Column<int>(type: "int", nullable: false),
                    IdTipo = table.Column<int>(type: "int", nullable: false),
                    IdAreaComun = table.Column<int>(type: "int", nullable: true),
                    IdPersonalAsignado = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaProgramada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Mantenimiento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Mantenimiento_THBT_A_AreaComun_IdAreaComun",
                        column: x => x.IdAreaComun,
                        principalTable: "THBT_A_AreaComun",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Mantenimiento_THBT_A_Incidencia_IdIncidencia",
                        column: x => x.IdIncidencia,
                        principalTable: "THBT_A_Incidencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Mantenimiento_THBT_A_Usuario_IdPersonalAsignado",
                        column: x => x.IdPersonalAsignado,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Mantenimiento_THBT_CAT_TipoMantenimiento_IdTipo",
                        column: x => x.IdTipo,
                        principalTable: "THBT_CAT_TipoMantenimiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Acceso_IdAutorizacion",
                table: "THBT_A_Acceso",
                column: "IdAutorizacion");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_AreaComun_IdTipo",
                table: "THBT_A_AreaComun",
                column: "IdTipo");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_AreaComunFoto_IdAreaComun",
                table: "THBT_A_AreaComunFoto",
                column: "IdAreaComun");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Autorizacion_IdUsuario",
                table: "THBT_A_Autorizacion",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Autorizacion_IdVisitante",
                table: "THBT_A_Autorizacion",
                column: "IdVisitante");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Autorizacion_IdVivienda",
                table: "THBT_A_Autorizacion",
                column: "IdVivienda");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_DisponibilidadArea_IdAreaComun",
                table: "THBT_A_DisponibilidadArea",
                column: "IdAreaComun");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_ImagenPublicacion_IdPublicacion",
                table: "THBT_A_ImagenPublicacion",
                column: "IdPublicacion");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_ApplicationUserId",
                table: "THBT_A_Incidencia",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_Estado",
                table: "THBT_A_Incidencia",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_FechaRegistro",
                table: "THBT_A_Incidencia",
                column: "FechaRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_IdAreaComun",
                table: "THBT_A_Incidencia",
                column: "IdAreaComun");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_IdUsuario",
                table: "THBT_A_Incidencia",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_IdVivienda",
                table: "THBT_A_Incidencia",
                column: "IdVivienda");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_Responsabilidad",
                table: "THBT_A_Incidencia",
                column: "Responsabilidad");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Mantenimiento_Estado",
                table: "THBT_A_Mantenimiento",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Mantenimiento_FechaProgramada",
                table: "THBT_A_Mantenimiento",
                column: "FechaProgramada");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Mantenimiento_IdAreaComun",
                table: "THBT_A_Mantenimiento",
                column: "IdAreaComun");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Mantenimiento_IdIncidencia",
                table: "THBT_A_Mantenimiento",
                column: "IdIncidencia",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Mantenimiento_IdPersonalAsignado",
                table: "THBT_A_Mantenimiento",
                column: "IdPersonalAsignado");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Mantenimiento_IdTipo",
                table: "THBT_A_Mantenimiento",
                column: "IdTipo");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Publicacion_IdCategoria",
                table: "THBT_A_Publicacion",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Publicacion_IdUsuario",
                table: "THBT_A_Publicacion",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Reserva_IdDisponibilidad",
                table: "THBT_A_Reserva",
                column: "IdDisponibilidad");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Reserva_IdUsuario",
                table: "THBT_A_Reserva",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Reserva_IdVivienda",
                table: "THBT_A_Reserva",
                column: "IdVivienda");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "THBT_A_Rol",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_RolClaim_RoleId",
                table: "THBT_A_RolClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "THBT_A_Usuario",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "THBT_A_Usuario",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_UsuarioClaim_UserId",
                table: "THBT_A_UsuarioClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_UsuarioLogin_UserId",
                table: "THBT_A_UsuarioLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_UsuarioRol_RoleId",
                table: "THBT_A_UsuarioRol",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Vehiculo_IdVisitante",
                table: "THBT_A_Vehiculo",
                column: "IdVisitante");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_ViviendaUsuario_IdUsuario",
                table: "THBT_A_ViviendaUsuario",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_ViviendaUsuario_IdVivienda",
                table: "THBT_A_ViviendaUsuario",
                column: "IdVivienda");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_CAT_TipoMantenimiento_Nombre",
                table: "THBT_CAT_TipoMantenimiento",
                column: "Nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "THBT_A_Acceso");

            migrationBuilder.DropTable(
                name: "THBT_A_AreaComunFoto");

            migrationBuilder.DropTable(
                name: "THBT_A_ExpiracionReserva");

            migrationBuilder.DropTable(
                name: "THBT_A_ImagenPublicacion");

            migrationBuilder.DropTable(
                name: "THBT_A_Mantenimiento");

            migrationBuilder.DropTable(
                name: "THBT_A_Reserva");

            migrationBuilder.DropTable(
                name: "THBT_A_RolClaim");

            migrationBuilder.DropTable(
                name: "THBT_A_UsuarioClaim");

            migrationBuilder.DropTable(
                name: "THBT_A_UsuarioLogin");

            migrationBuilder.DropTable(
                name: "THBT_A_UsuarioRol");

            migrationBuilder.DropTable(
                name: "THBT_A_UsuarioToken");

            migrationBuilder.DropTable(
                name: "THBT_A_Vehiculo");

            migrationBuilder.DropTable(
                name: "THBT_A_ViviendaUsuario");

            migrationBuilder.DropTable(
                name: "THBT_A_Autorizacion");

            migrationBuilder.DropTable(
                name: "THBT_A_Publicacion");

            migrationBuilder.DropTable(
                name: "THBT_A_Incidencia");

            migrationBuilder.DropTable(
                name: "THBT_CAT_TipoMantenimiento");

            migrationBuilder.DropTable(
                name: "THBT_A_DisponibilidadArea");

            migrationBuilder.DropTable(
                name: "THBT_A_Rol");

            migrationBuilder.DropTable(
                name: "THBT_A_Visitante");

            migrationBuilder.DropTable(
                name: "THBT_CAT_CategoriaPublicacion");

            migrationBuilder.DropTable(
                name: "THBT_A_Usuario");

            migrationBuilder.DropTable(
                name: "THBT_A_Vivienda");

            migrationBuilder.DropTable(
                name: "THBT_A_AreaComun");

            migrationBuilder.DropTable(
                name: "THBT_CAT_TipoArea");
        }
    }
}
