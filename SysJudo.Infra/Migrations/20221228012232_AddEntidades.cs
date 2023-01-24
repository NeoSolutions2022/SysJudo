using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysJudo.Infra.Migrations
{
    public partial class AddEntidades : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Bairro",
                table: "Agremiacoes",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "EmissoresIdentidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmissoresIdentidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmissoresIdentidades_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EstadosCivis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosCivis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstadosCivis_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Nacionalidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nacionalidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nacionalidades_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Profissoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profissoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profissoes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sexos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sexos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Atletas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistroFederacao = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RegistroConfederacao = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFiliacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Complemento = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Cpf = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Identidade = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DataIdentidade = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NomePai = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    NomeMae = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Anotacoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdFaixa = table.Column<int>(type: "int", nullable: false),
                    IdSexo = table.Column<int>(type: "int", nullable: false),
                    IdEstadoCivil = table.Column<int>(type: "int", nullable: false),
                    IdProfissaoAtleta = table.Column<int>(type: "int", nullable: false),
                    IdProfissaoMae = table.Column<int>(type: "int", nullable: true),
                    IdProfissaoPai = table.Column<int>(type: "int", nullable: true),
                    IdEmissor = table.Column<int>(type: "int", nullable: false),
                    IdNacionalidade = table.Column<int>(type: "int", nullable: false),
                    IdCidade = table.Column<int>(type: "int", nullable: false),
                    IdEstado = table.Column<int>(type: "int", nullable: false),
                    IdPais = table.Column<int>(type: "int", nullable: false),
                    IdAgremiacao = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atletas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Atletas_Agremiacoes_IdAgremiacao",
                        column: x => x.IdAgremiacao,
                        principalTable: "Agremiacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atletas_Cidades_IdCidade",
                        column: x => x.IdCidade,
                        principalTable: "Cidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atletas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atletas_EmissoresIdentidades_IdEmissor",
                        column: x => x.IdEmissor,
                        principalTable: "EmissoresIdentidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atletas_Estados_IdEstado",
                        column: x => x.IdEstado,
                        principalTable: "Estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atletas_EstadosCivis_IdEstadoCivil",
                        column: x => x.IdEstadoCivil,
                        principalTable: "EstadosCivis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atletas_Faixas_IdFaixa",
                        column: x => x.IdFaixa,
                        principalTable: "Faixas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atletas_Nacionalidades_IdNacionalidade",
                        column: x => x.IdNacionalidade,
                        principalTable: "Nacionalidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atletas_Paises_IdPais",
                        column: x => x.IdPais,
                        principalTable: "Paises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atletas_Profissoes_IdProfissaoAtleta",
                        column: x => x.IdProfissaoAtleta,
                        principalTable: "Profissoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atletas_Sexos_IdSexo",
                        column: x => x.IdSexo,
                        principalTable: "Sexos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_ClienteId",
                table: "Atletas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_IdAgremiacao",
                table: "Atletas",
                column: "IdAgremiacao");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_IdCidade",
                table: "Atletas",
                column: "IdCidade");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_IdEmissor",
                table: "Atletas",
                column: "IdEmissor");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_IdEstado",
                table: "Atletas",
                column: "IdEstado");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_IdEstadoCivil",
                table: "Atletas",
                column: "IdEstadoCivil");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_IdFaixa",
                table: "Atletas",
                column: "IdFaixa");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_IdNacionalidade",
                table: "Atletas",
                column: "IdNacionalidade");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_IdPais",
                table: "Atletas",
                column: "IdPais");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_IdProfissaoAtleta",
                table: "Atletas",
                column: "IdProfissaoAtleta");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_IdSexo",
                table: "Atletas",
                column: "IdSexo");

            migrationBuilder.CreateIndex(
                name: "IX_EmissoresIdentidades_ClienteId",
                table: "EmissoresIdentidades",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_EstadosCivis_ClienteId",
                table: "EstadosCivis",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Nacionalidades_ClienteId",
                table: "Nacionalidades",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Profissoes_ClienteId",
                table: "Profissoes",
                column: "ClienteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Atletas");

            migrationBuilder.DropTable(
                name: "EmissoresIdentidades");

            migrationBuilder.DropTable(
                name: "EstadosCivis");

            migrationBuilder.DropTable(
                name: "Nacionalidades");

            migrationBuilder.DropTable(
                name: "Profissoes");

            migrationBuilder.DropTable(
                name: "Sexos");

            migrationBuilder.AlterColumn<string>(
                name: "Bairro",
                table: "Agremiacoes",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);
        }
    }
}
