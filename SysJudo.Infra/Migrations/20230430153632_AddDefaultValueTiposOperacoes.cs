using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysJudo.Infra.Migrations
{
    public partial class AddDefaultValueTiposOperacoes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .InsertData("TiposOperacoes", new[]
                {
                    "Id", "Sigla", "Descricao"
                }, new object[,]
                {
                    {1 ,"ENTRADA"      , "Entrada no sistema"},
                    {2 ,"SAIDA"      , "Saída do sistema"},
                    {3 ,"ACESSAR"      , "Acessar função do menu"},
                    {4 ,"INCLUIR"      , "Incluir registro"},
                    {5 ,"ALTERAR"   , "Alterar registro"},
                    {6 ,"EXCLUIR"      , "Excluir registro"},
                    {7 ,"CONSULTAR"   , "Consultar registro"},
                    {8 ,"ANEXAR"      , "Anexar/Desanexar arquivo"},
                    {9 ,"VERANEXO"      , "Visualizar anexo"},
                    {10 ,"EXPORTAR"      , "Exportar dados para planilha"},
                    {11 ,"ANOTAR"      , "Anotar dados no registro"},
                    {12 ,"MARCAR"      , "Marcar/Desmarcar registros"},
                    {13 ,"FILTRAR"      , "Filtrar registros"},
                    {14,"PESQUISAR"      , "Pesquisa registros"},
                    {15,"TODAS"      , "Todas as operações"},
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("TiposOperacoes", "Id", Enumerable.Range(1, 14).ToArray());
        }
    }
}
