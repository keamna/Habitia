using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class Mantenimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "THBT_A_PersonalTipoMantenimiento",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_IdPersonal = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TN_IdTipo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_PersonalTipoMantenimiento", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_PersonalTipoMantenimiento_THBT_A_Usuario_TC_IdPersonal",
                        column: x => x.TC_IdPersonal,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_THBT_A_PersonalTipoMantenimiento_THBT_CAT_TipoMantenimiento_TN_IdTipo",
                        column: x => x.TN_IdTipo,
                        principalTable: "THBT_CAT_TipoMantenimiento",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_PersonalTipoMantenimiento_TC_IdPersonal_TN_IdTipo",
                table: "THBT_A_PersonalTipoMantenimiento",
                columns: new[] { "TC_IdPersonal", "TN_IdTipo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_PersonalTipoMantenimiento_TN_IdTipo",
                table: "THBT_A_PersonalTipoMantenimiento",
                column: "TN_IdTipo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "THBT_A_PersonalTipoMantenimiento");
        }
    }
}
