using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysJudo.Infra.Migrations
{
    public partial class AddRegistroDeEventos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Complemento",
                table: "Agremiacoes",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            migrationBuilder.CreateTable(
                name: "FuncoesMenus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuncoesMenus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposOperacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposOperacoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistroDeEventos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataHoraEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ComputadorId = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    TipoOperacaoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    FuncaoMenuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroDeEventos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistroDeEventos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistroDeEventos_FuncoesMenus_FuncaoMenuId",
                        column: x => x.FuncaoMenuId,
                        principalTable: "FuncoesMenus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistroDeEventos_TiposOperacoes_TipoOperacaoId",
                        column: x => x.TipoOperacaoId,
                        principalTable: "TiposOperacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistroDeEventos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistroDeEventos_ClienteId",
                table: "RegistroDeEventos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroDeEventos_FuncaoMenuId",
                table: "RegistroDeEventos",
                column: "FuncaoMenuId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroDeEventos_TipoOperacaoId",
                table: "RegistroDeEventos",
                column: "TipoOperacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroDeEventos_UsuarioId",
                table: "RegistroDeEventos",
                column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistroDeEventos");

            migrationBuilder.DropTable(
                name: "FuncoesMenus");

            migrationBuilder.DropTable(
                name: "TiposOperacoes");

            migrationBuilder.AlterColumn<string>(
                name: "Complemento",
                table: "Agremiacoes",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60,
                oldNullable: true);
        }
    }
}
