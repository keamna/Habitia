using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class _02Reservas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TC_Motivo",
                table: "THBT_A_Reserva",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TF_FechaCancelacion",
                table: "THBT_A_Reserva",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TB_Reservado",
                table: "THBT_A_DisponibilidadArea",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TN_Cantidad",
                table: "THBT_A_DisponibilidadArea",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TN_AnticipacionMinima",
                table: "THBT_A_AreaComun",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TC_Motivo",
                table: "THBT_A_Reserva");

            migrationBuilder.DropColumn(
                name: "TF_FechaCancelacion",
                table: "THBT_A_Reserva");

            migrationBuilder.DropColumn(
                name: "TB_Reservado",
                table: "THBT_A_DisponibilidadArea");

            migrationBuilder.DropColumn(
                name: "TN_Cantidad",
                table: "THBT_A_DisponibilidadArea");

            migrationBuilder.DropColumn(
                name: "TN_AnticipacionMinima",
                table: "THBT_A_AreaComun");
        }
    }
}