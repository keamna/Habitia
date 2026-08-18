using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class CodigoVerificacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "THBT_A_CodigoVerificacion",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TC_Codigo = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    TF_FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TF_FechaExpiracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TN_Intentos = table.Column<int>(type: "int", nullable: false),
                    TB_Usado = table.Column<bool>(type: "bit", nullable: false),
                    TB_Invalidado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_CodigoVerificacion", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_CodigoVerificacion_THBT_A_Usuario_TC_IdUsuario",
                        column: x => x.TC_IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_CodigoVerificacion_TC_IdUsuario",
                table: "THBT_A_CodigoVerificacion",
                column: "TC_IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "THBT_A_CodigoVerificacion");
        }
    }
}
