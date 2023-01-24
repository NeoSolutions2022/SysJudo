using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysJudo.Infra.Migrations
{
    public partial class AjusteTamanhoDePropriedadeEmAgremiacao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Estatuto",
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

            migrationBuilder.AlterColumn<string>(
                name: "DocumentacaoAtualizada",
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

            migrationBuilder.AlterColumn<string>(
                name: "ContratoSocial",
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Estatuto",
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

            migrationBuilder.AlterColumn<string>(
                name: "DocumentacaoAtualizada",
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

            migrationBuilder.AlterColumn<string>(
                name: "ContratoSocial",
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
