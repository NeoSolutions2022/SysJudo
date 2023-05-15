using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysJudo.Infra.Migrations
{
    public partial class AddUsuarioNomeEAdmNome : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdministradorNome",
                table: "RegistroDeEventos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioNome",
                table: "RegistroDeEventos",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdministradorNome",
                table: "RegistroDeEventos");

            migrationBuilder.DropColumn(
                name: "UsuarioNome",
                table: "RegistroDeEventos");
        }
    }
}
