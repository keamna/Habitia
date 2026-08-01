using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Habitia.Migrations
{
    /// <inheritdoc />
    public partial class Prueba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "THBT_A_ConfiguracionPago",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TB_EfectivoHabilitado = table.Column<bool>(type: "bit", nullable: false),
                    TB_TarjetaHabilitado = table.Column<bool>(type: "bit", nullable: false),
                    TB_SinpeHabilitado = table.Column<bool>(type: "bit", nullable: false),
                    TC_TitularTarjeta = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TC_IbanTarjeta = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    TC_TitularSinpe = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TC_NumeroSinpe = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    TF_FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TB_Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_ConfiguracionPago", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_CAT_EstadoCargo",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_CAT_EstadoCargo", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_CAT_MetodoPago",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_CAT_MetodoPago", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_CAT_TipoCargo",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_CAT_TipoCargo", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_CAT_TipoRecargo",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_CAT_TipoRecargo", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_CAT_TipoTarjeta",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_CAT_TipoTarjeta", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_H_Cargo",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdCargo = table.Column<int>(type: "int", nullable: false),
                    TN_IdEstadoCargoAnterior = table.Column<int>(type: "int", nullable: false),
                    TN_IdEstadoCargoNuevo = table.Column<int>(type: "int", nullable: false),
                    TF_FechaCambio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TC_IdUsuarioCambio = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_H_Cargo", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_H_ConfiguracionPago",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdConfiguracionPago = table.Column<int>(type: "int", nullable: false),
                    TB_EfectivoHabilitado = table.Column<bool>(type: "bit", nullable: false),
                    TB_TarjetaHabilitado = table.Column<bool>(type: "bit", nullable: false),
                    TB_SinpeHabilitado = table.Column<bool>(type: "bit", nullable: false),
                    TC_TitularTarjeta = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TC_IbanTarjeta = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    TC_TitularSinpe = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TC_NumeroSinpe = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    TF_FechaCambio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TC_IdUsuarioCambio = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_H_ConfiguracionPago", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_H_Pago",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdPago = table.Column<int>(type: "int", nullable: false),
                    TB_Aprobado = table.Column<bool>(type: "bit", nullable: false),
                    TC_MotivoRechazo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TF_FechaCambio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TC_IdUsuarioCambio = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_H_Pago", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Cargo",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_IdResidente = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TN_IdTipoCargo = table.Column<int>(type: "int", nullable: false),
                    TN_IdEstadoCargo = table.Column<int>(type: "int", nullable: false),
                    TC_Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TN_MontoBase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TN_MontoIva = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TN_MontoTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TF_FechaEmision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TF_FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TB_Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Cargo", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Cargo_THBT_A_Usuario_TC_IdResidente",
                        column: x => x.TC_IdResidente,
                        principalTable: "THBT_A_Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Cargo_THBT_CAT_EstadoCargo_TN_IdEstadoCargo",
                        column: x => x.TN_IdEstadoCargo,
                        principalTable: "THBT_CAT_EstadoCargo",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Cargo_THBT_CAT_TipoCargo_TN_IdTipoCargo",
                        column: x => x.TN_IdTipoCargo,
                        principalTable: "THBT_CAT_TipoCargo",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Pago",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdCargo = table.Column<int>(type: "int", nullable: false),
                    TN_IdMetodoPago = table.Column<int>(type: "int", nullable: false),
                    TC_RutaComprobante = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TF_FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TC_MotivoRechazo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TB_Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Pago", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Pago_THBT_A_Cargo_TN_IdCargo",
                        column: x => x.TN_IdCargo,
                        principalTable: "THBT_A_Cargo",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Pago_THBT_CAT_MetodoPago_TN_IdMetodoPago",
                        column: x => x.TN_IdMetodoPago,
                        principalTable: "THBT_CAT_MetodoPago",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "THBT_A_Recargo",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_IdCargo = table.Column<int>(type: "int", nullable: false),
                    TN_IdTipoRecargo = table.Column<int>(type: "int", nullable: false),
                    TN_Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TN_MontoAplicado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TF_FechaAplicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TB_Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THBT_A_Recargo", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_THBT_A_Recargo_THBT_A_Cargo_TN_IdCargo",
                        column: x => x.TN_IdCargo,
                        principalTable: "THBT_A_Cargo",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THBT_A_Recargo_THBT_CAT_TipoRecargo_TN_IdTipoRecargo",
                        column: x => x.TN_IdTipoRecargo,
                        principalTable: "THBT_CAT_TipoRecargo",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "THBT_CAT_EstadoCargo",
                columns: new[] { "TN_Id", "TC_Nombre" },
                values: new object[,]
                {
                    { 1, "Pendiente" },
                    { 2, "En revisión" },
                    { 3, "Pagado" }
                });

            migrationBuilder.InsertData(
                table: "THBT_CAT_MetodoPago",
                columns: new[] { "TN_Id", "TC_Nombre" },
                values: new object[,]
                {
                    { 1, "Efectivo" },
                    { 2, "Tarjeta" },
                    { 3, "SINPE" }
                });

            migrationBuilder.InsertData(
                table: "THBT_CAT_TipoRecargo",
                columns: new[] { "TN_Id", "TC_Nombre" },
                values: new object[,]
                {
                    { 1, "Fijo" },
                    { 2, "Porcentaje" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Cargo_TC_IdResidente",
                table: "THBT_A_Cargo",
                column: "TC_IdResidente");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Cargo_TF_FechaVencimiento",
                table: "THBT_A_Cargo",
                column: "TF_FechaVencimiento");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Cargo_TN_IdEstadoCargo",
                table: "THBT_A_Cargo",
                column: "TN_IdEstadoCargo");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Cargo_TN_IdTipoCargo",
                table: "THBT_A_Cargo",
                column: "TN_IdTipoCargo");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Pago_TN_IdCargo",
                table: "THBT_A_Pago",
                column: "TN_IdCargo");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Pago_TN_IdMetodoPago",
                table: "THBT_A_Pago",
                column: "TN_IdMetodoPago");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Recargo_TN_IdCargo",
                table: "THBT_A_Recargo",
                column: "TN_IdCargo");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_A_Recargo_TN_IdTipoRecargo",
                table: "THBT_A_Recargo",
                column: "TN_IdTipoRecargo");

            migrationBuilder.CreateIndex(
                name: "IX_THBT_CAT_TipoCargo_TC_Nombre",
                table: "THBT_CAT_TipoCargo",
                column: "TC_Nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "THBT_A_ConfiguracionPago");

            migrationBuilder.DropTable(
                name: "THBT_A_Pago");

            migrationBuilder.DropTable(
                name: "THBT_A_Recargo");

            migrationBuilder.DropTable(
                name: "THBT_CAT_TipoTarjeta");

            migrationBuilder.DropTable(
                name: "THBT_H_Cargo");

            migrationBuilder.DropTable(
                name: "THBT_H_ConfiguracionPago");

            migrationBuilder.DropTable(
                name: "THBT_H_Pago");

            migrationBuilder.DropTable(
                name: "THBT_CAT_MetodoPago");

            migrationBuilder.DropTable(
                name: "THBT_A_Cargo");

            migrationBuilder.DropTable(
                name: "THBT_CAT_TipoRecargo");

            migrationBuilder.DropTable(
                name: "THBT_CAT_EstadoCargo");

            migrationBuilder.DropTable(
                name: "THBT_CAT_TipoCargo");
        }
    }
}
