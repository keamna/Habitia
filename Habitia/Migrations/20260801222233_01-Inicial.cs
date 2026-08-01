using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class _01Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "THBT_A_ConfiguracionPago",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TB_EfectivoHabilitado = table.Column<bool>(type: "bit", nullable: false),
                    TB_TarjetaHabilitado = table.Column<bool>(type: "bit", nullable: false),
                    TB_SinpeHabilitado = table.Column<bool>(type: "bit", nullable: false),
                    TC_TitularTarjeta = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TC_IbanTarjeta = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    TC_TitularSinpe = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TC_NumeroSinpe = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    TF_FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TB_Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_ConfiguracionPago", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_ExpiracionReserva",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_Cantidad = table.Column<int>(type: "int", nullable: false),
                    TN_Tipo = table.Column<int>(type: "int", nullable: false),
                    TB_Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_ExpiracionReserva", x => x.TN_Id);
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
                    TC_Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TC_Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TN_TipoIdentificacion = table.Column<int>(type: "int", nullable: false),
                    TC_Identificacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TC_Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TN_Estado = table.Column<int>(type: "int", nullable: false),
                    TF_FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TC_FotoPerfil = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
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
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_TipoIdentificacion = table.Column<int>(type: "int", nullable: false),
                    TC_Identificacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TC_Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TC_Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TB_Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Visitante", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Vivienda",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Numero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TN_Tipo = table.Column<int>(type: "int", nullable: false),
                    TN_Estado = table.Column<int>(type: "int", nullable: false),
                    TN_CantidadInquilinos = table.Column<int>(type: "int", nullable: false),
                    TF_FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Vivienda", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_CAT_CategoriaPublicacion",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TB_Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_CAT_CategoriaPublicacion", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_CAT_EstadoCargo",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_CAT_EstadoCargo", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_CAT_MetodoPago",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_CAT_MetodoPago", x => x.TN_Id);
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
                name: "THBT_CAT_TipoCargo",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_CAT_TipoCargo", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_CAT_TipoMantenimiento",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TB_Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_CAT_TipoMantenimiento", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_CAT_TipoRecargo",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_CAT_TipoRecargo", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_CAT_TipoTarjeta",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_CAT_TipoTarjeta", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_H_Cargo",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdCargo = table.Column<int>(type: "int", nullable: false),
                    TN_IdEstadoCargoAnterior = table.Column<int>(type: "int", nullable: false),
                    TN_IdEstadoCargoNuevo = table.Column<int>(type: "int", nullable: false),
                    TF_FechaCambio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TC_IdUsuarioCambio = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_H_Cargo", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_H_ConfiguracionPago",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdConfiguracionPago = table.Column<int>(type: "int", nullable: false),
                    TB_EfectivoHabilitado = table.Column<bool>(type: "bit", nullable: false),
                    TB_TarjetaHabilitado = table.Column<bool>(type: "bit", nullable: false),
                    TB_SinpeHabilitado = table.Column<bool>(type: "bit", nullable: false),
                    TC_TitularTarjeta = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TC_IbanTarjeta = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    TC_TitularSinpe = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TC_NumeroSinpe = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    TF_FechaCambio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TC_IdUsuarioCambio = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_H_ConfiguracionPago", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_H_Pago",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdPago = table.Column<int>(type: "int", nullable: false),
                    TB_Aprobado = table.Column<bool>(type: "bit", nullable: false),
                    TC_MotivoRechazo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TF_FechaCambio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TC_IdUsuarioCambio = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_H_Pago", x => x.TN_Id);
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
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdVisitante = table.Column<int>(type: "int", nullable: false),
                    TC_Placa = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TC_Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TC_Observaciones = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TB_Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Vehiculo", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Vehiculo_THBT_A_Visitante_TN_IdVisitante",
                        column: x => x.TN_IdVisitante,
                        principalTable: "THBT_A_Visitante",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Autorizacion",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdVisitante = table.Column<int>(type: "int", nullable: false),
                    TC_IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TN_IdVivienda = table.Column<int>(type: "int", nullable: false),
                    TC_Codigo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TF_FechaVisita = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TF_FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TC_Motivo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TF_FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TN_Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Autorizacion", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Autorizacion_THBT_A_Usuario_TC_IdUsuario",
                        column: x => x.TC_IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Autorizacion_THBT_A_Visitante_TN_IdVisitante",
                        column: x => x.TN_IdVisitante,
                        principalTable: "THBT_A_Visitante",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Autorizacion_THBT_A_Vivienda_TN_IdVivienda",
                        column: x => x.TN_IdVivienda,
                        principalTable: "THBT_A_Vivienda",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_ViviendaUsuario",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdVivienda = table.Column<int>(type: "int", nullable: false),
                    TC_IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TN_TipoRelacion = table.Column<int>(type: "int", nullable: false),
                    TN_Estado = table.Column<int>(type: "int", nullable: false),
                    TB_ViveAhi = table.Column<bool>(type: "bit", nullable: false),
                    TF_FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_ViviendaUsuario", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_ViviendaUsuario_THBT_A_Usuario_TC_IdUsuario",
                        column: x => x.TC_IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_THBT_A_ViviendaUsuario_THBT_A_Vivienda_TN_IdVivienda",
                        column: x => x.TN_IdVivienda,
                        principalTable: "THBT_A_Vivienda",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Publicacion",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TN_IdCategoria = table.Column<int>(type: "int", nullable: false),
                    TC_Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TC_Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TN_Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TN_Estado = table.Column<int>(type: "int", nullable: false),
                    TN_Tipo = table.Column<int>(type: "int", nullable: false),
                    TF_FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TC_Especificaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TC_Contacto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TF_FechaServicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TT_HoraInicioServicio = table.Column<TimeSpan>(type: "time", nullable: true),
                    TT_HoraFinServicio = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Publicacion", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Publicacion_THBT_A_Usuario_TC_IdUsuario",
                        column: x => x.TC_IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_THBT_A_Publicacion_THBT_CAT_CategoriaPublicacion_TN_IdCategoria",
                        column: x => x.TN_IdCategoria,
                        principalTable: "THBT_CAT_CategoriaPublicacion",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_AreaComun",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TC_Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TN_IdTipo = table.Column<int>(type: "int", nullable: false),
                    TN_Capacidad = table.Column<int>(type: "int", nullable: false),
                    TB_Estado = table.Column<bool>(type: "bit", nullable: false),
                    TN_AnticipacionMinima = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_AreaComun", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_AreaComun_THBT_CAT_TipoArea_TN_IdTipo",
                        column: x => x.TN_IdTipo,
                        principalTable: "THBT_CAT_TipoArea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Cargo",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_IdResidente = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TN_IdTipoCargo = table.Column<int>(type: "int", nullable: false),
                    TN_IdEstadoCargo = table.Column<int>(type: "int", nullable: false),
                    TC_Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TN_MontoBase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TN_MontoIva = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TN_MontoTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TF_FechaEmision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TF_FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TB_Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Cargo", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Cargo_THBT_A_Usuario_TC_IdResidente",
                        column: x => x.TC_IdResidente,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Cargo_THBT_CAT_EstadoCargo_TN_IdEstadoCargo",
                        column: x => x.TN_IdEstadoCargo,
                        principalTable: "THBT_CAT_EstadoCargo",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Cargo_THBT_CAT_TipoCargo_TN_IdTipoCargo",
                        column: x => x.TN_IdTipoCargo,
                        principalTable: "THBT_CAT_TipoCargo",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Acceso",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdVisitante = table.Column<int>(type: "int", nullable: false),
                    TC_IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TN_IdVivienda = table.Column<int>(type: "int", nullable: false),
                    TC_Motivo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TN_IdAutorizacion = table.Column<int>(type: "int", nullable: true),
                    TN_IdVehiculo = table.Column<int>(type: "int", nullable: true),
                    TF_FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TF_FechaSalida = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TB_Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Acceso", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Acceso_THBT_A_Autorizacion_TN_IdAutorizacion",
                        column: x => x.TN_IdAutorizacion,
                        principalTable: "THBT_A_Autorizacion",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Acceso_THBT_A_Usuario_TC_IdUsuario",
                        column: x => x.TC_IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Acceso_THBT_A_Vehiculo_TN_IdVehiculo",
                        column: x => x.TN_IdVehiculo,
                        principalTable: "THBT_A_Vehiculo",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Acceso_THBT_A_Visitante_TN_IdVisitante",
                        column: x => x.TN_IdVisitante,
                        principalTable: "THBT_A_Visitante",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Acceso_THBT_A_Vivienda_TN_IdVivienda",
                        column: x => x.TN_IdVivienda,
                        principalTable: "THBT_A_Vivienda",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_ImagenPublicacion",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdPublicacion = table.Column<int>(type: "int", nullable: false),
                    TC_Url = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_ImagenPublicacion", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_ImagenPublicacion_THBT_A_Publicacion_TN_IdPublicacion",
                        column: x => x.TN_IdPublicacion,
                        principalTable: "THBT_A_Publicacion",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_ResenaPublicacion",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdPublicacion = table.Column<int>(type: "int", nullable: false),
                    TC_IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TN_Calificacion = table.Column<int>(type: "int", nullable: false),
                    TC_Comentario = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TF_FechaResena = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_ResenaPublicacion", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_ResenaPublicacion_THBT_A_Publicacion_TN_IdPublicacion",
                        column: x => x.TN_IdPublicacion,
                        principalTable: "THBT_A_Publicacion",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_ResenaPublicacion_THBT_A_Usuario_TC_IdUsuario",
                        column: x => x.TC_IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_AreaComunFoto",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdAreaComun = table.Column<int>(type: "int", nullable: false),
                    TC_Url = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TB_EsPrincipal = table.Column<bool>(type: "bit", nullable: false),
                    TN_Orden = table.Column<int>(type: "int", nullable: false),
                    TB_Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_AreaComunFoto", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_AreaComunFoto_THBT_A_AreaComun_TN_IdAreaComun",
                        column: x => x.TN_IdAreaComun,
                        principalTable: "THBT_A_AreaComun",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_DisponibilidadArea",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdAreaComun = table.Column<int>(type: "int", nullable: false),
                    TF_Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TF_HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    TF_HoraFin = table.Column<TimeSpan>(type: "time", nullable: false),
                    TB_Estado = table.Column<bool>(type: "bit", nullable: false),
                    TN_Cantidad = table.Column<int>(type: "int", nullable: false),
                    TB_Reservado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_DisponibilidadArea", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_DisponibilidadArea_THBT_A_AreaComun_TN_IdAreaComun",
                        column: x => x.TN_IdAreaComun,
                        principalTable: "THBT_A_AreaComun",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Pago",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdCargo = table.Column<int>(type: "int", nullable: false),
                    TN_IdMetodoPago = table.Column<int>(type: "int", nullable: false),
                    TC_RutaComprobante = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TF_FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TC_MotivoRechazo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TB_Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Pago", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Pago_THBT_A_Cargo_TN_IdCargo",
                        column: x => x.TN_IdCargo,
                        principalTable: "THBT_A_Cargo",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Pago_THBT_CAT_MetodoPago_TN_IdMetodoPago",
                        column: x => x.TN_IdMetodoPago,
                        principalTable: "THBT_CAT_MetodoPago",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Recargo",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdCargo = table.Column<int>(type: "int", nullable: false),
                    TN_IdTipoRecargo = table.Column<int>(type: "int", nullable: false),
                    TN_Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TN_MontoAplicado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TF_FechaAplicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TB_Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Recargo", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Recargo_THBT_A_Cargo_TN_IdCargo",
                        column: x => x.TN_IdCargo,
                        principalTable: "THBT_A_Cargo",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Recargo_THBT_CAT_TipoRecargo_TN_IdTipoRecargo",
                        column: x => x.TN_IdTipoRecargo,
                        principalTable: "THBT_CAT_TipoRecargo",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Reserva",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TN_IdVivienda = table.Column<int>(type: "int", nullable: false),
                    TN_IdDisponibilidad = table.Column<int>(type: "int", nullable: false),
                    TN_Cantidad = table.Column<int>(type: "int", nullable: false),
                    TN_Estado = table.Column<int>(type: "int", nullable: false),
                    TF_FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TC_Motivo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TF_FechaCancelacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Reserva", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Reserva_THBT_A_DisponibilidadArea_TN_IdDisponibilidad",
                        column: x => x.TN_IdDisponibilidad,
                        principalTable: "THBT_A_DisponibilidadArea",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_THBT_A_Reserva_THBT_A_Usuario_TC_IdUsuario",
                        column: x => x.TC_IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_THBT_A_Reserva_THBT_A_Vivienda_TN_IdVivienda",
                        column: x => x.TN_IdVivienda,
                        principalTable: "THBT_A_Vivienda",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Incidencia",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TN_IdVivienda = table.Column<int>(type: "int", nullable: true),
                    TN_IdAreaComun = table.Column<int>(type: "int", nullable: true),
                    TN_Tipo = table.Column<int>(type: "int", nullable: false),
                    TN_Estado = table.Column<int>(type: "int", nullable: false),
                    TN_Responsabilidad = table.Column<int>(type: "int", nullable: false),
                    TN_Prioridad = table.Column<int>(type: "int", nullable: true),
                    TC_Titulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TC_Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TF_FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TC_ImagenUrl = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    MantenimientoTN_Id = table.Column<int>(type: "int", nullable: true),
                    TF_FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Incidencia", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Incidencia_THBT_A_AreaComun_TN_IdAreaComun",
                        column: x => x.TN_IdAreaComun,
                        principalTable: "THBT_A_AreaComun",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Incidencia_THBT_A_Usuario_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_THBT_A_Incidencia_THBT_A_Usuario_TC_IdUsuario",
                        column: x => x.TC_IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Incidencia_THBT_A_Vivienda_TN_IdVivienda",
                        column: x => x.TN_IdVivienda,
                        principalTable: "THBT_A_Vivienda",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Mantenimiento",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdIncidencia = table.Column<int>(type: "int", nullable: false),
                    TN_IdTipo = table.Column<int>(type: "int", nullable: false),
                    TN_IdAreaComun = table.Column<int>(type: "int", nullable: true),
                    TC_IdPersonalAsignado = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TC_Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TF_FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TF_FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TF_FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TN_Estado = table.Column<int>(type: "int", nullable: false),
                    TC_Observaciones = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Mantenimiento", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Mantenimiento_THBT_A_AreaComun_TN_IdAreaComun",
                        column: x => x.TN_IdAreaComun,
                        principalTable: "THBT_A_AreaComun",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Mantenimiento_THBT_A_Incidencia_TN_IdIncidencia",
                        column: x => x.TN_IdIncidencia,
                        principalTable: "THBT_A_Incidencia",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Mantenimiento_THBT_A_Usuario_TC_IdPersonalAsignado",
                        column: x => x.TC_IdPersonalAsignado,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Mantenimiento_THBT_CAT_TipoMantenimiento_TN_IdTipo",
                        column: x => x.TN_IdTipo,
                        principalTable: "THBT_CAT_TipoMantenimiento",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "THBT_CAT_EstadoCargo",
                columns: new[] { "TN_Id", "TC_Nombre" },
                values: new object[,]
                {
                    { 1, "Pendiente" },
                    { 2, "En revisión" },
                    { 3, "Pagado" }
                });

            migrationBuilder.InsertData(
                table: "THBT_CAT_MetodoPago",
                columns: new[] { "TN_Id", "TC_Nombre" },
                values: new object[,]
                {
                    { 1, "Efectivo" },
                    { 2, "Tarjeta" },
                    { 3, "SINPE" }
                });

            migrationBuilder.InsertData(
                table: "THBT_CAT_TipoRecargo",
                columns: new[] { "TN_Id", "TC_Nombre" },
                values: new object[,]
                {
                    { 1, "Fijo" },
                    { 2, "Porcentaje" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Acceso_TC_IdUsuario",
                table: "THBT_A_Acceso",
                column: "TC_IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Acceso_TF_FechaIngreso",
                table: "THBT_A_Acceso",
                column: "TF_FechaIngreso");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Acceso_TF_FechaSalida",
                table: "THBT_A_Acceso",
                column: "TF_FechaSalida");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Acceso_TN_IdAutorizacion",
                table: "THBT_A_Acceso",
                column: "TN_IdAutorizacion",
                unique: true,
                filter: "[TN_IdAutorizacion] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Acceso_TN_IdVehiculo",
                table: "THBT_A_Acceso",
                column: "TN_IdVehiculo");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Acceso_TN_IdVisitante",
                table: "THBT_A_Acceso",
                column: "TN_IdVisitante");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Acceso_TN_IdVivienda",
                table: "THBT_A_Acceso",
                column: "TN_IdVivienda");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_AreaComun_TN_IdTipo",
                table: "THBT_A_AreaComun",
                column: "TN_IdTipo");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_AreaComunFoto_TN_IdAreaComun",
                table: "THBT_A_AreaComunFoto",
                column: "TN_IdAreaComun");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Autorizacion_TC_Codigo",
                table: "THBT_A_Autorizacion",
                column: "TC_Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Autorizacion_TC_IdUsuario",
                table: "THBT_A_Autorizacion",
                column: "TC_IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Autorizacion_TF_FechaVencimiento",
                table: "THBT_A_Autorizacion",
                column: "TF_FechaVencimiento");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Autorizacion_TN_Estado",
                table: "THBT_A_Autorizacion",
                column: "TN_Estado");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Autorizacion_TN_IdVisitante",
                table: "THBT_A_Autorizacion",
                column: "TN_IdVisitante");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Autorizacion_TN_IdVivienda",
                table: "THBT_A_Autorizacion",
                column: "TN_IdVivienda");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Cargo_TC_IdResidente",
                table: "THBT_A_Cargo",
                column: "TC_IdResidente");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Cargo_TF_FechaVencimiento",
                table: "THBT_A_Cargo",
                column: "TF_FechaVencimiento");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Cargo_TN_IdEstadoCargo",
                table: "THBT_A_Cargo",
                column: "TN_IdEstadoCargo");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Cargo_TN_IdTipoCargo",
                table: "THBT_A_Cargo",
                column: "TN_IdTipoCargo");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_DisponibilidadArea_TN_IdAreaComun",
                table: "THBT_A_DisponibilidadArea",
                column: "TN_IdAreaComun");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_ImagenPublicacion_TN_IdPublicacion",
                table: "THBT_A_ImagenPublicacion",
                column: "TN_IdPublicacion");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_ApplicationUserId",
                table: "THBT_A_Incidencia",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_MantenimientoTN_Id",
                table: "THBT_A_Incidencia",
                column: "MantenimientoTN_Id");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_TC_IdUsuario",
                table: "THBT_A_Incidencia",
                column: "TC_IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_TF_FechaRegistro",
                table: "THBT_A_Incidencia",
                column: "TF_FechaRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_TN_Estado",
                table: "THBT_A_Incidencia",
                column: "TN_Estado");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_TN_IdAreaComun",
                table: "THBT_A_Incidencia",
                column: "TN_IdAreaComun");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_TN_IdVivienda",
                table: "THBT_A_Incidencia",
                column: "TN_IdVivienda");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_TN_Responsabilidad",
                table: "THBT_A_Incidencia",
                column: "TN_Responsabilidad");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Mantenimiento_TC_IdPersonalAsignado",
                table: "THBT_A_Mantenimiento",
                column: "TC_IdPersonalAsignado");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Mantenimiento_TN_Estado",
                table: "THBT_A_Mantenimiento",
                column: "TN_Estado");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Mantenimiento_TN_IdAreaComun",
                table: "THBT_A_Mantenimiento",
                column: "TN_IdAreaComun");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Mantenimiento_TN_IdIncidencia",
                table: "THBT_A_Mantenimiento",
                column: "TN_IdIncidencia",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Mantenimiento_TN_IdTipo",
                table: "THBT_A_Mantenimiento",
                column: "TN_IdTipo");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Pago_TN_IdCargo",
                table: "THBT_A_Pago",
                column: "TN_IdCargo");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Pago_TN_IdMetodoPago",
                table: "THBT_A_Pago",
                column: "TN_IdMetodoPago");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Publicacion_TC_IdUsuario",
                table: "THBT_A_Publicacion",
                column: "TC_IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Publicacion_TN_IdCategoria",
                table: "THBT_A_Publicacion",
                column: "TN_IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Recargo_TN_IdCargo",
                table: "THBT_A_Recargo",
                column: "TN_IdCargo");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Recargo_TN_IdTipoRecargo",
                table: "THBT_A_Recargo",
                column: "TN_IdTipoRecargo");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_ResenaPublicacion_TC_IdUsuario",
                table: "THBT_A_ResenaPublicacion",
                column: "TC_IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_ResenaPublicacion_TN_IdPublicacion",
                table: "THBT_A_ResenaPublicacion",
                column: "TN_IdPublicacion");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Reserva_TC_IdUsuario",
                table: "THBT_A_Reserva",
                column: "TC_IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Reserva_TN_IdDisponibilidad",
                table: "THBT_A_Reserva",
                column: "TN_IdDisponibilidad");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Reserva_TN_IdVivienda",
                table: "THBT_A_Reserva",
                column: "TN_IdVivienda");

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
                name: "IX_THBT_A_Vehiculo_TN_IdVisitante",
                table: "THBT_A_Vehiculo",
                column: "TN_IdVisitante");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Vivienda_TC_Numero",
                table: "THBT_A_Vivienda",
                column: "TC_Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_ViviendaUsuario_TC_IdUsuario",
                table: "THBT_A_ViviendaUsuario",
                column: "TC_IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_ViviendaUsuario_TN_IdVivienda",
                table: "THBT_A_ViviendaUsuario",
                column: "TN_IdVivienda");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_CAT_TipoCargo_TC_Nombre",
                table: "THBT_CAT_TipoCargo",
                column: "TC_Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_THBT_CAT_TipoMantenimiento_TC_Nombre",
                table: "THBT_CAT_TipoMantenimiento",
                column: "TC_Nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_THBT_A_Incidencia_THBT_A_Mantenimiento_MantenimientoTN_Id",
                table: "THBT_A_Incidencia",
                column: "MantenimientoTN_Id",
                principalTable: "THBT_A_Mantenimiento",
                principalColumn: "TN_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_THBT_A_Incidencia_THBT_A_Usuario_ApplicationUserId",
                table: "THBT_A_Incidencia");

            migrationBuilder.DropForeignKey(
                name: "FK_THBT_A_Incidencia_THBT_A_Usuario_TC_IdUsuario",
                table: "THBT_A_Incidencia");

            migrationBuilder.DropForeignKey(
                name: "FK_THBT_A_Mantenimiento_THBT_A_Usuario_TC_IdPersonalAsignado",
                table: "THBT_A_Mantenimiento");

            migrationBuilder.DropForeignKey(
                name: "FK_THBT_A_Incidencia_THBT_A_Vivienda_TN_IdVivienda",
                table: "THBT_A_Incidencia");

            migrationBuilder.DropForeignKey(
                name: "FK_THBT_A_AreaComun_THBT_CAT_TipoArea_TN_IdTipo",
                table: "THBT_A_AreaComun");

            migrationBuilder.DropForeignKey(
                name: "FK_THBT_A_Incidencia_THBT_A_AreaComun_TN_IdAreaComun",
                table: "THBT_A_Incidencia");

            migrationBuilder.DropForeignKey(
                name: "FK_THBT_A_Mantenimiento_THBT_A_AreaComun_TN_IdAreaComun",
                table: "THBT_A_Mantenimiento");

            migrationBuilder.DropForeignKey(
                name: "FK_THBT_A_Incidencia_THBT_A_Mantenimiento_MantenimientoTN_Id",
                table: "THBT_A_Incidencia");

            migrationBuilder.DropTable(
                name: "THBT_A_Acceso");

            migrationBuilder.DropTable(
                name: "THBT_A_AreaComunFoto");

            migrationBuilder.DropTable(
                name: "THBT_A_ConfiguracionPago");

            migrationBuilder.DropTable(
                name: "THBT_A_ExpiracionReserva");

            migrationBuilder.DropTable(
                name: "THBT_A_ImagenPublicacion");

            migrationBuilder.DropTable(
                name: "THBT_A_Pago");

            migrationBuilder.DropTable(
                name: "THBT_A_Recargo");

            migrationBuilder.DropTable(
                name: "THBT_A_ResenaPublicacion");

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
                name: "THBT_A_ViviendaUsuario");

            migrationBuilder.DropTable(
                name: "THBT_CAT_TipoTarjeta");

            migrationBuilder.DropTable(
                name: "THBT_H_Cargo");

            migrationBuilder.DropTable(
                name: "THBT_H_ConfiguracionPago");

            migrationBuilder.DropTable(
                name: "THBT_H_Pago");

            migrationBuilder.DropTable(
                name: "THBT_A_Autorizacion");

            migrationBuilder.DropTable(
                name: "THBT_A_Vehiculo");

            migrationBuilder.DropTable(
                name: "THBT_CAT_MetodoPago");

            migrationBuilder.DropTable(
                name: "THBT_A_Cargo");

            migrationBuilder.DropTable(
                name: "THBT_CAT_TipoRecargo");

            migrationBuilder.DropTable(
                name: "THBT_A_Publicacion");

            migrationBuilder.DropTable(
                name: "THBT_A_DisponibilidadArea");

            migrationBuilder.DropTable(
                name: "THBT_A_Rol");

            migrationBuilder.DropTable(
                name: "THBT_A_Visitante");

            migrationBuilder.DropTable(
                name: "THBT_CAT_EstadoCargo");

            migrationBuilder.DropTable(
                name: "THBT_CAT_TipoCargo");

            migrationBuilder.DropTable(
                name: "THBT_CAT_CategoriaPublicacion");

            migrationBuilder.DropTable(
                name: "THBT_A_Usuario");

            migrationBuilder.DropTable(
                name: "THBT_A_Vivienda");

            migrationBuilder.DropTable(
                name: "THBT_CAT_TipoArea");

            migrationBuilder.DropTable(
                name: "THBT_A_AreaComun");

            migrationBuilder.DropTable(
                name: "THBT_A_Mantenimiento");

            migrationBuilder.DropTable(
                name: "THBT_A_Incidencia");

            migrationBuilder.DropTable(
                name: "THBT_CAT_TipoMantenimiento");
        }
    }
}
