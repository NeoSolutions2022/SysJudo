using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysJudo.Infra.Migrations
{
    public partial class ChangedFilter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgremiacoesFiltro_Cidades_IdCidade",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropForeignKey(
                name: "FK_AgremiacoesFiltro_Clientes_ClienteId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropForeignKey(
                name: "FK_AgremiacoesFiltro_Estados_IdEstado",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropForeignKey(
                name: "FK_AgremiacoesFiltro_Paises_IdPais",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropForeignKey(
                name: "FK_AgremiacoesFiltro_Regioes_IdRegiao",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropTable(
                name: "AgremiacaoFiltroAtleta");

            migrationBuilder.DropIndex(
                name: "IX_AgremiacoesFiltro_IdCidade",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropIndex(
                name: "IX_AgremiacoesFiltro_IdEstado",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropIndex(
                name: "IX_AgremiacoesFiltro_IdPais",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropIndex(
                name: "IX_AgremiacoesFiltro_IdRegiao",
                table: "AgremiacoesFiltro");

            migrationBuilder.AddColumn<int>(
                name: "AtletaId",
                table: "AgremiacoesFiltro",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CidadeId",
                table: "AgremiacoesFiltro",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CidadeNome",
                table: "AgremiacoesFiltro",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EstadoId",
                table: "AgremiacoesFiltro",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstadoNome",
                table: "AgremiacoesFiltro",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PaisId",
                table: "AgremiacoesFiltro",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaisNome",
                table: "AgremiacoesFiltro",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RegiaoId",
                table: "AgremiacoesFiltro",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegiaoNome",
                table: "AgremiacoesFiltro",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AgremiacoesFiltro_AtletaId",
                table: "AgremiacoesFiltro",
                column: "AtletaId");

            migrationBuilder.CreateIndex(
                name: "IX_AgremiacoesFiltro_CidadeId",
                table: "AgremiacoesFiltro",
                column: "CidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_AgremiacoesFiltro_EstadoId",
                table: "AgremiacoesFiltro",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_AgremiacoesFiltro_PaisId",
                table: "AgremiacoesFiltro",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_AgremiacoesFiltro_RegiaoId",
                table: "AgremiacoesFiltro",
                column: "RegiaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AgremiacoesFiltro_Atletas_AtletaId",
                table: "AgremiacoesFiltro",
                column: "AtletaId",
                principalTable: "Atletas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AgremiacoesFiltro_Cidades_CidadeId",
                table: "AgremiacoesFiltro",
                column: "CidadeId",
                principalTable: "Cidades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AgremiacoesFiltro_Clientes_ClienteId",
                table: "AgremiacoesFiltro",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgremiacoesFiltro_Estados_EstadoId",
                table: "AgremiacoesFiltro",
                column: "EstadoId",
                principalTable: "Estados",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AgremiacoesFiltro_Paises_PaisId",
                table: "AgremiacoesFiltro",
                column: "PaisId",
                principalTable: "Paises",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AgremiacoesFiltro_Regioes_RegiaoId",
                table: "AgremiacoesFiltro",
                column: "RegiaoId",
                principalTable: "Regioes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgremiacoesFiltro_Atletas_AtletaId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropForeignKey(
                name: "FK_AgremiacoesFiltro_Cidades_CidadeId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropForeignKey(
                name: "FK_AgremiacoesFiltro_Clientes_ClienteId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropForeignKey(
                name: "FK_AgremiacoesFiltro_Estados_EstadoId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropForeignKey(
                name: "FK_AgremiacoesFiltro_Paises_PaisId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropForeignKey(
                name: "FK_AgremiacoesFiltro_Regioes_RegiaoId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropIndex(
                name: "IX_AgremiacoesFiltro_AtletaId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropIndex(
                name: "IX_AgremiacoesFiltro_CidadeId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropIndex(
                name: "IX_AgremiacoesFiltro_EstadoId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropIndex(
                name: "IX_AgremiacoesFiltro_PaisId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropIndex(
                name: "IX_AgremiacoesFiltro_RegiaoId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropColumn(
                name: "AtletaId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropColumn(
                name: "CidadeId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropColumn(
                name: "CidadeNome",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropColumn(
                name: "EstadoId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropColumn(
                name: "EstadoNome",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropColumn(
                name: "PaisId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropColumn(
                name: "PaisNome",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropColumn(
                name: "RegiaoId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropColumn(
                name: "RegiaoNome",
                table: "AgremiacoesFiltro");

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

            migrationBuilder.CreateIndex(
                name: "IX_AgremiacaoFiltroAtleta_AtletasId",
                table: "AgremiacaoFiltroAtleta",
                column: "AtletasId");

            migrationBuilder.AddForeignKey(
                name: "FK_AgremiacoesFiltro_Cidades_IdCidade",
                table: "AgremiacoesFiltro",
                column: "IdCidade",
                principalTable: "Cidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AgremiacoesFiltro_Clientes_ClienteId",
                table: "AgremiacoesFiltro",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AgremiacoesFiltro_Estados_IdEstado",
                table: "AgremiacoesFiltro",
                column: "IdEstado",
                principalTable: "Estados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AgremiacoesFiltro_Paises_IdPais",
                table: "AgremiacoesFiltro",
                column: "IdPais",
                principalTable: "Paises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AgremiacoesFiltro_Regioes_IdRegiao",
                table: "AgremiacoesFiltro",
                column: "IdRegiao",
                principalTable: "Regioes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
