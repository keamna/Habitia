using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class CoolDownCodigoVerificacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TF_UltimoReenvioUtc",
                table: "THBT_A_CodigoVerificacion",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TN_CiclosFallidos",
                table: "THBT_A_CodigoVerificacion",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TF_UltimoReenvioUtc",
                table: "THBT_A_CodigoVerificacion");

            migrationBuilder.DropColumn(
                name: "TN_CiclosFallidos",
                table: "THBT_A_CodigoVerificacion");
        }
    }
}
