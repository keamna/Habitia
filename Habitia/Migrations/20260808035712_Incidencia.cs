using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class Incidencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_THBT_A_Incidencia_THBT_A_Mantenimiento_MantenimientoTN_Id",
                table: "THBT_A_Incidencia");

            migrationBuilder.DropIndex(
                name: "IX_THBT_A_Incidencia_MantenimientoTN_Id",
                table: "THBT_A_Incidencia");

            migrationBuilder.DropColumn(
                name: "MantenimientoTN_Id",
                table: "THBT_A_Incidencia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MantenimientoTN_Id",
                table: "THBT_A_Incidencia",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_MantenimientoTN_Id",
                table: "THBT_A_Incidencia",
                column: "MantenimientoTN_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_THBT_A_Incidencia_THBT_A_Mantenimiento_MantenimientoTN_Id",
                table: "THBT_A_Incidencia",
                column: "MantenimientoTN_Id",
                principalTable: "THBT_A_Mantenimiento",
                principalColumn: "TN_Id");
        }
    }
}
