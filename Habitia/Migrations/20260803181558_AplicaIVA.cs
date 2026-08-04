using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class AplicaIVA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TB_AplicaIva",
                table: "THBT_A_Cargo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "THBT_CAT_EstadoCargo",
                columns: new[] { "TN_Id", "TC_Nombre" },
                values: new object[] { 4, "Vencido" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "THBT_CAT_EstadoCargo",
                keyColumn: "TN_Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "TB_AplicaIva",
                table: "THBT_A_Cargo");
        }
    }
}
