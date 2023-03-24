using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysJudo.Infra.Migrations
{
    public partial class RemovendoNomeCidadeNomePaisNomeEstado : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaisNome",
                table: "AgremiacoesFiltro",
                newName: "Pais");

            migrationBuilder.RenameColumn(
                name: "EstadoNome",
                table: "AgremiacoesFiltro",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "CidadeNome",
                table: "AgremiacoesFiltro",
                newName: "Cidade");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Pais",
                table: "AgremiacoesFiltro",
                newName: "PaisNome");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "AgremiacoesFiltro",
                newName: "EstadoNome");

            migrationBuilder.RenameColumn(
                name: "Cidade",
                table: "AgremiacoesFiltro",
                newName: "CidadeNome");
        }
    }
}
