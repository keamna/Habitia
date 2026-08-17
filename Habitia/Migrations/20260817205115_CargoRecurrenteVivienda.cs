using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class CargoRecurrenteVivienda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TN_IdVivienda",
                table: "THBT_A_CargoRecurrenteResidente",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_CargoRecurrenteResidente_TN_IdVivienda",
                table: "THBT_A_CargoRecurrenteResidente",
                column: "TN_IdVivienda");

            migrationBuilder.AddForeignKey(
                name: "FK_THBT_A_CargoRecurrenteResidente_THBT_A_Vivienda_TN_IdVivienda",
                table: "THBT_A_CargoRecurrenteResidente",
                column: "TN_IdVivienda",
                principalTable: "THBT_A_Vivienda",
                principalColumn: "TN_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_THBT_A_CargoRecurrenteResidente_THBT_A_Vivienda_TN_IdVivienda",
                table: "THBT_A_CargoRecurrenteResidente");

            migrationBuilder.DropIndex(
                name: "IX_THBT_A_CargoRecurrenteResidente_TN_IdVivienda",
                table: "THBT_A_CargoRecurrenteResidente");

            migrationBuilder.DropColumn(
                name: "TN_IdVivienda",
                table: "THBT_A_CargoRecurrenteResidente");
        }
    }
}
