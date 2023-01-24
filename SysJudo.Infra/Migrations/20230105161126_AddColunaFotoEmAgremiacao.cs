using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysJudo.Infra.Migrations
{
    public partial class AddColunaFotoEmAgremiacao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Foto",
                table: "Atletas",
                type: "nchar(255)",
                fixedLength: true,
                maxLength: 255,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AlvaraLocacao",
                table: "Agremiacoes",
                type: "nchar(255)",
                fixedLength: true,
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(1)",
                oldFixedLength: true,
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Foto",
                table: "Agremiacoes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Foto",
                table: "Atletas");

            migrationBuilder.DropColumn(
                name: "Foto",
                table: "Agremiacoes");

            migrationBuilder.AlterColumn<string>(
                name: "AlvaraLocacao",
                table: "Agremiacoes",
                type: "nchar(1)",
                fixedLength: true,
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(255)",
                oldFixedLength: true,
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
