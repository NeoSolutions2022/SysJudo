using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysJudo.Infra.Migrations
{
    public partial class RemoveSelecao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agremiacao_Cidade_IdCidade",
                table: "Agremiacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Agremiacao_Clientes_ClienteId",
                table: "Agremiacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Agremiacao_Estado_IdEstado",
                table: "Agremiacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Agremiacao_Pais_IdPais",
                table: "Agremiacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Agremiacao_Regiao_IdRegiao",
                table: "Agremiacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Cidade_Clientes_ClienteId",
                table: "Cidade");

            migrationBuilder.DropForeignKey(
                name: "FK_Cidade_Estado_IdEstado",
                table: "Cidade");

            migrationBuilder.DropForeignKey(
                name: "FK_Cidade_Pais_IdPais",
                table: "Cidade");

            migrationBuilder.DropForeignKey(
                name: "FK_Estado_Clientes_ClienteId",
                table: "Estado");

            migrationBuilder.DropForeignKey(
                name: "FK_Estado_Pais_IdPais",
                table: "Estado");

            migrationBuilder.DropForeignKey(
                name: "FK_Pais_Clientes_ClienteId",
                table: "Pais");

            migrationBuilder.DropForeignKey(
                name: "FK_Regiao_Cidade_IdCidade",
                table: "Regiao");

            migrationBuilder.DropForeignKey(
                name: "FK_Regiao_Estado_IdEstado",
                table: "Regiao");

            migrationBuilder.DropForeignKey(
                name: "FK_Regiao_Pais_IdPais",
                table: "Regiao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pais",
                table: "Pais");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Estado",
                table: "Estado");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cidade",
                table: "Cidade");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Agremiacao",
                table: "Agremiacao");

            migrationBuilder.RenameTable(
                name: "Pais",
                newName: "Paises");

            migrationBuilder.RenameTable(
                name: "Estado",
                newName: "Estados");

            migrationBuilder.RenameTable(
                name: "Cidade",
                newName: "Cidades");

            migrationBuilder.RenameTable(
                name: "Agremiacao",
                newName: "Agremiacoes");

            migrationBuilder.RenameIndex(
                name: "IX_Pais_ClienteId",
                table: "Paises",
                newName: "IX_Paises_ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Estado_IdPais",
                table: "Estados",
                newName: "IX_Estados_IdPais");

            migrationBuilder.RenameIndex(
                name: "IX_Estado_ClienteId",
                table: "Estados",
                newName: "IX_Estados_ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Cidade_IdPais",
                table: "Cidades",
                newName: "IX_Cidades_IdPais");

            migrationBuilder.RenameIndex(
                name: "IX_Cidade_IdEstado",
                table: "Cidades",
                newName: "IX_Cidades_IdEstado");

            migrationBuilder.RenameIndex(
                name: "IX_Cidade_ClienteId",
                table: "Cidades",
                newName: "IX_Cidades_ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Agremiacao_IdRegiao",
                table: "Agremiacoes",
                newName: "IX_Agremiacoes_IdRegiao");

            migrationBuilder.RenameIndex(
                name: "IX_Agremiacao_IdPais",
                table: "Agremiacoes",
                newName: "IX_Agremiacoes_IdPais");

            migrationBuilder.RenameIndex(
                name: "IX_Agremiacao_IdEstado",
                table: "Agremiacoes",
                newName: "IX_Agremiacoes_IdEstado");

            migrationBuilder.RenameIndex(
                name: "IX_Agremiacao_IdCidade",
                table: "Agremiacoes",
                newName: "IX_Agremiacoes_IdCidade");

            migrationBuilder.RenameIndex(
                name: "IX_Agremiacao_ClienteId",
                table: "Agremiacoes",
                newName: "IX_Agremiacoes_ClienteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Paises",
                table: "Paises",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Estados",
                table: "Estados",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cidades",
                table: "Cidades",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Agremiacoes",
                table: "Agremiacoes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Agremiacoes_Cidades_IdCidade",
                table: "Agremiacoes",
                column: "IdCidade",
                principalTable: "Cidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agremiacoes_Clientes_ClienteId",
                table: "Agremiacoes",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agremiacoes_Estados_IdEstado",
                table: "Agremiacoes",
                column: "IdEstado",
                principalTable: "Estados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agremiacoes_Paises_IdPais",
                table: "Agremiacoes",
                column: "IdPais",
                principalTable: "Paises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agremiacoes_Regiao_IdRegiao",
                table: "Agremiacoes",
                column: "IdRegiao",
                principalTable: "Regiao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cidades_Clientes_ClienteId",
                table: "Cidades",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cidades_Estados_IdEstado",
                table: "Cidades",
                column: "IdEstado",
                principalTable: "Estados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cidades_Paises_IdPais",
                table: "Cidades",
                column: "IdPais",
                principalTable: "Paises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Estados_Clientes_ClienteId",
                table: "Estados",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Estados_Paises_IdPais",
                table: "Estados",
                column: "IdPais",
                principalTable: "Paises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Paises_Clientes_ClienteId",
                table: "Paises",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Regiao_Cidades_IdCidade",
                table: "Regiao",
                column: "IdCidade",
                principalTable: "Cidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Regiao_Estados_IdEstado",
                table: "Regiao",
                column: "IdEstado",
                principalTable: "Estados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Regiao_Paises_IdPais",
                table: "Regiao",
                column: "IdPais",
                principalTable: "Paises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agremiacoes_Cidades_IdCidade",
                table: "Agremiacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Agremiacoes_Clientes_ClienteId",
                table: "Agremiacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Agremiacoes_Estados_IdEstado",
                table: "Agremiacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Agremiacoes_Paises_IdPais",
                table: "Agremiacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Agremiacoes_Regiao_IdRegiao",
                table: "Agremiacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Cidades_Clientes_ClienteId",
                table: "Cidades");

            migrationBuilder.DropForeignKey(
                name: "FK_Cidades_Estados_IdEstado",
                table: "Cidades");

            migrationBuilder.DropForeignKey(
                name: "FK_Cidades_Paises_IdPais",
                table: "Cidades");

            migrationBuilder.DropForeignKey(
                name: "FK_Estados_Clientes_ClienteId",
                table: "Estados");

            migrationBuilder.DropForeignKey(
                name: "FK_Estados_Paises_IdPais",
                table: "Estados");

            migrationBuilder.DropForeignKey(
                name: "FK_Paises_Clientes_ClienteId",
                table: "Paises");

            migrationBuilder.DropForeignKey(
                name: "FK_Regiao_Cidades_IdCidade",
                table: "Regiao");

            migrationBuilder.DropForeignKey(
                name: "FK_Regiao_Estados_IdEstado",
                table: "Regiao");

            migrationBuilder.DropForeignKey(
                name: "FK_Regiao_Paises_IdPais",
                table: "Regiao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Paises",
                table: "Paises");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Estados",
                table: "Estados");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cidades",
                table: "Cidades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Agremiacoes",
                table: "Agremiacoes");

            migrationBuilder.RenameTable(
                name: "Paises",
                newName: "Pais");

            migrationBuilder.RenameTable(
                name: "Estados",
                newName: "Estado");

            migrationBuilder.RenameTable(
                name: "Cidades",
                newName: "Cidade");

            migrationBuilder.RenameTable(
                name: "Agremiacoes",
                newName: "Agremiacao");

            migrationBuilder.RenameIndex(
                name: "IX_Paises_ClienteId",
                table: "Pais",
                newName: "IX_Pais_ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Estados_IdPais",
                table: "Estado",
                newName: "IX_Estado_IdPais");

            migrationBuilder.RenameIndex(
                name: "IX_Estados_ClienteId",
                table: "Estado",
                newName: "IX_Estado_ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Cidades_IdPais",
                table: "Cidade",
                newName: "IX_Cidade_IdPais");

            migrationBuilder.RenameIndex(
                name: "IX_Cidades_IdEstado",
                table: "Cidade",
                newName: "IX_Cidade_IdEstado");

            migrationBuilder.RenameIndex(
                name: "IX_Cidades_ClienteId",
                table: "Cidade",
                newName: "IX_Cidade_ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Agremiacoes_IdRegiao",
                table: "Agremiacao",
                newName: "IX_Agremiacao_IdRegiao");

            migrationBuilder.RenameIndex(
                name: "IX_Agremiacoes_IdPais",
                table: "Agremiacao",
                newName: "IX_Agremiacao_IdPais");

            migrationBuilder.RenameIndex(
                name: "IX_Agremiacoes_IdEstado",
                table: "Agremiacao",
                newName: "IX_Agremiacao_IdEstado");

            migrationBuilder.RenameIndex(
                name: "IX_Agremiacoes_IdCidade",
                table: "Agremiacao",
                newName: "IX_Agremiacao_IdCidade");

            migrationBuilder.RenameIndex(
                name: "IX_Agremiacoes_ClienteId",
                table: "Agremiacao",
                newName: "IX_Agremiacao_ClienteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pais",
                table: "Pais",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Estado",
                table: "Estado",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cidade",
                table: "Cidade",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Agremiacao",
                table: "Agremiacao",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Agremiacao_Cidade_IdCidade",
                table: "Agremiacao",
                column: "IdCidade",
                principalTable: "Cidade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agremiacao_Clientes_ClienteId",
                table: "Agremiacao",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agremiacao_Estado_IdEstado",
                table: "Agremiacao",
                column: "IdEstado",
                principalTable: "Estado",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agremiacao_Pais_IdPais",
                table: "Agremiacao",
                column: "IdPais",
                principalTable: "Pais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agremiacao_Regiao_IdRegiao",
                table: "Agremiacao",
                column: "IdRegiao",
                principalTable: "Regiao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cidade_Clientes_ClienteId",
                table: "Cidade",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cidade_Estado_IdEstado",
                table: "Cidade",
                column: "IdEstado",
                principalTable: "Estado",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cidade_Pais_IdPais",
                table: "Cidade",
                column: "IdPais",
                principalTable: "Pais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Estado_Clientes_ClienteId",
                table: "Estado",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Estado_Pais_IdPais",
                table: "Estado",
                column: "IdPais",
                principalTable: "Pais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pais_Clientes_ClienteId",
                table: "Pais",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Regiao_Cidade_IdCidade",
                table: "Regiao",
                column: "IdCidade",
                principalTable: "Cidade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Regiao_Estado_IdEstado",
                table: "Regiao",
                column: "IdEstado",
                principalTable: "Estado",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Regiao_Pais_IdPais",
                table: "Regiao",
                column: "IdPais",
                principalTable: "Pais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
