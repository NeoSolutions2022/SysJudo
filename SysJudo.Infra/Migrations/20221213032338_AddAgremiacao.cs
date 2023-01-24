using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysJudo.Infra.Migrations
{
    public partial class AddAgremiacao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdFaixa",
                table: "Clientes");

            migrationBuilder.CreateTable(
                name: "Pais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: false),
                    Sigla3 = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    Sigla2 = table.Column<string>(type: "varchar(2)", unicode: false, maxLength: 2, nullable: false),
                    Nacionalidade = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    Selecao = table.Column<bool>(type: "bit", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pais_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Estado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "varchar(2)", unicode: false, maxLength: 2, nullable: false),
                    Descricao = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: false),
                    Selecao = table.Column<bool>(type: "bit", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    IdEstado = table.Column<int>(type: "int", nullable: false),
                    IdPais = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estado_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Estado_Pais_IdPais",
                        column: x => x.IdPais,
                        principalTable: "Pais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cidade",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: false),
                    Selecao = table.Column<bool>(type: "bit", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    IdPais = table.Column<int>(type: "int", nullable: false),
                    IdEstado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cidade", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cidade_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cidade_Estado_IdEstado",
                        column: x => x.IdEstado,
                        principalTable: "Estado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cidade_Pais_IdPais",
                        column: x => x.IdPais,
                        principalTable: "Pais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Regiao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Responsavel = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Cep = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Complemento = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    ANOTACOES = table.Column<string>(type: "text", nullable: true),
                    Selecao = table.Column<bool>(type: "bit", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    IdCidade = table.Column<int>(type: "int", nullable: false),
                    IdEstado = table.Column<int>(type: "int", nullable: false),
                    IdPais = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regiao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Regiao_Cidade_IdCidade",
                        column: x => x.IdCidade,
                        principalTable: "Cidade",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Regiao_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Regiao_Estado_IdEstado",
                        column: x => x.IdEstado,
                        principalTable: "Estado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Regiao_Pais_IdPais",
                        column: x => x.IdPais,
                        principalTable: "Pais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Agremiacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Fantasia = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Responsavel = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Representante = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    DataFiliacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Complemento = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    InscricaoMunicipal = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    InscricaoEstadual = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    DataCnpj = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataAta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AlvaraLocacao = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: true),
                    Estatuto = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: true),
                    ContratoSocial = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: true),
                    DocumentacaoAtualizada = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: true),
                    Anotacoes = table.Column<string>(type: "nvarchar(1200)", maxLength: 1200, nullable: true),
                    Selecao = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IdPais = table.Column<int>(type: "int", nullable: false),
                    IdCidade = table.Column<int>(type: "int", nullable: false),
                    IdEstado = table.Column<int>(type: "int", nullable: false),
                    IdRegiao = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agremiacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agremiacao_Cidade_IdCidade",
                        column: x => x.IdCidade,
                        principalTable: "Cidade",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Agremiacao_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Agremiacao_Estado_IdEstado",
                        column: x => x.IdEstado,
                        principalTable: "Estado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Agremiacao_Pais_IdPais",
                        column: x => x.IdPais,
                        principalTable: "Pais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Agremiacao_Regiao_IdRegiao",
                        column: x => x.IdRegiao,
                        principalTable: "Regiao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agremiacao_ClienteId",
                table: "Agremiacao",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Agremiacao_IdCidade",
                table: "Agremiacao",
                column: "IdCidade");

            migrationBuilder.CreateIndex(
                name: "IX_Agremiacao_IdEstado",
                table: "Agremiacao",
                column: "IdEstado");

            migrationBuilder.CreateIndex(
                name: "IX_Agremiacao_IdPais",
                table: "Agremiacao",
                column: "IdPais");

            migrationBuilder.CreateIndex(
                name: "IX_Agremiacao_IdRegiao",
                table: "Agremiacao",
                column: "IdRegiao");

            migrationBuilder.CreateIndex(
                name: "IX_Cidade_ClienteId",
                table: "Cidade",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Cidade_IdEstado",
                table: "Cidade",
                column: "IdEstado");

            migrationBuilder.CreateIndex(
                name: "IX_Cidade_IdPais",
                table: "Cidade",
                column: "IdPais");

            migrationBuilder.CreateIndex(
                name: "IX_Estado_ClienteId",
                table: "Estado",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Estado_IdPais",
                table: "Estado",
                column: "IdPais");

            migrationBuilder.CreateIndex(
                name: "IX_Pais_ClienteId",
                table: "Pais",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Regiao_ClienteId",
                table: "Regiao",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Regiao_IdCidade",
                table: "Regiao",
                column: "IdCidade");

            migrationBuilder.CreateIndex(
                name: "IX_Regiao_IdEstado",
                table: "Regiao",
                column: "IdEstado");

            migrationBuilder.CreateIndex(
                name: "IX_Regiao_IdPais",
                table: "Regiao",
                column: "IdPais");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agremiacao");

            migrationBuilder.DropTable(
                name: "Regiao");

            migrationBuilder.DropTable(
                name: "Cidade");

            migrationBuilder.DropTable(
                name: "Estado");

            migrationBuilder.DropTable(
                name: "Pais");

            migrationBuilder.AddColumn<int>(
                name: "IdFaixa",
                table: "Clientes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
