using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class CargoRecurrente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_THBT_A_CargoRecurrenteResidente_THBT_A_Vivienda_TN_IdVivienda",
                table: "THBT_A_CargoRecurrenteResidente");

            migrationBuilder.DropIndex(
                name: "IX_THBT_A_CargoRecurrenteResidente_TN_IdCargoRecurrente_TC_IdResidente",
                table: "THBT_A_CargoRecurrenteResidente");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_CargoRecurrenteResidente_TN_IdCargoRecurrente_TC_IdResidente_TN_IdVivienda",
                table: "THBT_A_CargoRecurrenteResidente",
                columns: new[] { "TN_IdCargoRecurrente", "TC_IdResidente", "TN_IdVivienda" },
                unique: true,
                filter: "[TN_IdVivienda] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_THBT_A_CargoRecurrenteResidente_THBT_A_Vivienda_TN_IdVivienda",
                table: "THBT_A_CargoRecurrenteResidente",
                column: "TN_IdVivienda",
                principalTable: "THBT_A_Vivienda",
                principalColumn: "TN_Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_THBT_A_CargoRecurrenteResidente_THBT_A_Vivienda_TN_IdVivienda",
                table: "THBT_A_CargoRecurrenteResidente");

            migrationBuilder.DropIndex(
                name: "IX_THBT_A_CargoRecurrenteResidente_TN_IdCargoRecurrente_TC_IdResidente_TN_IdVivienda",
                table: "THBT_A_CargoRecurrenteResidente");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_CargoRecurrenteResidente_TN_IdCargoRecurrente_TC_IdResidente",
                table: "THBT_A_CargoRecurrenteResidente",
                columns: new[] { "TN_IdCargoRecurrente", "TC_IdResidente" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_THBT_A_CargoRecurrenteResidente_THBT_A_Vivienda_TN_IdVivienda",
                table: "THBT_A_CargoRecurrenteResidente",
                column: "TN_IdVivienda",
                principalTable: "THBT_A_Vivienda",
                principalColumn: "TN_Id");
        }
    }
}
