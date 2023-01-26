using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysJudo.Infra.Migrations
{
    public partial class AddEntityAgremiacaoFiltro : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AgremiacoesFiltro",
                columns: table => new
                {
                    Identificador = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Fantasia = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Responsavel = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Representante = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Conteudo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    DataFiliacao = table.Column<DateTime>(type: "DATE", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "DATE", nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Complemento = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    InscricaoMunicipal = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    InscricaoEstadual = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    DataCnpj = table.Column<DateTime>(type: "DATE", nullable: true),
                    DataAta = table.Column<DateTime>(type: "DATE", nullable: true),
                    Foto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlvaraLocacao = table.Column<string>(type: "nchar(255)", fixedLength: true, maxLength: 255, nullable: true),
                    Estatuto = table.Column<string>(type: "nchar(255)", fixedLength: true, maxLength: 255, nullable: true),
                    ContratoSocial = table.Column<string>(type: "nchar(255)", fixedLength: true, maxLength: 255, nullable: true),
                    DocumentacaoAtualizada = table.Column<string>(type: "nchar(255)", fixedLength: true, maxLength: 255, nullable: true),
                    Anotacoes = table.Column<string>(type: "nvarchar(1200)", maxLength: 1200, nullable: true),
                    IdPais = table.Column<int>(type: "int", nullable: false),
                    IdCidade = table.Column<int>(type: "int", nullable: false),
                    IdEstado = table.Column<int>(type: "int", nullable: false),
                    IdRegiao = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgremiacoesFiltro", x => x.Identificador);
                    table.ForeignKey(
                        name: "FK_AgremiacoesFiltro_Cidades_IdCidade",
                        column: x => x.IdCidade,
                        principalTable: "Cidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AgremiacoesFiltro_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AgremiacoesFiltro_Estados_IdEstado",
                        column: x => x.IdEstado,
                        principalTable: "Estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AgremiacoesFiltro_Paises_IdPais",
                        column: x => x.IdPais,
                        principalTable: "Paises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AgremiacoesFiltro_Regioes_IdRegiao",
                        column: x => x.IdRegiao,
                        principalTable: "Regioes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AgremiacaoFiltroAtleta",
                columns: table => new
                {
                    AgremiacoesFiltroIdentificador = table.Column<int>(type: "int", nullable: false),
                    AtletasId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgremiacaoFiltroAtleta", x => new { x.AgremiacoesFiltroIdentificador, x.AtletasId });
                    table.ForeignKey(
                        name: "FK_AgremiacaoFiltroAtleta_AgremiacoesFiltro_AgremiacoesFiltroIdentificador",
                        column: x => x.AgremiacoesFiltroIdentificador,
                        principalTable: "AgremiacoesFiltro",
                        principalColumn: "Identificador",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgremiacaoFiltroAtleta_Atletas_AtletasId",
                        column: x => x.AtletasId,
                        principalTable: "Atletas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgremiacaoFiltroAtleta_AtletasId",
                table: "AgremiacaoFiltroAtleta",
                column: "AtletasId");

            migrationBuilder.CreateIndex(
                name: "IX_AgremiacoesFiltro_ClienteId",
                table: "AgremiacoesFiltro",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_AgremiacoesFiltro_IdCidade",
                table: "AgremiacoesFiltro",
                column: "IdCidade");

            migrationBuilder.CreateIndex(
                name: "IX_AgremiacoesFiltro_IdEstado",
                table: "AgremiacoesFiltro",
                column: "IdEstado");

            migrationBuilder.CreateIndex(
                name: "IX_AgremiacoesFiltro_IdPais",
                table: "AgremiacoesFiltro",
                column: "IdPais");

            migrationBuilder.CreateIndex(
                name: "IX_AgremiacoesFiltro_IdRegiao",
                table: "AgremiacoesFiltro",
                column: "IdRegiao");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgremiacaoFiltroAtleta");

            migrationBuilder.DropTable(
                name: "AgremiacoesFiltro");
        }
    }
}
