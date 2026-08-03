using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class _02DocumentosAsambleas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "THBT_A_Asamblea",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TF_FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TN_Modalidad = table.Column<int>(type: "int", nullable: false),
                    TC_Lugar = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TC_Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TN_Estado = table.Column<int>(type: "int", nullable: false),
                    TC_IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TF_FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Asamblea", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Asamblea_THBT_A_Usuario_TC_IdUsuario",
                        column: x => x.TC_IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "THBT_CAT_CategoriaDocumento",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TB_Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_CAT_CategoriaDocumento", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_ParticipanteAsamblea",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdAsamblea = table.Column<int>(type: "int", nullable: false),
                    TC_IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TB_Confirmado = table.Column<bool>(type: "bit", nullable: false),
                    TB_Asistio = table.Column<bool>(type: "bit", nullable: false),
                    TF_FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_ParticipanteAsamblea", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_ParticipanteAsamblea_THBT_A_Asamblea_TN_IdAsamblea",
                        column: x => x.TN_IdAsamblea,
                        principalTable: "THBT_A_Asamblea",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_THBT_A_ParticipanteAsamblea_THBT_A_Usuario_TC_IdUsuario",
                        column: x => x.TC_IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Documento",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TC_Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TN_IdCategoria = table.Column<int>(type: "int", nullable: false),
                    TC_Archivo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TC_NombreOriginal = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TN_Tamano = table.Column<long>(type: "bigint", nullable: false),
                    TC_IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TB_Estado = table.Column<bool>(type: "bit", nullable: false),
                    TF_FechaCarga = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Documento", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Documento_THBT_A_Usuario_TC_IdUsuario",
                        column: x => x.TC_IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Documento_THBT_CAT_CategoriaDocumento_TN_IdCategoria",
                        column: x => x.TN_IdCategoria,
                        principalTable: "THBT_CAT_CategoriaDocumento",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_DocumentoAsamblea",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdAsamblea = table.Column<int>(type: "int", nullable: false),
                    TN_IdDocumento = table.Column<int>(type: "int", nullable: false),
                    TF_FechaAsociacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_DocumentoAsamblea", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_DocumentoAsamblea_THBT_A_Asamblea_TN_IdAsamblea",
                        column: x => x.TN_IdAsamblea,
                        principalTable: "THBT_A_Asamblea",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_THBT_A_DocumentoAsamblea_THBT_A_Documento_TN_IdDocumento",
                        column: x => x.TN_IdDocumento,
                        principalTable: "THBT_A_Documento",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "THBT_CAT_CategoriaDocumento",
                columns: new[] { "TN_Id", "TB_Estado", "TC_Nombre" },
                values: new object[,]
                {
                    { 1, true, "Reglamentos" },
                    { 2, true, "Actas" },
                    { 3, true, "Comunicados" },
                    { 4, true, "Otros" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Asamblea_TC_IdUsuario",
                table: "THBT_A_Asamblea",
                column: "TC_IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Asamblea_TF_FechaHora",
                table: "THBT_A_Asamblea",
                column: "TF_FechaHora");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Asamblea_TN_Estado",
                table: "THBT_A_Asamblea",
                column: "TN_Estado");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Documento_TB_Estado",
                table: "THBT_A_Documento",
                column: "TB_Estado");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Documento_TC_IdUsuario",
                table: "THBT_A_Documento",
                column: "TC_IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Documento_TN_IdCategoria",
                table: "THBT_A_Documento",
                column: "TN_IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_DocumentoAsamblea_TN_IdAsamblea_TN_IdDocumento",
                table: "THBT_A_DocumentoAsamblea",
                columns: new[] { "TN_IdAsamblea", "TN_IdDocumento" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_DocumentoAsamblea_TN_IdDocumento",
                table: "THBT_A_DocumentoAsamblea",
                column: "TN_IdDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_ParticipanteAsamblea_TC_IdUsuario",
                table: "THBT_A_ParticipanteAsamblea",
                column: "TC_IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_ParticipanteAsamblea_TN_IdAsamblea_TC_IdUsuario",
                table: "THBT_A_ParticipanteAsamblea",
                columns: new[] { "TN_IdAsamblea", "TC_IdUsuario" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_THBT_CAT_CategoriaDocumento_TC_Nombre",
                table: "THBT_CAT_CategoriaDocumento",
                column: "TC_Nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "THBT_A_DocumentoAsamblea");

            migrationBuilder.DropTable(
                name: "THBT_A_ParticipanteAsamblea");

            migrationBuilder.DropTable(
                name: "THBT_A_Documento");

            migrationBuilder.DropTable(
                name: "THBT_A_Asamblea");

            migrationBuilder.DropTable(
                name: "THBT_CAT_CategoriaDocumento");
        }
    }
}
