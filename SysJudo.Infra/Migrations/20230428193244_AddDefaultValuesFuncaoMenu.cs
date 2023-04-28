﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysJudo.Infra.Migrations
{
    public partial class AddDefaultValuesFuncaoMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .InsertData("FuncoesMenus", new[]
                {
                    "Id", "Sigla", "Descricao", "ClienteId",
                }, new object[,]
                {
                    {1 ,"CADATLE"      , "Cadastro de Atletas"                                  , 1 },
                    {2 ,"CADAGRE"      , "Cadastro de Agremiações"                              , 1 },
                    {3 ,"CADARBI"      , "Cadastro de Árbitros"                                 , 1 },
                    {4 ,"CADPROF"      , "Cadastro de Professores"                              , 1 },
                    {5 ,"ASSPROAGRE"   , "Associação Professores x Agremiações"                 , 1 },
                    {6 ,"CADAPOI"      , "Cadastro de Pessoal de Apoio"                         , 1 },
                    {7 ,"ASSAPOFUNC"   , "Associação Pessoal Apoio x Funções"                   , 1 },
                    {8 ,"TABREGI"      , "Tabela de Regiões"                                    , 1 },
                    {9 ,"TABLOCA"      , "Tabela de Locais de Competições"                      , 1 },
                    {10 ,"TABPATR"      , "Tabela de Patrocinadores"                             , 1 },
                    {11 ,"TABATIV"      , "Tabela de Profissões"                                 , 1 },
                    {12 ,"TABEMIS"      , "Tabela de Emissores de Identidades"                   , 1 },
                    {13 ,"TABCIDA"      , "Tabela de Cidades"		                            , 1 },
                    {14,"TABESTA"      , "Tabela de Estados"		                            , 1 },
                    {15 ,"TABPAIS"      , "Tabela de Países"                                     , 1 },
                    {16 ,"TABFAIX"      , "Tabela de Faixas"				                        , 1 },
                    {17 ,"TABCATE"      , "Tabela de Categorias"                                 , 1 },
                    {18 ,"TABCLAS"      , "Tabela de Classes"                                    , 1 },
                    {19 ,"TABPESO"      , "Tabela de Pesos"                                      , 1 },
                    {20 ,"TABPENA"      , "Tabela de Penalidades"                                , 1 },
                    {21 ,"TABPONT"      , "Tabela de Pontuações"                                 , 1 },
                    {22 ,"TABNIVE"      , "Tabela de Níveis de Árbitros"                         , 1 },
                    {23 ,"TABFUNC"      , "Tabela de Funções de Apoio"                           , 1 },
                    {24 ,"TABTDESFFAT"  , "Tabela de Tipos de Desfiliações de Atletas"           , 1 },
                    {25 ,"TABTDESFFAG"  , "Tabela de Tipos de Desfiliações de Agremiações"       , 1 },
                    {26 ,"TABTISEAT"    , "Tabela de Tipos de Isenções de Atletas"               , 1 },
                    {27 ,"TABTISEAG"    , "Tabela de Tipos de Isenções de Agremiações"           , 1 },
                    {28 ,"TABTCURAT"    , "Tabela de Tipos de Currículos de Atletas"             , 1 },
                    {29 ,"TABTCURAG"    , "Tabela de Tipos de Currículos de Agremiações"         , 1 },
                    {30 ,"TABTPROMAT"   , "Tabela de Tipos de Promoções de Atletas"              , 1 },
                    {31 ,"TABTPROMAR"   , "Tabela de Tipos de Promoções de Árbitros"             , 1 },
                    {32 ,"TABTTRA"      , "Tabela de Tipos de Transferências"                    , 1 },
                    {33 ,"TABTANUI"     , "Tabela de Tipos de Anuidades"                         , 1 },
                    {34 ,"TABTPAGAT"    , "Tabela de Tipos de Pagamentos de Atletas"             , 1 },
                    {35 ,"TABTPAGAG"    , "Tabela de Tipos de Pagamentos de Agremiações"         , 1 },
                    {36 ,"TABITREC"     , "Tabela de Itens de Recibos"                           , 1 },
                    {37 ,"TABFPAG"      , "Tabela de Formas de Pagamentos"                       , 1 },
                    {38 ,"TABANUI"      , "Tabelas de Anuidades"                                 , 1 },
                    {39 ,"TABMENS"      , "Tabelas de Mensalidades"                              , 1 },
                    {40 ,"TABSERV"      , "Tabelas de Serviços"                                  , 1 },
                    {41 ,"VALDEFA"      , "Valores Default"                                      , 1 },
                    {42 ,"REGPAGANUI"   , "Registro de Pagamentos de Anuidades"                  , 1 },
                    {43 ,"REGPAGMENS"   , "Registro de Pagamentos de Mensalidades"               , 1 },
                    {44 ,"REGMULTMENS"  , "Registro de Pagamentos de Múltiplas Mensalidades"     , 1 },
                    {45 ,"EMICOBR"      , "Emissão de Cobranças de Agremiações"                  , 1 },
                    {46 ,"RECISERV"     , "Emissão de Recibos de Serviços"                       , 1 },
                    {47 ,"RECIGENE"     , "Emissão de Recibos de Genéricos"                      , 1 },
                    {48 ,"ISEATLE"      , "Registro de Isenção de Atletas"                       , 1 },
                    {49 ,"ISEAGRE"      , "Registro de Isenção de Agremiações"                   , 1 },
                    {50 ,"RELANUI"      , "Relatório de Anuidades"                               , 1 },
                    {51 ,"RELMENS"      , "Relatório de Mensalidades"                            , 1 },
                    {52 ,"RELRECI"      , "Relatório de Recibos Emitidos"                        , 1 },
                    {53 ,"REGPROAT"     , "Registros de Promoções de Atletas"                    , 1 },
                    {54 ,"REGPROAR"     , "Registros de Promoções de Árbitros"                   , 1 },
                    {55 ,"REGTRAN"      , "Registros de Transferências de Atletas"               , 1 },
                    {56 ,"EMICART"      , "Emissão de Carteiras de Atletas"                      , 1 },
                    {57 ,"REGCURAT"     , "Registro em Currículo de Atleta"                      , 1 },
                    {58 ,"REGCURAG"     , "Registro em Currículo de Agremiação"                  , 1 },
                    {59 ,"REGDESFAT"    , "Registro de Desfiliação de Atleta"                    , 1 },
                    {60 ,"REGDESFAG"    , "Registro de Desfiliação de Agremiação"                , 1 },
                    {61 ,"RELPROMAT"    , "Relatório de Promoções de Atletas"                    , 1 },
                    {62 ,"RELPROMAG"    , "Relatório de Promoções de Árbitros"                   , 1 },
                    {63 ,"RELTRANAT"    , "Relatório de Transferências de Atletas"               , 1 },
                    {64 ,"RELHISTAT"    , "Relatório de Histórico de Atletas"                    , 1 },
                    {65 ,"RELCERTAT"    , "Relatório de Certificados de Atletas"                 , 1 },
                    {66 ,"CADCOMP"      , "Cadastro de Competições"			                    , 1 },
                    {67 ,"DEFCOMP"      , "Definir Competição Ativa"			                    , 1 },
                    {68 ,"TABFAIXCOMP"  , "Tabela de Faixas de Competição"		                , 1 },
                    {69 ,"TABCATECOMP"  , "Tabela de Categorias de Competição"		            , 1 },
                    {70 ,"TABCLASCOMP"  , "Tabela de Classes de Competição"		                , 1 },
                    {71 ,"TABPESOCOMP"  , "Tabela de Peso de Competição"	                        , 1 },
                    {72 ,"TABPENACOMP"  , "Tabela de Penalidades de Competição"   	            , 1 },
                    {73 ,"TABPONTCOMP"  , "Tabela de Pontuações de Competição"	                , 1 },
                    {74 ,"TABAREACOMP"  , "Tabela de Áreas de Competição"	                    , 1 },
                    {75 ,"TABCONFCLAS"  , "Tabela de Confronto entre Classes"	                , 1 },
                    {76 ,"DEFCHAVES"    , "Definição de Chaves de Competição"	                , 1 },
                    {77 ,"ALOCAPOICOMP" , "Alocação de Pessoal de Apoio em Competição"	        , 1 },
                    {78 ,"ALOCARBICOMP" , "Alocação de Árbitros em Competição"	                , 1 },
                    {79 ,"ALOCPATRCOMP" , "Alocação de Patrocinadores em Competição"	            , 1 },
                    {80 ,"REGINSCCOMP"  , "Registro de Inscrições de Atletas em Competição"      , 1 },
                    {81 ,"SORTEIO"      , "Sorteio de Atletas nas Chaves de Competição"          , 1 },
                    {82 ,"RELCHAVES"    , "Relatório de Chaves de Competição"                    , 1 } ,    
                    {83 ,"REGLUTAS"     , "Registro de Lutas de Competição"	                    , 1 },
                    {84 ,"REGRESU"      , "Registro de Resultados de Competição"	                , 1 },
                    {85 ,"REGFALTAAT"   , "Registro de Faltas de Atletas em Competição"	        , 1 },
                    {86 ,"REGFALTAAR"   , "Registro de Faltas de Árbitros em Competição"	        , 1 },
                    {87 ,"REGFALTAAP"   , "Registro de Faltas de Pessoal de Apoio em Competição"	, 1 },
                    {88 ,"RELATLECHAV"  , "Relatório de Atletas x Chaves de Competições"	        , 1 },
                    {89 ,"RELATLEAGRE"  , "Relatório de Atletas x Agremiações de Competições"	, 1 },
                    {90 ,"RELAGRECLAS"  , "Relatório de Agremiações x Classes de Competições"	, 1 },
                    {91 ,"CADUSU"       , "Cadastro de Usuários"				                    , 1 },
                    {92 ,"CADPERF"      , "Cadastro de Perfís"					                , 1 },
                    {93 ,"ASSPERFUSU"   , "Associação Perfil x Usuário"		                    , 1 },
                    {94 ,"CADFUNMENU"   , "Cadastro de Funções do Menu"			                , 1 },
                    {95 ,"CADTOPER"     , "Cadastro de Tipos de Operações"		                , 1 },
                    {96 ,"DEFPERM"      , "Definição de Permissões"				                , 1 },
                    {97 ,"VISREGEVEN"   , "Visualizar Registros de Eventos do Sistema"        	, 1 },
                    {98 ,"CADSIST"      , "Cadastro de Sistemas"								   	, 1 },
                    {99 ,"CADCLI"       , "Cadastro de Clientes"								   	, 1 },
                    {100 ,"TABTIPMSGSIS" , "Tabela de Tipos de Mensagens dos Sistemas"		   	, 1 },
                    {101 ,"CADMSGSIST"   , "Cadastro de Mensagens dos Sistemas"				   	, 1 },});
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("FuncoesMenus", "Id", Enumerable.Range(1, 100).ToArray());
        }
    }
}
