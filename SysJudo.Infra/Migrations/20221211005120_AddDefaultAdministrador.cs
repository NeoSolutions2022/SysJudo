using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysJudo.Infra.Migrations
{
    public partial class AddDefaultAdministrador : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var senha =
                "$argon2id$v=19$m=32768,t=4,p=1$8kSN61J8u9f2fBanH2sbjA$mcjis6H1GOwjNVVNBznVkOkktsa+CHUc9bP95x8IsEo";
            
            migrationBuilder.InsertData(
                table: "Administradores",
                columns: new[] { "Id", "Nome", "Email", "Senha", "Selecao"},
                values: new object[,]
                {
                    { 1, "Admin", "admin@admin.com", senha, false  }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .DeleteData("Administradores", "Id", 1);
        }
    }
}
