using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysJudo.Infra.Migrations
{
    public partial class removedIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agremiacoes_Cidades_IdCidade",
                table: "Agremiacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Agremiacoes_Estados_IdEstado",
                table: "Agremiacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Agremiacoes_Paises_IdPais",
                table: "Agremiacoes");

            migrationBuilder.DropIndex(
                name: "IX_Agremiacoes_IdCidade",
                table: "Agremiacoes");

            migrationBuilder.DropIndex(
                name: "IX_Agremiacoes_IdEstado",
                table: "Agremiacoes");

            migrationBuilder.DropIndex(
                name: "IX_Agremiacoes_IdPais",
                table: "Agremiacoes");

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "Agremiacoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CidadeId",
                table: "Agremiacoes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Agremiacoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EstadoId",
                table: "Agremiacoes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pais",
                table: "Agremiacoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PaisId",
                table: "Agremiacoes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agremiacoes_CidadeId",
                table: "Agremiacoes",
                column: "CidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Agremiacoes_EstadoId",
                table: "Agremiacoes",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Agremiacoes_PaisId",
                table: "Agremiacoes",
                column: "PaisId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agremiacoes_Cidades_CidadeId",
                table: "Agremiacoes",
                column: "CidadeId",
                principalTable: "Cidades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Agremiacoes_Estados_EstadoId",
                table: "Agremiacoes",
                column: "EstadoId",
                principalTable: "Estados",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Agremiacoes_Paises_PaisId",
                table: "Agremiacoes",
                column: "PaisId",
                principalTable: "Paises",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agremiacoes_Cidades_CidadeId",
                table: "Agremiacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Agremiacoes_Estados_EstadoId",
                table: "Agremiacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Agremiacoes_Paises_PaisId",
                table: "Agremiacoes");

            migrationBuilder.DropIndex(
                name: "IX_Agremiacoes_CidadeId",
                table: "Agremiacoes");

            migrationBuilder.DropIndex(
                name: "IX_Agremiacoes_EstadoId",
                table: "Agremiacoes");

            migrationBuilder.DropIndex(
                name: "IX_Agremiacoes_PaisId",
                table: "Agremiacoes");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "Agremiacoes");

            migrationBuilder.DropColumn(
                name: "CidadeId",
                table: "Agremiacoes");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Agremiacoes");

            migrationBuilder.DropColumn(
                name: "EstadoId",
                table: "Agremiacoes");

            migrationBuilder.DropColumn(
                name: "Pais",
                table: "Agremiacoes");

            migrationBuilder.DropColumn(
                name: "PaisId",
                table: "Agremiacoes");

            migrationBuilder.CreateIndex(
                name: "IX_Agremiacoes_IdCidade",
                table: "Agremiacoes",
                column: "IdCidade");

            migrationBuilder.CreateIndex(
                name: "IX_Agremiacoes_IdEstado",
                table: "Agremiacoes",
                column: "IdEstado");

            migrationBuilder.CreateIndex(
                name: "IX_Agremiacoes_IdPais",
                table: "Agremiacoes",
                column: "IdPais");

            migrationBuilder.AddForeignKey(
                name: "FK_Agremiacoes_Cidades_IdCidade",
                table: "Agremiacoes",
                column: "IdCidade",
                principalTable: "Cidades",
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
        }
    }
}
