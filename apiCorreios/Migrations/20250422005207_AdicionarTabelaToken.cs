using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apiCorreios.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarTabelaToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartaoPostagem",
                columns: table => new
                {
                    numero = table.Column<string>(type: "TEXT", nullable: false),
                    contrato = table.Column<string>(type: "TEXT", nullable: false),
                    dr = table.Column<int>(type: "INTEGER", nullable: false),
                    api = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartaoPostagem", x => x.numero);
                });

            migrationBuilder.CreateTable(
                name: "Contrato",
                columns: table => new
                {
                    numero = table.Column<string>(type: "TEXT", nullable: false),
                    dr = table.Column<int>(type: "INTEGER", nullable: false),
                    api = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contrato", x => x.numero);
                });

            migrationBuilder.CreateTable(
                name: "cToken",
                columns: table => new
                {
                    codigo = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ambiente = table.Column<string>(type: "TEXT", nullable: false),
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    ip = table.Column<string>(type: "TEXT", nullable: false),
                    perfil = table.Column<string>(type: "TEXT", nullable: false),
                    cnpj = table.Column<string>(type: "TEXT", nullable: false),
                    pjInternacional = table.Column<int>(type: "INTEGER", nullable: true),
                    cpf = table.Column<string>(type: "TEXT", nullable: false),
                    cie = table.Column<string>(type: "TEXT", nullable: false),
                    cartaoPostagemnumero = table.Column<string>(type: "TEXT", nullable: false),
                    contratonumero = table.Column<string>(type: "TEXT", nullable: false),
                    emissao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    expiraEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    zoneOffset = table.Column<string>(type: "TEXT", nullable: false),
                    token = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cToken", x => x.codigo);
                    table.ForeignKey(
                        name: "FK_cToken_CartaoPostagem_cartaoPostagemnumero",
                        column: x => x.cartaoPostagemnumero,
                        principalTable: "CartaoPostagem",
                        principalColumn: "numero",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cToken_Contrato_contratonumero",
                        column: x => x.contratonumero,
                        principalTable: "Contrato",
                        principalColumn: "numero",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cToken_cartaoPostagemnumero",
                table: "cToken",
                column: "cartaoPostagemnumero");

            migrationBuilder.CreateIndex(
                name: "IX_cToken_contratonumero",
                table: "cToken",
                column: "contratonumero");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cToken");

            migrationBuilder.DropTable(
                name: "CartaoPostagem");

            migrationBuilder.DropTable(
                name: "Contrato");
        }
    }
}
