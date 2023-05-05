using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysJudo.Infra.Migrations
{
    public partial class AddDefaultValueFuncoesMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .InsertData("FuncoesMenus", new[]
                {
                    "Id", "Sigla", "Descricao"
                }, new object[,]
                {
                    {1 ,"CADATLE"      , "Cadastro de Atletas"},
                    {2 ,"CADAGRE"      , "Cadastro de Agremiações"},
                    {3 ,"CADARBI"      , "Cadastro de Árbitros"},
                    {4 ,"CADPROF"      , "Cadastro de Professores"},
                    {5 ,"ASSPROAGRE"   , "Associação Professores x Agremiações"},
                    {6 ,"CADAPOI"      , "Cadastro de Pessoal de Apoio"},
                    {7 ,"ASSAPOFUNC"   , "Associação Pessoal Apoio x Funções"},
                    {8 ,"TABREGI"      , "Tabela de Regiões"},
                    {9 ,"TABLOCA"      , "Tabela de Locais de Competições"},
                    {10 ,"TABPATR"      , "Tabela de Patrocinadores"},
                    {11 ,"TABATIV"      , "Tabela de Profissões"},
                    {12 ,"TABEMIS"      , "Tabela de Emissores de Identidades"},
                    {13 ,"TABCIDA"      , "Tabela de Cidades"},
                    {14,"TABESTA"      , "Tabela de Estados"},
                    {15 ,"TABPAIS"      , "Tabela de Países"},
                    {16 ,"TABFAIX"      , "Tabela de Faixas"},
                    {17 ,"TABCATE"      , "Tabela de Categorias"},
                    {18 ,"TABCLAS"      , "Tabela de Classes"},
                    {19 ,"TABPESO"      , "Tabela de Pesos"},
                    {20 ,"TABPENA"      , "Tabela de Penalidades"},
                    {21 ,"TABPONT"      , "Tabela de Pontuações"},
                    {22 ,"TABNIVE"      , "Tabela de Níveis de Árbitros"},
                    {23 ,"TABFUNC"      , "Tabela de Funções de Apoio"},
                    {24 ,"TABTDESFFAT"  , "Tabela de Tipos de Desfiliações de Atletas"},
                    {25 ,"TABTDESFFAG"  , "Tabela de Tipos de Desfiliações de Agremiações"},
                    {26 ,"TABTISEAT"    , "Tabela de Tipos de Isenções de Atletas"},
                    {27 ,"TABTISEAG"    , "Tabela de Tipos de Isenções de Agremiações"},
                    {28 ,"TABTCURAT"    , "Tabela de Tipos de Currículos de Atletas"},
                    {29 ,"TABTCURAG"    , "Tabela de Tipos de Currículos de Agremiações"},
                    {30 ,"TABTPROMAT"   , "Tabela de Tipos de Promoções de Atletas"},
                    {31 ,"TABTPROMAR"   , "Tabela de Tipos de Promoções de Árbitros"},
                    {32 ,"TABTTRA"      , "Tabela de Tipos de Transferências"},
                    {33 ,"TABTANUI"     , "Tabela de Tipos de Anuidades"},
                    {34 ,"TABTPAGAT"    , "Tabela de Tipos de Pagamentos de Atletas"},
                    {35 ,"TABTPAGAG"    , "Tabela de Tipos de Pagamentos de Agremiações"},
                    {36 ,"TABITREC"     , "Tabela de Itens de Recibos"},
                    {37 ,"TABFPAG"      , "Tabela de Formas de Pagamentos"},
                    {38 ,"TABANUI"      , "Tabelas de Anuidades"},
                    {39 ,"TABMENS"      , "Tabelas de Mensalidades"},
                    {40 ,"TABSERV"      , "Tabelas de Serviços"},
                    {41 ,"VALDEFA"      , "Valores Default"},
                    {42 ,"REGPAGANUI"   , "Registro de Pagamentos de Anuidades"},
                    {43 ,"REGPAGMENS"   , "Registro de Pagamentos de Mensalidades"},
                    {44 ,"REGMULTMENS"  , "Registro de Pagamentos de Múltiplas Mensalidades"},
                    {45 ,"EMICOBR"      , "Emissão de Cobranças de Agremiações"},
                    {46 ,"RECISERV"     , "Emissão de Recibos de Serviços"},
                    {47 ,"RECIGENE"     , "Emissão de Recibos de Genéricos"},
                    {48 ,"ISEATLE"      , "Registro de Isenção de Atletas"},
                    {49 ,"ISEAGRE"      , "Registro de Isenção de Agremiações"},
                    {50 ,"RELANUI"      , "Relatório de Anuidades"},
                    {51 ,"RELMENS"      , "Relatório de Mensalidades"},
                    {52 ,"RELRECI"      , "Relatório de Recibos Emitidos"},
                    {53 ,"REGPROAT"     , "Registros de Promoções de Atletas"},
                    {54 ,"REGPROAR"     , "Registros de Promoções de Árbitros"},
                    {55 ,"REGTRAN"      , "Registros de Transferências de Atletas"},
                    {56 ,"EMICART"      , "Emissão de Carteiras de Atletas"},
                    {57 ,"REGCURAT"     , "Registro em Currículo de Atleta"},
                    {58 ,"REGCURAG"     , "Registro em Currículo de Agremiação"},
                    {59 ,"REGDESFAT"    , "Registro de Desfiliação de Atleta"},
                    {60 ,"REGDESFAG"    , "Registro de Desfiliação de Agremiação"},
                    {61 ,"RELPROMAT"    , "Relatório de Promoções de Atletas"},
                    {62 ,"RELPROMAG"    , "Relatório de Promoções de Árbitros"},
                    {63 ,"RELTRANAT"    , "Relatório de Transferências de Atletas"},
                    {64 ,"RELHISTAT"    , "Relatório de Histórico de Atletas"},
                    {65 ,"RELCERTAT"    , "Relatório de Certificados de Atletas"},
                    {66 ,"CADCOMP"      , "Cadastro de Competições"},
                    {67 ,"DEFCOMP"      , "Definir Competição Ativa"},
                    {68 ,"TABFAIXCOMP"  , "Tabela de Faixas de Competição"},
                    {69 ,"TABCATECOMP"  , "Tabela de Categorias de Competição"},
                    {70 ,"TABCLASCOMP"  , "Tabela de Classes de Competição"},
                    {71 ,"TABPESOCOMP"  , "Tabela de Peso de Competição"},
                    {72 ,"TABPENACOMP"  , "Tabela de Penalidades de Competição"},
                    {73 ,"TABPONTCOMP"  , "Tabela de Pontuações de Competição"},
                    {74 ,"TABAREACOMP"  , "Tabela de Áreas de Competição"},
                    {75 ,"TABCONFCLAS"  , "Tabela de Confronto entre Classes"},
                    {76 ,"DEFCHAVES"    , "Definição de Chaves de Competição"},
                    {77 ,"ALOCAPOICOMP" , "Alocação de Pessoal de Apoio em Competição"},
                    {78 ,"ALOCARBICOMP" , "Alocação de Árbitros em Competição"},
                    {79 ,"ALOCPATRCOMP" , "Alocação de Patrocinadores em Competição"},
                    {80 ,"REGINSCCOMP"  , "Registro de Inscrições de Atletas em Competição"},
                    {81 ,"SORTEIO"      , "Sorteio de Atletas nas Chaves de Competição"},
                    {82 ,"RELCHAVES"    , "Relatório de Chaves de Competição"} ,    
                    {83 ,"REGLUTAS"     , "Registro de Lutas de Competição"},
                    {84 ,"REGRESU"      , "Registro de Resultados de Competição"},
                    {85 ,"REGFALTAAT"   , "Registro de Faltas de Atletas em Competição"},
                    {86 ,"REGFALTAAR"   , "Registro de Faltas de Árbitros em Competição"},
                    {87 ,"REGFALTAAP"   , "Registro de Faltas de Pessoal de Apoio em Competição"},
                    {88 ,"RELATLECHAV"  , "Relatório de Atletas x Chaves de Competições"},
                    {89 ,"RELATLEAGRE"  , "Relatório de Atletas x Agremiações de Competições"},
                    {90 ,"RELAGRECLAS"  , "Relatório de Agremiações x Classes de Competições"},
                    {91 ,"CADUSU"       , "Cadastro de Usuários"},
                    {92 ,"CADPERF"      , "Cadastro de Perfís"},
                    {93 ,"ASSPERFUSU"   , "Associação Perfil x Usuário"},
                    {94 ,"CADFUNMENU"   , "Cadastro de Funções do Menu"},
                    {95 ,"CADTOPER"     , "Cadastro de Tipos de Operações"},
                    {96 ,"DEFPERM"      , "Definição de Permissões"},
                    {97 ,"VISREGEVEN"   , "Visualizar Registros de Eventos do Sistema"},
                    {98 ,"CADSIST"      , "Cadastro de Sistemas"},
                    {99 ,"CADCLI"       , "Cadastro de Clientes"},
                    {100 ,"TABTIPMSGSIS" , "Tabela de Tipos de Mensagens dos Sistemas"},
                    {101 ,"CADMSGSIST"   , "Cadastro de Mensagens dos Sistemas"},});
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("FuncoesMenus", "Id", Enumerable.Range(1, 101).ToArray());        
        }
    }
}
