using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class CargosProgramados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "THBT_A_CargoRecurrente",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdTipoCargo = table.Column<int>(type: "int", nullable: false),
                    TN_MontoBase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TB_AplicaIva = table.Column<bool>(type: "bit", nullable: false),
                    TC_Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TN_DiasPlazoPago = table.Column<int>(type: "int", nullable: false),
                    TC_Frecuencia = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TB_AplicarATodos = table.Column<bool>(type: "bit", nullable: false),
                    TB_RecargoProgramado = table.Column<bool>(type: "bit", nullable: false),
                    TN_IdTipoRecargo = table.Column<int>(type: "int", nullable: true),
                    TN_ValorRecargo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TF_FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TF_UltimaGeneracion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TB_Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_CargoRecurrente", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_CargoRecurrente_THBT_CAT_TipoCargo_TN_IdTipoCargo",
                        column: x => x.TN_IdTipoCargo,
                        principalTable: "THBT_CAT_TipoCargo",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_CargoRecurrente_THBT_CAT_TipoRecargo_TN_IdTipoRecargo",
                        column: x => x.TN_IdTipoRecargo,
                        principalTable: "THBT_CAT_TipoRecargo",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_CargoRecurrenteResidente",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdCargoRecurrente = table.Column<int>(type: "int", nullable: false),
                    TC_IdResidente = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_CargoRecurrenteResidente", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_CargoRecurrenteResidente_THBT_A_CargoRecurrente_TN_IdCargoRecurrente",
                        column: x => x.TN_IdCargoRecurrente,
                        principalTable: "THBT_A_CargoRecurrente",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_THBT_A_CargoRecurrenteResidente_THBT_A_Usuario_TC_IdResidente",
                        column: x => x.TC_IdResidente,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_CargoRecurrente_TB_Estado",
                table: "THBT_A_CargoRecurrente",
                column: "TB_Estado");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_CargoRecurrente_TN_IdTipoCargo",
                table: "THBT_A_CargoRecurrente",
                column: "TN_IdTipoCargo");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_CargoRecurrente_TN_IdTipoRecargo",
                table: "THBT_A_CargoRecurrente",
                column: "TN_IdTipoRecargo");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_CargoRecurrenteResidente_TC_IdResidente",
                table: "THBT_A_CargoRecurrenteResidente",
                column: "TC_IdResidente");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_CargoRecurrenteResidente_TN_IdCargoRecurrente_TC_IdResidente",
                table: "THBT_A_CargoRecurrenteResidente",
                columns: new[] { "TN_IdCargoRecurrente", "TC_IdResidente" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "THBT_A_CargoRecurrenteResidente");

            migrationBuilder.DropTable(
                name: "THBT_A_CargoRecurrente");
        }
    }
}
