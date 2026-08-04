using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategoriasPublicacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "THBT_CAT_CategoriaPublicacion",
                columns: new[] { "TN_Id", "TB_Estado", "TC_Nombre" },
                values: new object[,]
                {
                    { 1, true, "Muebles y hogar" },
                    { 2, true, "Electrodomésticos" },
                    { 3, true, "Ropa y accesorios" },
                    { 4, true, "Alimentos y bebidas" },
                    { 5, true, "Servicios del hogar" },
                    { 6, true, "Cuidado personal" },
                    { 7, true, "Clases y tutorías" },
                    { 8, true, "Mascotas" },
                    { 9, true, "Tecnología" },
                    { 10, true, "Otros" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "THBT_CAT_CategoriaPublicacion",
                keyColumn: "TN_Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "THBT_CAT_CategoriaPublicacion",
                keyColumn: "TN_Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "THBT_CAT_CategoriaPublicacion",
                keyColumn: "TN_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "THBT_CAT_CategoriaPublicacion",
                keyColumn: "TN_Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "THBT_CAT_CategoriaPublicacion",
                keyColumn: "TN_Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "THBT_CAT_CategoriaPublicacion",
                keyColumn: "TN_Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "THBT_CAT_CategoriaPublicacion",
                keyColumn: "TN_Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "THBT_CAT_CategoriaPublicacion",
                keyColumn: "TN_Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "THBT_CAT_CategoriaPublicacion",
                keyColumn: "TN_Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "THBT_CAT_CategoriaPublicacion",
                keyColumn: "TN_Id",
                keyValue: 10);
        }
    }
}
