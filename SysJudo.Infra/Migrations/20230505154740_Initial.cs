using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysJudo.Infra.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Administradores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradores", x => x.Id);
                });

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
                name: "Permissoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sexos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sexos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sistemas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Versao = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sistemas", x => x.Id);
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
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PastaArquivo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdSistema = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clientes_Sistemas_IdSistema",
                        column: x => x.IdSistema,
                        principalTable: "Sistemas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmissoresIdentidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmissoresIdentidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmissoresIdentidades_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EstadosCivis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosCivis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstadosCivis_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Faixas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    MesesCarencia = table.Column<int>(type: "int", nullable: false),
                    IdadeMinima = table.Column<int>(type: "int", nullable: false),
                    OrdemExibicao = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faixas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Faixas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GruposAcesso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Administrador = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Desativado = table.Column<bool>(type: "bit", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GruposAcesso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GruposAcesso_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Nacionalidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nacionalidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nacionalidades_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Profissoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profissoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profissoes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Regioes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Responsavel = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Cep = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Complemento = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    ANOTACOES = table.Column<string>(type: "text", nullable: true),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regioes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Regioes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimoLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataExpiracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Senha = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Inadiplente = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GrupoAcessoPermissao",
                columns: table => new
                {
                    GrupoAcessoId = table.Column<int>(type: "int", nullable: false),
                    PermissaoId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CriadoPor = table.Column<int>(type: "int", nullable: true),
                    CriadoPorAdmin = table.Column<bool>(type: "bit", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoPor = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoAcessoPermissao", x => new { x.GrupoAcessoId, x.PermissaoId });
                    table.ForeignKey(
                        name: "FK_GrupoAcessoPermissao_GruposAcesso_GrupoAcessoId",
                        column: x => x.GrupoAcessoId,
                        principalTable: "GruposAcesso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GrupoAcessoPermissao_Permissoes_PermissaoId",
                        column: x => x.PermissaoId,
                        principalTable: "Permissoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Agremiacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Fantasia = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Responsavel = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Representante = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Conteudo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    DocumentosUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataFiliacao = table.Column<DateTime>(type: "DATE", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "DATE", nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Complemento = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    InscricaoMunicipal = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    InscricaoEstadual = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DataCnpj = table.Column<DateTime>(type: "DATE", nullable: true),
                    DataAta = table.Column<DateTime>(type: "DATE", nullable: true),
                    Foto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlvaraLocacao = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Estatuto = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ContratoSocial = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DocumentacaoAtualizada = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Anotacoes = table.Column<string>(type: "nvarchar(1200)", maxLength: 1200, nullable: true),
                    Pais = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IdRegiao = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agremiacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agremiacoes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Agremiacoes_Regioes_IdRegiao",
                        column: x => x.IdRegiao,
                        principalTable: "Regioes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegistroDeEventos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataHoraEvento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ComputadorId = table.Column<int>(type: "int", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    TipoOperacaoId = table.Column<int>(type: "int", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    AdministradorId = table.Column<int>(type: "int", nullable: true),
                    FuncaoMenuId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroDeEventos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistroDeEventos_Administradores_AdministradorId",
                        column: x => x.AdministradorId,
                        principalTable: "Administradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateTable(
                name: "Atletas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Foto = table.Column<string>(type: "nchar(255)", fixedLength: true, maxLength: 255, nullable: true),
                    RegistroFederacao = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RegistroConfederacao = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFiliacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Complemento = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Cpf = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Identidade = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DataIdentidade = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NomePai = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    NomeMae = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Anotacoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdFaixa = table.Column<int>(type: "int", nullable: false),
                    IdSexo = table.Column<int>(type: "int", nullable: false),
                    IdEstadoCivil = table.Column<int>(type: "int", nullable: false),
                    IdProfissaoAtleta = table.Column<int>(type: "int", nullable: false),
                    IdProfissaoMae = table.Column<int>(type: "int", nullable: true),
                    IdProfissaoPai = table.Column<int>(type: "int", nullable: true),
                    IdEmissor = table.Column<int>(type: "int", nullable: false),
                    IdNacionalidade = table.Column<int>(type: "int", nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IdAgremiacao = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atletas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Atletas_Agremiacoes_IdAgremiacao",
                        column: x => x.IdAgremiacao,
                        principalTable: "Agremiacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atletas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atletas_EmissoresIdentidades_IdEmissor",
                        column: x => x.IdEmissor,
                        principalTable: "EmissoresIdentidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atletas_EstadosCivis_IdEstadoCivil",
                        column: x => x.IdEstadoCivil,
                        principalTable: "EstadosCivis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atletas_Faixas_IdFaixa",
                        column: x => x.IdFaixa,
                        principalTable: "Faixas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atletas_Nacionalidades_IdNacionalidade",
                        column: x => x.IdNacionalidade,
                        principalTable: "Nacionalidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atletas_Profissoes_IdProfissaoAtleta",
                        column: x => x.IdProfissaoAtleta,
                        principalTable: "Profissoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atletas_Sexos_IdSexo",
                        column: x => x.IdSexo,
                        principalTable: "Sexos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgremiacoesFiltro",
                columns: table => new
                {
                    Identificador = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Fantasia = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Responsavel = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Representante = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Conteudo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    DataFiliacao = table.Column<DateTime>(type: "DATE", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "DATE", nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Complemento = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    InscricaoMunicipal = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    InscricaoEstadual = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DataCnpj = table.Column<DateTime>(type: "DATE", nullable: true),
                    DataAta = table.Column<DateTime>(type: "DATE", nullable: true),
                    Foto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlvaraLocacao = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Estatuto = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ContratoSocial = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DocumentacaoAtualizada = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Anotacoes = table.Column<string>(type: "nvarchar(1200)", maxLength: 1200, nullable: true),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegiaoNome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdRegiao = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    AtletaId = table.Column<int>(type: "int", nullable: true),
                    RegiaoId = table.Column<int>(type: "int", nullable: true),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgremiacoesFiltro", x => x.Identificador);
                    table.ForeignKey(
                        name: "FK_AgremiacoesFiltro_Atletas_AtletaId",
                        column: x => x.AtletaId,
                        principalTable: "Atletas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AgremiacoesFiltro_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgremiacoesFiltro_Regioes_RegiaoId",
                        column: x => x.RegiaoId,
                        principalTable: "Regioes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agremiacoes_ClienteId",
                table: "Agremiacoes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Agremiacoes_IdRegiao",
                table: "Agremiacoes",
                column: "IdRegiao");

            migrationBuilder.CreateIndex(
                name: "IX_AgremiacoesFiltro_AtletaId",
                table: "AgremiacoesFiltro",
                column: "AtletaId");

            migrationBuilder.CreateIndex(
                name: "IX_AgremiacoesFiltro_ClienteId",
                table: "AgremiacoesFiltro",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_AgremiacoesFiltro_RegiaoId",
                table: "AgremiacoesFiltro",
                column: "RegiaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_ClienteId",
                table: "Atletas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_IdAgremiacao",
                table: "Atletas",
                column: "IdAgremiacao");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_IdEmissor",
                table: "Atletas",
                column: "IdEmissor");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_IdEstadoCivil",
                table: "Atletas",
                column: "IdEstadoCivil");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_IdFaixa",
                table: "Atletas",
                column: "IdFaixa");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_IdNacionalidade",
                table: "Atletas",
                column: "IdNacionalidade");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_IdProfissaoAtleta",
                table: "Atletas",
                column: "IdProfissaoAtleta");

            migrationBuilder.CreateIndex(
                name: "IX_Atletas_IdSexo",
                table: "Atletas",
                column: "IdSexo");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_IdSistema",
                table: "Clientes",
                column: "IdSistema");

            migrationBuilder.CreateIndex(
                name: "IX_EmissoresIdentidades_ClienteId",
                table: "EmissoresIdentidades",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_EstadosCivis_ClienteId",
                table: "EstadosCivis",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Faixas_ClienteId",
                table: "Faixas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoAcessoPermissao_PermissaoId",
                table: "GrupoAcessoPermissao",
                column: "PermissaoId");

            migrationBuilder.CreateIndex(
                name: "IX_GruposAcesso_ClienteId",
                table: "GruposAcesso",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Nacionalidades_ClienteId",
                table: "Nacionalidades",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Profissoes_ClienteId",
                table: "Profissoes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Regioes_ClienteId",
                table: "Regioes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroDeEventos_AdministradorId",
                table: "RegistroDeEventos",
                column: "AdministradorId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_ClienteId",
                table: "Usuarios",
                column: "ClienteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgremiacoesFiltro");

            migrationBuilder.DropTable(
                name: "GrupoAcessoPermissao");

            migrationBuilder.DropTable(
                name: "RegistroDeEventos");

            migrationBuilder.DropTable(
                name: "Atletas");

            migrationBuilder.DropTable(
                name: "GruposAcesso");

            migrationBuilder.DropTable(
                name: "Permissoes");

            migrationBuilder.DropTable(
                name: "Administradores");

            migrationBuilder.DropTable(
                name: "FuncoesMenus");

            migrationBuilder.DropTable(
                name: "TiposOperacoes");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Agremiacoes");

            migrationBuilder.DropTable(
                name: "EmissoresIdentidades");

            migrationBuilder.DropTable(
                name: "EstadosCivis");

            migrationBuilder.DropTable(
                name: "Faixas");

            migrationBuilder.DropTable(
                name: "Nacionalidades");

            migrationBuilder.DropTable(
                name: "Profissoes");

            migrationBuilder.DropTable(
                name: "Sexos");

            migrationBuilder.DropTable(
                name: "Regioes");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Sistemas");
        }
    }
}
