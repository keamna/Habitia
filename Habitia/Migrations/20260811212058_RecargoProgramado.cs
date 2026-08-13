using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class RecargoProgramado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TC_Descripcion",
                table: "THBT_A_Cargo",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<bool>(
                name: "TB_RecargoAplicado",
                table: "THBT_A_Cargo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TB_RecargoProgramado",
                table: "THBT_A_Cargo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TN_IdTipoRecargo",
                table: "THBT_A_Cargo",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TN_ValorRecargo",
                table: "THBT_A_Cargo",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Cargo_TN_IdTipoRecargo",
                table: "THBT_A_Cargo",
                column: "TN_IdTipoRecargo");

            migrationBuilder.AddForeignKey(
                name: "FK_THBT_A_Cargo_THBT_CAT_TipoRecargo_TN_IdTipoRecargo",
                table: "THBT_A_Cargo",
                column: "TN_IdTipoRecargo",
                principalTable: "THBT_CAT_TipoRecargo",
                principalColumn: "TN_Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_THBT_A_Cargo_THBT_CAT_TipoRecargo_TN_IdTipoRecargo",
                table: "THBT_A_Cargo");

            migrationBuilder.DropIndex(
                name: "IX_THBT_A_Cargo_TN_IdTipoRecargo",
                table: "THBT_A_Cargo");

            migrationBuilder.DropColumn(
                name: "TB_RecargoAplicado",
                table: "THBT_A_Cargo");

            migrationBuilder.DropColumn(
                name: "TB_RecargoProgramado",
                table: "THBT_A_Cargo");

            migrationBuilder.DropColumn(
                name: "TN_IdTipoRecargo",
                table: "THBT_A_Cargo");

            migrationBuilder.DropColumn(
                name: "TN_ValorRecargo",
                table: "THBT_A_Cargo");

            migrationBuilder.AlterColumn<string>(
                name: "TC_Descripcion",
                table: "THBT_A_Cargo",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
