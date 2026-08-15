using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class RecargoProgramado2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "TC_FrecuenciaRecargo",
                table: "THBT_A_CargoRecurrente",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TC_FrecuenciaRecargo",
                table: "THBT_A_Cargo",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TF_UltimaAplicacionRecargo",
                table: "THBT_A_Cargo",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TC_FrecuenciaRecargo",
                table: "THBT_A_CargoRecurrente");

            migrationBuilder.DropColumn(
                name: "TC_FrecuenciaRecargo",
                table: "THBT_A_Cargo");

            migrationBuilder.DropColumn(
                name: "TF_UltimaAplicacionRecargo",
                table: "THBT_A_Cargo");

            migrationBuilder.AddColumn<int>(
                name: "TN_DiasPlazoPago",
                table: "THBT_A_CargoRecurrente",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
