using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysJudo.Infra.Migrations
{
    public partial class RemovendoPaisEstadoCidade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropForeignKey(
                name: "FK_AgremiacoesFiltro_Cidades_CidadeId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropForeignKey(
                name: "FK_AgremiacoesFiltro_Estados_EstadoId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropForeignKey(
                name: "FK_AgremiacoesFiltro_Paises_PaisId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropForeignKey(
                name: "FK_Atletas_Cidades_IdCidade",
                table: "Atletas");

            migrationBuilder.DropForeignKey(
                name: "FK_Atletas_Estados_IdEstado",
                table: "Atletas");

            migrationBuilder.DropForeignKey(
                name: "FK_Atletas_Paises_IdPais",
                table: "Atletas");

            migrationBuilder.DropForeignKey(
                name: "FK_Regioes_Cidades_IdCidade",
                table: "Regioes");

            migrationBuilder.DropForeignKey(
                name: "FK_Regioes_Estados_IdEstado",
                table: "Regioes");

            migrationBuilder.DropForeignKey(
                name: "FK_Regioes_Paises_IdPais",
                table: "Regioes");

            migrationBuilder.DropTable(
                name: "Cidades");

            migrationBuilder.DropTable(
                name: "Estados");

            migrationBuilder.DropTable(
                name: "Paises");

            migrationBuilder.DropIndex(
                name: "IX_Regioes_IdCidade",
                table: "Regioes");

            migrationBuilder.DropIndex(
                name: "IX_Regioes_IdEstado",
                table: "Regioes");

            migrationBuilder.DropIndex(
                name: "IX_Regioes_IdPais",
                table: "Regioes");

            migrationBuilder.DropIndex(
                name: "IX_Atletas_IdCidade",
                table: "Atletas");

            migrationBuilder.DropIndex(
                name: "IX_Atletas_IdEstado",
                table: "Atletas");

            migrationBuilder.DropIndex(
                name: "IX_Atletas_IdPais",
                table: "Atletas");

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
                name: "IX_Agremiacoes_CidadeId",
                table: "Agremiacoes");

            migrationBuilder.DropIndex(
                name: "IX_Agremiacoes_EstadoId",
                table: "Agremiacoes");

            migrationBuilder.DropIndex(
                name: "IX_Agremiacoes_PaisId",
                table: "Agremiacoes");

            migrationBuilder.DropColumn(
                name: "IdCidade",
                table: "Regioes");

            migrationBuilder.DropColumn(
                name: "IdEstado",
                table: "Regioes");

            migrationBuilder.DropColumn(
                name: "IdPais",
                table: "Regioes");

            migrationBuilder.DropColumn(
                name: "IdCidade",
                table: "Atletas");

            migrationBuilder.DropColumn(
                name: "IdEstado",
                table: "Atletas");

            migrationBuilder.DropColumn(
                name: "IdPais",
                table: "Atletas");

            migrationBuilder.DropColumn(
                name: "CidadeId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropColumn(
                name: "EstadoId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropColumn(
                name: "IdCidade",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropColumn(
                name: "IdEstado",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropColumn(
                name: "IdPais",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropColumn(
                name: "PaisId",
                table: "AgremiacoesFiltro");

            migrationBuilder.DropColumn(
                name: "CidadeId",
                table: "Agremiacoes");

            migrationBuilder.DropColumn(
                name: "EstadoId",
                table: "Agremiacoes");

            migrationBuilder.DropColumn(
                name: "IdCidade",
                table: "Agremiacoes");

            migrationBuilder.DropColumn(
                name: "IdEstado",
                table: "Agremiacoes");

            migrationBuilder.DropColumn(
                name: "IdPais",
                table: "Agremiacoes");

            migrationBuilder.DropColumn(
                name: "PaisId",
                table: "Agremiacoes");

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "Regioes",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Regioes",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Pais",
                table: "Regioes",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "Atletas",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Atletas",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Pais",
                table: "Atletas",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Pais",
                table: "Agremiacoes",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Agremiacoes",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Cidade",
                table: "Agremiacoes",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "Regioes");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Regioes");

            migrationBuilder.DropColumn(
                name: "Pais",
                table: "Regioes");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "Atletas");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Atletas");

            migrationBuilder.DropColumn(
                name: "Pais",
                table: "Atletas");

            migrationBuilder.AddColumn<int>(
                name: "IdCidade",
                table: "Regioes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdEstado",
                table: "Regioes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdPais",
                table: "Regioes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdCidade",
                table: "Atletas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdEstado",
                table: "Atletas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdPais",
                table: "Atletas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CidadeId",
                table: "AgremiacoesFiltro",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstadoId",
                table: "AgremiacoesFiltro",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdCidade",
                table: "AgremiacoesFiltro",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdEstado",
                table: "AgremiacoesFiltro",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdPais",
                table: "AgremiacoesFiltro",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaisId",
                table: "AgremiacoesFiltro",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Pais",
                table: "Agremiacoes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Agremiacoes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Cidade",
                table: "Agremiacoes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AddColumn<int>(
                name: "CidadeId",
                table: "Agremiacoes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstadoId",
                table: "Agremiacoes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdCidade",
                table: "Agremiacoes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdEstado",
                table: "Agremiacoes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdPais",
                table: "Agremiacoes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaisId",
                table: "Agremiacoes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Paises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: false),
                    Nacionalidade = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    Sigla2 = table.Column<string>(type: "varchar(2)", unicode: false, maxLength: 2, nullable: false),
                    Sigla3 = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Paises_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Estados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    IdPais = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: false),
                    Sigla = table.Column<string>(type: "varchar(2)", unicode: false, maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estados_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Estados_Paises_IdPais",
                        column: x => x.IdPais,
                        principalTable: "Paises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    IdEstado = table.Column<int>(type: "int", nullable: false),
                    IdPais = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: false),
                    Sigla = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cidades_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cidades_Estados_IdEstado",
                        column: x => x.IdEstado,
                        principalTable: "Estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cidades_Paises_IdPais",
                        column: x => x.IdPais,
                        principalTable: "Paises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Regioes_IdCidade",
                table: "Regioes",
                column: "IdCidade");

            migrationBuilder.CreateIndex(
                name: "IX_Regioes_IdEstado",
                table: "Regioes",
                column: "IdEstado");

            migrationBuilder.CreateIndex(
                name: "IX_Regioes_IdPais",
                table: "Regioes",
                column: "IdPais");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_IdCidade",
                table: "Atletas",
                column: "IdCidade");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_IdEstado",
                table: "Atletas",
                column: "IdEstado");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_IdPais",
                table: "Atletas",
                column: "IdPais");

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

            migrationBuilder.CreateIndex(
                name: "IX_Cidades_ClienteId",
                table: "Cidades",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Cidades_IdEstado",
                table: "Cidades",
                column: "IdEstado");

            migrationBuilder.CreateIndex(
                name: "IX_Cidades_IdPais",
                table: "Cidades",
                column: "IdPais");

            migrationBuilder.CreateIndex(
                name: "IX_Estados_ClienteId",
                table: "Estados",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Estados_IdPais",
                table: "Estados",
                column: "IdPais");

            migrationBuilder.CreateIndex(
                name: "IX_Paises_ClienteId",
                table: "Paises",
                column: "ClienteId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_AgremiacoesFiltro_Cidades_CidadeId",
                table: "AgremiacoesFiltro",
                column: "CidadeId",
                principalTable: "Cidades",
                principalColumn: "Id");

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
                name: "FK_Atletas_Cidades_IdCidade",
                table: "Atletas",
                column: "IdCidade",
                principalTable: "Cidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Atletas_Estados_IdEstado",
                table: "Atletas",
                column: "IdEstado",
                principalTable: "Estados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Atletas_Paises_IdPais",
                table: "Atletas",
                column: "IdPais",
                principalTable: "Paises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Regioes_Cidades_IdCidade",
                table: "Regioes",
                column: "IdCidade",
                principalTable: "Cidades",
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
    }
}
