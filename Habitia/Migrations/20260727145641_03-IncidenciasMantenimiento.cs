using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class _03IncidenciasMantenimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "THBT_A_Incidencia",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TN_IdVivienda = table.Column<int>(type: "int", nullable: true),
                    TN_IdAreaComun = table.Column<int>(type: "int", nullable: true),
                    TN_Tipo = table.Column<int>(type: "int", nullable: false),
                    TN_Estado = table.Column<int>(type: "int", nullable: false),
                    TN_Responsabilidad = table.Column<int>(type: "int", nullable: false),
                    TC_Titulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TC_Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TF_FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TC_ImagenUrl = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MantenimientoTN_Id = table.Column<int>(type: "int", nullable: true),
                    TF_FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TN_Prioridad = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Incidencia", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Incidencia_THBT_A_AreaComun_TN_IdAreaComun",
                        column: x => x.TN_IdAreaComun,
                        principalTable: "THBT_A_AreaComun",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Incidencia_THBT_A_Usuario_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_THBT_A_Incidencia_THBT_A_Usuario_TC_IdUsuario",
                        column: x => x.TC_IdUsuario,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Incidencia_THBT_A_Vivienda_TN_IdVivienda",
                        column: x => x.TN_IdVivienda,
                        principalTable: "THBT_A_Vivienda",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Mantenimiento",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdIncidencia = table.Column<int>(type: "int", nullable: false),
                    TN_IdTipo = table.Column<int>(type: "int", nullable: false),
                    TN_IdAreaComun = table.Column<int>(type: "int", nullable: true),
                    TC_IdPersonalAsignado = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TC_Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TF_FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TF_FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TF_FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TN_Estado = table.Column<int>(type: "int", nullable: false),
                    TC_Observaciones = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Mantenimiento", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Mantenimiento_THBT_A_AreaComun_TN_IdAreaComun",
                        column: x => x.TN_IdAreaComun,
                        principalTable: "THBT_A_AreaComun",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Mantenimiento_THBT_A_Incidencia_TN_IdIncidencia",
                        column: x => x.TN_IdIncidencia,
                        principalTable: "THBT_A_Incidencia",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Mantenimiento_THBT_A_Usuario_TC_IdPersonalAsignado",
                        column: x => x.TC_IdPersonalAsignado,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Mantenimiento_THBT_CAT_TipoMantenimiento_TN_IdTipo",
                        column: x => x.TN_IdTipo,
                        principalTable: "THBT_CAT_TipoMantenimiento",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_ApplicationUserId",
                table: "THBT_A_Incidencia",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_TC_IdUsuario",
                table: "THBT_A_Incidencia",
                column: "TC_IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_TF_FechaRegistro",
                table: "THBT_A_Incidencia",
                column: "TF_FechaRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_TN_Estado",
                table: "THBT_A_Incidencia",
                column: "TN_Estado");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_TN_IdAreaComun",
                table: "THBT_A_Incidencia",
                column: "TN_IdAreaComun");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_TN_IdVivienda",
                table: "THBT_A_Incidencia",
                column: "TN_IdVivienda");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_TN_Responsabilidad",
                table: "THBT_A_Incidencia",
                column: "TN_Responsabilidad");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Incidencia_MantenimientoTN_Id",
                table: "THBT_A_Incidencia",
                column: "MantenimientoTN_Id");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Mantenimiento_TC_IdPersonalAsignado",
                table: "THBT_A_Mantenimiento",
                column: "TC_IdPersonalAsignado");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Mantenimiento_TF_FechaRegistro",
                table: "THBT_A_Mantenimiento",
                column: "TF_FechaRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Mantenimiento_TN_Estado",
                table: "THBT_A_Mantenimiento",
                column: "TN_Estado");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Mantenimiento_TN_IdAreaComun",
                table: "THBT_A_Mantenimiento",
                column: "TN_IdAreaComun");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Mantenimiento_TN_IdIncidencia",
                table: "THBT_A_Mantenimiento",
                column: "TN_IdIncidencia",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Mantenimiento_TN_IdTipo",
                table: "THBT_A_Mantenimiento",
                column: "TN_IdTipo");

            migrationBuilder.AddForeignKey(
                name: "FK_THBT_A_Incidencia_THBT_A_Mantenimiento_MantenimientoTN_Id",
                table: "THBT_A_Incidencia",
                column: "MantenimientoTN_Id",
                principalTable: "THBT_A_Mantenimiento",
                principalColumn: "TN_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_THBT_A_Incidencia_THBT_A_Mantenimiento_MantenimientoTN_Id",
                table: "THBT_A_Incidencia");

            migrationBuilder.DropTable(
                name: "THBT_A_Mantenimiento");

            migrationBuilder.DropTable(
                name: "THBT_A_Incidencia");
        }
    }
}