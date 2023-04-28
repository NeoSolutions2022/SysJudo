using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysJudo.Infra.Migrations
{
    public partial class AddDefaultValueTipoOperacao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .InsertData("TiposOperacoes", new[]
                {
                    "Id", "Sigla", "Descricao", "ClienteId",
                }, new object[,]
                {
                    {1 ,"ENTRADA"      , "Entrada no sistema"                                  , 1 },
                    {2 ,"SAIDA"      , "Saída do sistema"                                             , 1 },
                    {3 ,"ACESSAR"      , "Acessar função do menu"                                 , 1 },
                    {4 ,"INCLUIR"      , "Incluir registro"                              , 1 },
                    {5 ,"ALTERAR"   , "Alterar registro"                 , 1 },
                    {6 ,"EXCLUIR"      , "Excluir registro"                         , 1 },
                    {7 ,"CONSULTAR"   , "Consultar registro"                   , 1 },
                    {8 ,"ANEXAR"      , "Anexar/Desanexar arquivo"                                    , 1 },
                    {9 ,"VERANEXO"      , "Visualizar anexo"                      , 1 },
                    {10 ,"EXPORTAR"      , "Exportar dados para planilha"                             , 1 },
                    {11 ,"ANOTAR"      , "Anotar dados no registro"                                 , 1 },
                    {12 ,"MARCAR"      , "Marcar/Desmarcar registros"                   , 1 },
                    {13 ,"FILTRAR"      , "Filtrar registros"		                            , 1 },
                    {14,"PESQUISAR"      , "Pesquisa registros"		                            , 1 },
                    {15,"TODAS"      , "Todas as operações"		                            , 1 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("TiposOperacoes", "Id", Enumerable.Range(1, 14).ToArray());
        }
    }
}
