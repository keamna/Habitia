using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class AnticipacionPorHorario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TN_AnticipacionMinima",
                table: "THBT_A_DisponibilidadArea",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Los horarios que ya existían heredan el valor que tenía su área común,
            // para que las reservas vigentes no cambien de regla de un día para otro.
            migrationBuilder.Sql(@"
                UPDATE d
                SET d.TN_AnticipacionMinima = a.TN_AnticipacionMinima
                FROM THBT_A_DisponibilidadArea d
                INNER JOIN THBT_A_AreaComun a ON a.TN_Id = d.TN_IdAreaComun;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TN_AnticipacionMinima",
                table: "THBT_A_DisponibilidadArea");
        }
    }
}