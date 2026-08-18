using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class CambiosContrasena : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "THBT_A_CambioContrasena",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TC_ContrasenaTemporal = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    TF_FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TF_FechaExpiracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TB_Utilizado = table.Column<bool>(type: "bit", nullable: false),
                    TF_FechaUtilizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_CambioContrasena", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_CambioContrasena_THBT_A_Usuario_TC_IdUsuario",
                        column: x => x.TC_IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_CambioEmail",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TC_EmailNuevo = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TC_Codigo = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    TF_FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TF_FechaExpiracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TN_Intentos = table.Column<int>(type: "int", nullable: false),
                    TB_Confirmado = table.Column<bool>(type: "bit", nullable: false),
                    TB_Invalidado = table.Column<bool>(type: "bit", nullable: false),
                    TF_UltimoReenvioUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_CambioEmail", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_CambioEmail_THBT_A_Usuario_TC_IdUsuario",
                        column: x => x.TC_IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_CambioContrasena_TC_IdUsuario",
                table: "THBT_A_CambioContrasena",
                column: "TC_IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_CambioEmail_TC_IdUsuario",
                table: "THBT_A_CambioEmail",
                column: "TC_IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "THBT_A_CambioContrasena");

            migrationBuilder.DropTable(
                name: "THBT_A_CambioEmail");
        }
    }
}
