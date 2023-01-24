using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysJudo.Infra.Migrations
{
    public partial class AjusteEmUsuarioERemovensoSelecaoDoDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agremiacoes_Regiao_IdRegiao",
                table: "Agremiacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Regiao_Cidades_IdCidade",
                table: "Regiao");

            migrationBuilder.DropForeignKey(
                name: "FK_Regiao_Clientes_ClienteId",
                table: "Regiao");

            migrationBuilder.DropForeignKey(
                name: "FK_Regiao_Estados_IdEstado",
                table: "Regiao");

            migrationBuilder.DropForeignKey(
                name: "FK_Regiao_Paises_IdPais",
                table: "Regiao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Regiao",
                table: "Regiao");

            migrationBuilder.DropColumn(
                name: "Selecao",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Situacao",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Selecao",
                table: "Sistemas");

            migrationBuilder.DropColumn(
                name: "Selecao",
                table: "Paises");

            migrationBuilder.DropColumn(
                name: "Selecao",
                table: "Faixas");

            migrationBuilder.DropColumn(
                name: "IdEstado",
                table: "Estados");

            migrationBuilder.DropColumn(
                name: "Selecao",
                table: "Estados");

            migrationBuilder.DropColumn(
                name: "Selecao",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Selecao",
                table: "Cidades");

            migrationBuilder.DropColumn(
                name: "Selecao",
                table: "Agremiacoes");

            migrationBuilder.DropColumn(
                name: "Selecao",
                table: "Administradores");

            migrationBuilder.DropColumn(
                name: "Selecao",
                table: "Regiao");

            migrationBuilder.RenameTable(
                name: "Regiao",
                newName: "Regioes");

            migrationBuilder.RenameIndex(
                name: "IX_Regiao_IdPais",
                table: "Regioes",
                newName: "IX_Regioes_IdPais");

            migrationBuilder.RenameIndex(
                name: "IX_Regiao_IdEstado",
                table: "Regioes",
                newName: "IX_Regioes_IdEstado");

            migrationBuilder.RenameIndex(
                name: "IX_Regiao_IdCidade",
                table: "Regioes",
                newName: "IX_Regioes_IdCidade");

            migrationBuilder.RenameIndex(
                name: "IX_Regiao_ClienteId",
                table: "Regioes",
                newName: "IX_Regioes_ClienteId");

            migrationBuilder.AddColumn<bool>(
                name: "Inadiplente",
                table: "Usuarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Regioes",
                table: "Regioes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Agremiacoes_Regioes_IdRegiao",
                table: "Agremiacoes",
                column: "IdRegiao",
                principalTable: "Regioes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Regioes_Cidades_IdCidade",
                table: "Regioes",
                column: "IdCidade",
                principalTable: "Cidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Regioes_Clientes_ClienteId",
                table: "Regioes",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Regioes_Estados_IdEstado",
                table: "Regioes",
                column: "IdEstado",
                principalTable: "Estados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Regioes_Paises_IdPais",
                table: "Regioes",
                column: "IdPais",
                principalTable: "Paises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agremiacoes_Regioes_IdRegiao",
                table: "Agremiacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Regioes_Cidades_IdCidade",
                table: "Regioes");

            migrationBuilder.DropForeignKey(
                name: "FK_Regioes_Clientes_ClienteId",
                table: "Regioes");

            migrationBuilder.DropForeignKey(
                name: "FK_Regioes_Estados_IdEstado",
                table: "Regioes");

            migrationBuilder.DropForeignKey(
                name: "FK_Regioes_Paises_IdPais",
                table: "Regioes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Regioes",
                table: "Regioes");

            migrationBuilder.DropColumn(
                name: "Inadiplente",
                table: "Usuarios");

            migrationBuilder.RenameTable(
                name: "Regioes",
                newName: "Regiao");

            migrationBuilder.RenameIndex(
                name: "IX_Regioes_IdPais",
                table: "Regiao",
                newName: "IX_Regiao_IdPais");

            migrationBuilder.RenameIndex(
                name: "IX_Regioes_IdEstado",
                table: "Regiao",
                newName: "IX_Regiao_IdEstado");

            migrationBuilder.RenameIndex(
                name: "IX_Regioes_IdCidade",
                table: "Regiao",
                newName: "IX_Regiao_IdCidade");

            migrationBuilder.RenameIndex(
                name: "IX_Regioes_ClienteId",
                table: "Regiao",
                newName: "IX_Regiao_ClienteId");

            migrationBuilder.AddColumn<bool>(
                name: "Selecao",
                table: "Usuarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Situacao",
                table: "Usuarios",
                type: "nvarchar(1)",
                nullable: false,
                defaultValue: "A");

            migrationBuilder.AddColumn<bool>(
                name: "Selecao",
                table: "Sistemas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Selecao",
                table: "Paises",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Selecao",
                table: "Faixas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "IdEstado",
                table: "Estados",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Selecao",
                table: "Estados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Selecao",
                table: "Clientes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Selecao",
                table: "Cidades",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Selecao",
                table: "Agremiacoes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Selecao",
                table: "Administradores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Selecao",
                table: "Regiao",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Regiao",
                table: "Regiao",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Agremiacoes_Regiao_IdRegiao",
                table: "Agremiacoes",
                column: "IdRegiao",
                principalTable: "Regiao",
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
                name: "FK_Regiao_Clientes_ClienteId",
                table: "Regiao",
                column: "ClienteId",
                principalTable: "Clientes",
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
    }
}
