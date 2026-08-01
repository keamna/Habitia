using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class _05Accesos2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =========================
            // THBT_A_Visitante
            // =========================
            migrationBuilder.CreateTable(
                name: "THBT_A_Visitante",
                columns: table => new
                {
                    TN_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_TipoIdentificacion = table.Column<int>(nullable: false),
                    TC_Identificacion = table.Column<string>(maxLength: 20, nullable: false),
                    TC_Nombre = table.Column<string>(maxLength: 150, nullable: false),
                    TC_Telefono = table.Column<string>(maxLength: 20, nullable: true),
                    TB_Estado = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Visitante", x => x.TN_Id);
                });

            // =========================
            // THBT_A_Vehiculo
            // =========================
            migrationBuilder.CreateTable(
                name: "THBT_A_Vehiculo",
                columns: table => new
                {
                    TN_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdVisitante = table.Column<int>(nullable: false),
                    TC_Placa = table.Column<string>(maxLength: 10, nullable: false),
                    TC_Tipo = table.Column<string>(maxLength: 50, nullable: true),
                    TC_Observaciones = table.Column<string>(maxLength: 300, nullable: true),
                    TB_Estado = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Vehiculo", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_Vehiculo_Visitante",
                        column: x => x.TN_IdVisitante,
                        principalTable: "THBT_A_Visitante",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // =========================
            // THBT_A_Autorizacion
            // =========================
            migrationBuilder.CreateTable(
                name: "THBT_A_Autorizacion",
                columns: table => new
                {
                    TN_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdVisitante = table.Column<int>(nullable: false),
                    TC_IdUsuario = table.Column<string>(nullable: false),
                    TN_IdVivienda = table.Column<int>(nullable: false),
                    TC_Codigo = table.Column<string>(maxLength: 100, nullable: false),
                    TF_FechaVisita = table.Column<DateTime>(nullable: false),
                    TF_FechaVencimiento = table.Column<DateTime>(nullable: false),
                    TC_Motivo = table.Column<string>(maxLength: 200, nullable: false),
                    TF_FechaRegistro = table.Column<DateTime>(nullable: false),
                    TN_Estado = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Autorizacion", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_Autorizacion_Visitante",
                        column: x => x.TN_IdVisitante,
                        principalTable: "THBT_A_Visitante",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Autorizacion_Usuario",
                        column: x => x.TC_IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Autorizacion_Vivienda",
                        column: x => x.TN_IdVivienda,
                        principalTable: "THBT_A_Vivienda",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // =========================
            // THBT_A_Acceso
            // =========================
            migrationBuilder.CreateTable(
                name: "THBT_A_Acceso",
                columns: table => new
                {
                    TN_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdVisitante = table.Column<int>(nullable: false),
                    TC_IdUsuario = table.Column<string>(nullable: false),
                    TN_IdVivienda = table.Column<int>(nullable: false),
                    TC_Motivo = table.Column<string>(maxLength: 200, nullable: false),
                    TN_IdAutorizacion = table.Column<int>(nullable: true),
                    TN_IdVehiculo = table.Column<int>(nullable: true),
                    TF_FechaIngreso = table.Column<DateTime>(nullable: false),
                    TF_FechaSalida = table.Column<DateTime>(nullable: true),
                    TB_Estado = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Acceso", x => x.TN_Id);

                    table.ForeignKey(
                        name: "FK_Acceso_Visitante",
                        column: x => x.TN_IdVisitante,
                        principalTable: "THBT_A_Visitante",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_Acceso_Usuario",
                        column: x => x.TC_IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_Acceso_Vivienda",
                        column: x => x.TN_IdVivienda,
                        principalTable: "THBT_A_Vivienda",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_Acceso_Autorizacion",
                        column: x => x.TN_IdAutorizacion,
                        principalTable: "THBT_A_Autorizacion",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_Acceso_Vehiculo",
                        column: x => x.TN_IdVehiculo,
                        principalTable: "THBT_A_Vehiculo",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "THBT_A_Acceso");
            migrationBuilder.DropTable(name: "THBT_A_Autorizacion");
            migrationBuilder.DropTable(name: "THBT_A_Vehiculo");
            migrationBuilder.DropTable(name: "THBT_A_Visitante");
        }
    }
}
