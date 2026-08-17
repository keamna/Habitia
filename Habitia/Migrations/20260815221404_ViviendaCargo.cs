using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class ViviendaCargo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TN_IdVivienda",
                table: "THBT_A_Cargo",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Cargo_TN_IdVivienda",
                table: "THBT_A_Cargo",
                column: "TN_IdVivienda");

            migrationBuilder.AddForeignKey(
                name: "FK_THBT_A_Cargo_THBT_A_Vivienda_TN_IdVivienda",
                table: "THBT_A_Cargo",
                column: "TN_IdVivienda",
                principalTable: "THBT_A_Vivienda",
                principalColumn: "TN_Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_THBT_A_Cargo_THBT_A_Vivienda_TN_IdVivienda",
                table: "THBT_A_Cargo");

            migrationBuilder.DropIndex(
                name: "IX_THBT_A_Cargo_TN_IdVivienda",
                table: "THBT_A_Cargo");

            migrationBuilder.DropColumn(
                name: "TN_IdVivienda",
                table: "THBT_A_Cargo");
        }
    }
}
