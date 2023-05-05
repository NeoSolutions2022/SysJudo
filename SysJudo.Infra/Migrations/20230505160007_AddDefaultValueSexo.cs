using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysJudo.Infra.Migrations
{
    public partial class AddDefaultValueSexo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .InsertData("Sexos", new[]
                {
                    "Id", "Sigla", "Descricao",
                }, new object[,]
                {
                    { 1, "M", "Masculino" },
                    { 2, "F", "Feminino" },
                    { 3, "O", "Outro" },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .DeleteData("PreparacaoTipos", "Id", new[] { 1, 2, 3 });
        }
    }
}
