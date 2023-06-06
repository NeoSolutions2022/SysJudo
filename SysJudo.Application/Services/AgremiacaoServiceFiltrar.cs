using System.Text;
using SysJudo.Application.Dto.Agremiacao;
using SysJudo.Core.Extension;
using SysJudo.Domain.Entities;
using SysJudo.Domain.Entities.EntitiesFiltros;

namespace SysJudo.Application.Services;

public partial class AgremiacaoService
{
    public async Task<List<AgremiacaoFiltroDto>> Filtrar(List<FiltragemAgremiacaoDto> dto,
        List<AgremiacaoFiltro>? agremiacoes = null!, int tamanho = 0, int aux = 0)
    {
        var descricao = new StringBuilder();
        tamanho = dto.Count;

        if (aux < tamanho)
        {
            #region Sigla

            if (dto[aux].NomeParametro == "Sigla")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Sigla.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Sigla.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Sigla.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Sigla == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Sigla == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.Sigla == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Sigla != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Sigla != dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Sigla != dto[aux].ValorString);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                             1)
                                .ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 8:
                        if (dto[aux].ValorString2 == null)
                        {
                            Notificator.Handle("ValorSting2 precisa ser informado!");
                        }

                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                .ToList()
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();

                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Nome

            if (dto[aux].NomeParametro == "Nome")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Nome.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Nome.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Nome.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Nome == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Nome == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.Nome == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Nome != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Nome != dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Nome != dto[aux].ValorString);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                             1)
                                .ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 8:
                        if (dto[aux].ValorString2 == null)
                        {
                            Notificator.Handle("ValorSting2 precisa ser informado!");
                        }

                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                .ToList()
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Fantasia

            if (dto[aux].NomeParametro == "Fantasia")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c =>
                                c.Fantasia != null && c.Fantasia.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c =>
                                c.Fantasia != null && c.Fantasia.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c =>
                            c.Fantasia != null && c.Fantasia.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Fantasia == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Fantasia == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.Fantasia == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Fantasia != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Fantasia != dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Fantasia != dto[aux].ValorString);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                             1)
                                .ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 8:
                        if (dto[aux].ValorString2 == null)
                        {
                            Notificator.Handle("ValorSting2 precisa ser informado!");
                        }

                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                .ToList()
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region InscricaoMunicipal

            if (dto[aux].NomeParametro == "InscricaoMunicipal")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c =>
                                c.InscricaoMunicipal != null && c.InscricaoMunicipal.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.InscricaoMunicipal != null &&
                                                                  c.InscricaoMunicipal.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains =
                            agremiacoes.FindAll(c =>
                                c.InscricaoMunicipal != null && c.InscricaoMunicipal.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.InscricaoMunicipal == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.InscricaoMunicipal == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.InscricaoMunicipal == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.InscricaoMunicipal != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.InscricaoMunicipal != dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.InscricaoMunicipal != dto[aux].ValorString);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                             1)
                                .ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 8:
                        if (dto[aux].ValorString2 == null)
                        {
                            Notificator.Handle("ValorSting2 precisa ser informado!");
                        }

                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                .ToList()
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region InscricaoEstadual

            if (dto[aux].NomeParametro == "InscricaoEstadual")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c =>
                                c.InscricaoEstadual != null && c.InscricaoEstadual!.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c =>
                                c.InscricaoEstadual != null && c.InscricaoEstadual!.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains =
                            agremiacoes.FindAll(c =>
                                c.InscricaoEstadual != null && c.InscricaoEstadual!.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.InscricaoEstadual == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.InscricaoEstadual == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.InscricaoEstadual == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.InscricaoEstadual != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.InscricaoEstadual != dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.InscricaoEstadual != dto[aux].ValorString);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 8:
                        if (dto[aux].ValorString2 == null)
                        {
                            Notificator.Handle("ValorSting2 precisa ser informado!");
                        }

                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                .ToList()
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Responsavel

            if (dto[aux].NomeParametro == "Responsavel")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Responsavel.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Responsavel.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Responsavel.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Responsavel == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Responsavel == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.Responsavel == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Responsavel != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Responsavel != dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Responsavel != dto[aux].ValorString);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                             1)
                                .ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 8:
                        if (dto[aux].ValorString2 == null)
                        {
                            Notificator.Handle("ValorSting2 precisa ser informado!");
                        }

                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                .ToList()
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Cep

            if (dto[aux].NomeParametro == "Cep")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Cep.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Cep.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Cep.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Cep == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Cep == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.Cep == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Cep != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Cep != dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Cep != dto[aux].ValorString);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                             1)
                                .ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 8:
                        if (dto[aux].ValorString2 == null)
                        {
                            Notificator.Handle("ValorSting2 precisa ser informado!");
                        }

                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                .ToList()
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Endereco

            if (dto[aux].NomeParametro == "Endereco")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Endereco.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Endereco.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Endereco.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Endereco == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Endereco == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.Endereco == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Endereco != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Endereco != dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Endereco != dto[aux].ValorString);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                             1)
                                .ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 8:
                        if (dto[aux].ValorString2 == null)
                        {
                            Notificator.Handle("ValorSting2 precisa ser informado!");
                        }

                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                .ToList()
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Bairro

            if (dto[aux].NomeParametro == "Bairro")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Bairro.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Bairro.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Bairro.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Bairro == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Bairro == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.Bairro == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Bairro != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Bairro != dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Bairro != dto[aux].ValorString);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                             1)
                                .ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 8:
                        if (dto[aux].ValorString2 == null)
                        {
                            Notificator.Handle("ValorSting2 precisa ser informado!");
                        }

                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                .ToList()
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Complemento

            if (dto[aux].NomeParametro == "Complemento")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Complemento.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Complemento == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Complemento.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Complemento == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Complemento == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.Complemento == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Complemento != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Complemento == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Complemento != dto[aux].ValorString);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                             1)
                                .ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 8:
                        if (dto[aux].ValorString2 == null)
                        {
                            Notificator.Handle("ValorSting2 precisa ser informado!");
                        }

                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                .ToList()
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Telefone

            if (dto[aux].NomeParametro == "Telefone")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Telefone.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Telefone.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Telefone.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Telefone == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Telefone == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.Telefone == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Telefone != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Telefone == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Telefone != dto[aux].ValorString);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                             1)
                                .ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 8:
                        if (dto[aux].ValorString2 == null)
                        {
                            Notificator.Handle("ValorSting2 precisa ser informado!");
                        }

                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                .ToList()
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Email

            if (dto[aux].NomeParametro == "Email")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Email.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Email.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Email.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Email == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Email == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.Email == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Email != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Email == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Email != dto[aux].ValorString);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                             1)
                                .ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 8:
                        if (dto[aux].ValorString2 == null)
                        {
                            Notificator.Handle("ValorSting2 precisa ser informado!");
                        }

                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                .ToList()
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Cnpj

            if (dto[aux].NomeParametro == "Cnpj")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Cnpj.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Cnpj.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Cnpj.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Cnpj == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Cnpj == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.Cnpj == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Cnpj != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Cnpj == dto[aux].ValorString);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Cnpj != dto[aux].ValorString);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                             1)
                                .ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 8:
                        if (dto[aux].ValorString2 == null)
                        {
                            Notificator.Handle("ValorSting2 precisa ser informado!");
                        }

                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                .ToList()
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Representante

            if (dto[aux].NomeParametro == "Representante")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Representante.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Representante.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Representante.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Representante == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Representante == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.Representante == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //  Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Representante != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Representante == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Representante != dto[aux].ValorString);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                             1)
                                .ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 8:
                        if (dto[aux].ValorString2 == null)
                        {
                            Notificator.Handle("ValorSting2 precisa ser informado!");
                        }

                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                .ToList()
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Pais

            if (dto[aux].NomeParametro == "Pais")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Pais.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Pais.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Pais.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Pais == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Pais == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.Pais == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Pais != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Pais == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Pais != dto[aux].ValorString);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                             1)
                                .ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 8:
                        if (dto[aux].ValorString2 == null)
                        {
                            Notificator.Handle("ValorSting2 precisa ser informado!");
                        }

                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList()
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList()
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();

                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Cidade

            if (dto[aux].NomeParametro == "Cidade")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Cidade.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Cidade.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains =
                            agremiacoes.FindAll(c => c.Cidade.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Cidade == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Cidade == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.Cidade == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Cidade != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Cidade == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Cidade != dto[aux].ValorString);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                             1)
                                .ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 8:
                        if (dto[aux].ValorString2 == null)
                        {
                            Notificator.Handle("ValorSting2 precisa ser informado!");
                        }

                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                .ToList()
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Estado

            if (dto[aux].NomeParametro == "Estado")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Estado.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Estado.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains =
                            agremiacoes.FindAll(c => c.Estado.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Estado == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Estado == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.Estado == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Estado != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Estado == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Estado != dto[aux].ValorString);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                             1)
                                .ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 8:
                        if (dto[aux].ValorString2 == null)
                        {
                            Notificator.Handle("ValorSting2 precisa ser informado!");
                        }

                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                .ToList()
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Regiao

            if (dto[aux].NomeParametro == "Regiao")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.RegiaoNome.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.RegiaoNome.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains =
                            agremiacoes.FindAll(c => c.RegiaoNome.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.RegiaoNome == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.RegiaoNome == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.RegiaoNome == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.RegiaoNome != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.RegiaoNome == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.RegiaoNome != dto[aux].ValorString);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes
                            .Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                             1)
                                .ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Skip(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                .ToList();
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 8:
                        if (dto[aux].ValorString2 == null)
                        {
                            Notificator.Handle("ValorSting2 precisa ser informado!");
                        }

                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes
                                .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                                .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista
                                .Take(GetIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                .ToList()
                                .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                .ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes
                            .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!)).ToList()
                            .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                            .ToList();
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region DataFiliacao

            if (dto[aux].NomeParametro == "DataFiliacao")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //Igual
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.DataFiliacao == dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataFiliacao == dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.DataFiliacao == dto[aux].DataInicial);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.DataFiliacao != dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataFiliacao != dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.DataFiliacao != dto[aux].DataInicial);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c => c.DataFiliacao < dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataFiliacao < dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.FindAll(c => c.DataFiliacao < dto[aux].DataInicial);
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes.FindAll(c => c.DataFiliacao <= dto[aux].DataInicial);
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataFiliacao <= dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue = agremiacoes.FindAll(c => c.DataFiliacao <= dto[aux].DataInicial);
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c => c.DataFiliacao > dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataFiliacao <= dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.FindAll(c => c.DataFiliacao > dto[aux].DataInicial);
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.DataFiliacao >= dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataFiliacao >= dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue = agremiacoes.FindAll(c => c.DataFiliacao >= dto[aux].DataInicial);
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c =>
                                c.DataFiliacao < dto[aux].DataFinal && c.DataFiliacao > dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c =>
                                c.DataFiliacao < dto[aux].DataFinal && c.DataFiliacao > dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.FindAll(c =>
                            c.DataFiliacao < dto[aux].DataFinal && c.DataFiliacao > dto[aux].DataInicial);
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region DataNascimento

            if (dto[aux].NomeParametro == "DataNascimento")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //Igual
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.DataNascimento == dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataNascimento == dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.DataNascimento == dto[aux].DataInicial);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.DataNascimento != dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataNascimento != dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.DataNascimento != dto[aux].DataInicial);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c => c.DataNascimento < dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataNascimento < dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.FindAll(c => c.DataNascimento < dto[aux].DataInicial);
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes.FindAll(c => c.DataNascimento <= dto[aux].DataInicial);
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataNascimento <= dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue = agremiacoes.FindAll(c => c.DataNascimento <= dto[aux].DataInicial);
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c => c.DataNascimento > dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataNascimento > dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.FindAll(c => c.DataNascimento > dto[aux].DataInicial);
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.DataNascimento >= dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataNascimento >= dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue = agremiacoes.FindAll(c => c.DataNascimento >= dto[aux].DataInicial);
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c =>
                                c.DataNascimento < dto[aux].DataFinal && c.DataNascimento > dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c =>
                                c.DataNascimento < dto[aux].DataFinal && c.DataNascimento > dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.FindAll(c =>
                            c.DataNascimento < dto[aux].DataFinal && c.DataNascimento > dto[aux].DataInicial);
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region DataCnpj

            if (dto[aux].NomeParametro == "DataCnpj")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //Igual
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c =>
                                c.DataCnpj != null && c.DataCnpj == dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c =>
                                c.DataCnpj != null && c.DataCnpj == dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual =
                            agremiacoes.FindAll(c => c.DataCnpj != null && c.DataCnpj == dto[aux].DataInicial);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.DataCnpj != dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataCnpj != dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.DataCnpj != dto[aux].DataInicial);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c => c.DataCnpj < dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataCnpj < dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.FindAll(c => c.DataCnpj < dto[aux].DataInicial);
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes.FindAll(c => c.DataCnpj <= dto[aux].DataInicial);
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataCnpj <= dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue = agremiacoes.FindAll(c => c.DataCnpj <= dto[aux].DataInicial);
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c => c.DataCnpj > dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataCnpj > dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.FindAll(c => c.DataCnpj > dto[aux].DataInicial);
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.DataCnpj >= dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataCnpj >= dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue = agremiacoes.FindAll(c => c.DataCnpj >= dto[aux].DataInicial);
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c =>
                                c.DataCnpj < dto[aux].DataFinal && c.DataCnpj > dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c =>
                                c.DataCnpj < dto[aux].DataFinal && c.DataCnpj > dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.FindAll(c =>
                            c.DataCnpj < dto[aux].DataFinal && c.DataCnpj > dto[aux].DataInicial);
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region DataAta

            if (dto[aux].NomeParametro == "DataAta")
            {
                agremiacoes = await PossuiAgremiacao(dto[aux].NomeParametro, agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //Igual
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.DataAta != null && c.DataAta == dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(
                                c => c.DataAta != null && c.DataAta == dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual =
                            agremiacoes.FindAll(c => c.DataAta != null && c.DataAta == dto[aux].DataInicial);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.DataAta != dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataAta != dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.DataAta != dto[aux].DataInicial);
                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c => c.DataAta < dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataAta < dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.FindAll(c => c.DataAta < dto[aux].DataInicial);
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes.FindAll(c => c.DataAta <= dto[aux].DataInicial);
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataAta <= dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue = agremiacoes.FindAll(c => c.DataAta <= dto[aux].DataInicial);
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c => c.DataAta > dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataAta > dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.FindAll(c => c.DataAta > dto[aux].DataInicial);
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.DataAta >= dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataAta >= dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue = agremiacoes.FindAll(c => c.DataAta >= dto[aux].DataInicial);
                        return await Filtrar(dto, filtroMaiorIgualQue, tamanho, ++aux);

                    //Entre
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c =>
                                c.DataAta < dto[aux].DataFinal && c.DataAta > dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c =>
                                c.DataAta < dto[aux].DataFinal && c.DataAta > dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.FindAll(c =>
                            c.DataAta < dto[aux].DataFinal && c.DataAta > dto[aux].DataInicial);
                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);

                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion
        }

        await _filtroRepository.RemoverTodos();
        if (agremiacoes != null)
        {
            foreach (var agremiacao in agremiacoes)
            {
                _filtroRepository.Cadastrar(agremiacao);
            }
        }

        var valor = "";
        var valor2 = "";
        var operadorLogico = "";
        foreach (var pesquisa in dto)
        {
            operadorLogico = pesquisa.OperadorLogico is 1 or null ? "E" : "OU";

            switch (pesquisa.OperacaoId)
            {
                case 1:
                {
                    if (pesquisa.ValorString != null)
                    {
                        valor = pesquisa.ValorString;
                    }
                    else if (pesquisa.DataFinal != null)
                    {
                        valor = pesquisa.DataInicial!.Value.ToString("yy-MM-dd");
                    }

                    descricao.Append(
                        $"Campo={pesquisa.NomeParametro};Tipo de operação=Contain;Valor={valor};Operador lógico={operadorLogico};");
                }
                    break;
                case 2:
                {
                    if (pesquisa.ValorString != null)
                    {
                        valor = pesquisa.ValorString;
                    }
                    else if (pesquisa.DataFinal != null)
                    {
                        valor = pesquisa.DataInicial!.Value.ToString("yy-MM-dd");
                    }

                    descricao.Append(
                        $"Campo={pesquisa.NomeParametro};Tipo de operação=Igual;Valor={valor};Operador lógico={operadorLogico};");
                }
                    break;
                case 3:
                {
                    if (pesquisa.ValorString != null)
                    {
                        valor = pesquisa.ValorString;
                    }
                    else if (pesquisa.DataFinal != null)
                    {
                        valor = pesquisa.DataInicial!.Value.ToString("yy-MM-dd");
                    }

                    descricao.Append(
                        $"Campo={pesquisa.NomeParametro};Tipo de operação=Diferente;Valor={valor};Operador lógico={operadorLogico};");
                }
                    break;
                case 4:
                {
                    if (pesquisa.ValorString != null)
                    {
                        valor = pesquisa.ValorString;
                    }
                    else if (pesquisa.DataFinal != null)
                    {
                        valor = pesquisa.DataInicial!.Value.ToString("yy-MM-dd");
                    }

                    descricao.Append(
                        $"Campo={pesquisa.NomeParametro};Tipo de operação=Menor;Valor={valor};Operador lógico={operadorLogico};");
                }
                    break;
                case 5:
                {
                    if (pesquisa.ValorString != null)
                    {
                        valor = pesquisa.ValorString;
                    }
                    else if (pesquisa.DataFinal != null)
                    {
                        valor = pesquisa.DataInicial!.Value.ToString("yy-MM-dd");
                    }

                    descricao.Append(
                        $"Campo={pesquisa.NomeParametro};Tipo de operação=MenorIgual;Valor={valor};Operador lógico={operadorLogico};");
                }
                    break;
                case 6:
                {
                    if (pesquisa.ValorString != null)
                    {
                        valor = pesquisa.ValorString;
                    }
                    else if (pesquisa.DataFinal != null)
                    {
                        valor = pesquisa.DataInicial!.Value.ToString("yy-MM-dd");
                    }

                    descricao.Append(
                        $"Campo={pesquisa.NomeParametro};Tipo de operação=Maior;Valor={valor};Operador lógico={operadorLogico};");
                }
                    break;
                case 7:
                {
                    if (pesquisa.ValorString != null)
                    {
                        valor = pesquisa.ValorString;
                    }
                    else if (pesquisa.DataFinal != null)
                    {
                        valor = pesquisa.DataInicial!.Value.ToString("yy-MM-dd");
                    }

                    descricao.Append(
                        $"Campo={pesquisa.NomeParametro};Tipo de operação=MaiorIgual;Valor={valor};Operador lógico={operadorLogico};");
                }
                    break;
                case 8:
                {
                    if (pesquisa.ValorString != null)
                    {
                        valor = pesquisa.ValorString;
                        valor2 = pesquisa.ValorString2;
                    }
                    else if (pesquisa.DataFinal != null)
                    {
                        valor = pesquisa.DataInicial!.Value.ToString("yy-MM-dd");
                        valor2 = pesquisa.DataFinal.Value.ToString("yy-MM-dd");
                    }

                    descricao.Append(
                        $"Campo={pesquisa.NomeParametro};Tipo de operação=Entre;Valores={valor} e {valor2};Operador lógico={operadorLogico};");
                }
                    break;
            }
        }

        RegistroDeEventos.Adicionar(new RegistroDeEvento
        {
            DataHoraEvento = DateTime.Now,
            ComputadorId = ObterIp(),
            Descricao = $"{descricao}",
            ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
            TipoOperacaoId = 13,
            UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
            AdministradorNome = null,
            UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
            AdministradorId = null,
            FuncaoMenuId = 2
        });

        if (await _filtroRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<List<AgremiacaoFiltroDto>>(
                await _filtroRepository.Listar());
        }

        return Mapper.Map<List<AgremiacaoFiltroDto>>(
            await _filtroRepository.Listar());
    }

    private async Task<List<AgremiacaoFiltro>> PossuiAgremiacao(string nomeParametro,
        List<AgremiacaoFiltro>? agremiacoes = null)
    {
        if (agremiacoes == null)
        {
            var agremiacoesN = await _agremiacaoRepository.ObterTodos();
            agremiacoes = Mapper.Map<List<AgremiacaoFiltro>>(agremiacoesN);
            foreach (var agremiacaoN in agremiacoesN)
            {
                foreach (var agremiacaoF in agremiacoes.Where(agremiacaoF => agremiacaoN.Sigla == agremiacaoF.Sigla))
                {
                    agremiacaoF.RegiaoNome = agremiacaoN.Regiao.Descricao;
                    agremiacaoF.Pais = agremiacaoN.Pais;
                    agremiacaoF.Cidade = agremiacaoN.Cidade;
                    agremiacaoF.Estado = agremiacaoN.Estado;
                }
            }
        }

        switch (nomeParametro)
        {
            case "Sigla": return agremiacoes.OrderBy(c => c.Sigla).ToList();
            case "Nome": return agremiacoes.OrderBy(c => c.Nome).ToList();
            case "Fantasia": return agremiacoes.OrderBy(c => c.Fantasia).ToList();
            case "Responsavel": return agremiacoes.OrderBy(c => c.Responsavel).ToList();
            case "Representante": return agremiacoes.OrderBy(c => c.Representante).ToList();
            case "DataFiliacao": return agremiacoes.OrderBy(c => c.DataFiliacao).ToList();
            case "DataNascimento": return agremiacoes.OrderBy(c => c.DataNascimento).ToList();
            case "Cep": return agremiacoes.OrderBy(c => c.Cep).ToList();
            case "Endereco": return agremiacoes.OrderBy(c => c.Endereco).ToList();
            case "Bairro": return agremiacoes.OrderBy(c => c.Bairro).ToList();
            case "Complemento": return agremiacoes.OrderBy(c => c.Complemento).ToList();
            case "Telefone": return agremiacoes.OrderBy(c => c.Telefone).ToList();
            case "Email": return agremiacoes.OrderBy(c => c.Email).ToList();
            case "Cnpj": return agremiacoes.OrderBy(c => c.Cnpj).ToList();
            case "InscricaoMunicipal": return agremiacoes.OrderBy(c => c.InscricaoMunicipal).ToList();
            case "InscricaoEstadual": return agremiacoes.OrderBy(c => c.InscricaoEstadual).ToList();
            case "DataCnpj": return agremiacoes.OrderBy(c => c.DataCnpj).ToList();
            case "DataAta": return agremiacoes.OrderBy(c => c.DataAta).ToList();
            case "Pais": return agremiacoes.OrderBy(c => c.Pais).ToList();
            case "Estado": return agremiacoes.OrderBy(c => c.Estado).ToList();
            case "Cidade": return agremiacoes.OrderBy(c => c.Cidade).ToList();
            case "Regiao": return agremiacoes.OrderBy(c => c.RegiaoNome).ToList();
            default: return agremiacoes.OrderBy(c => c.Id).ToList();
        }
    }

    private int GetLastIndex(List<AgremiacaoFiltro>? agremiacoes, string nomeParametro, string nome)
    {
        switch (nomeParametro)
        {
            case "Sigla":
            {
                var index = agremiacoes!.FindLastIndex(c => c.Sigla == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
            case "Fantasia":
            {
                var index = agremiacoes!.FindLastIndex(c => c.Fantasia == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
            case "Responsavel":
            {
                var index = agremiacoes!.FindLastIndex(c => c.Responsavel == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
            case "Representante":
            {
                var index = agremiacoes!.FindLastIndex(c => c.Representante == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
            case "Cep":
            {
                var index = agremiacoes!.FindLastIndex(c => c.Cep == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
            case "Endereco":
            {
                var index = agremiacoes!.FindLastIndex(c => c.Endereco == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
            case "Bairro":
            {
                var index = agremiacoes!.FindLastIndex(c => c.Bairro == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
            case "Complemento":
            {
                var index = agremiacoes!.FindLastIndex(c => c.Complemento == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
            case "Telefone":
            {
                var index = agremiacoes!.FindLastIndex(c => c.Telefone == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
            case "Email":
            {
                var index = agremiacoes!.FindLastIndex(c => c.Email == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
            case "Cnpj":
            {
                var index = agremiacoes!.FindLastIndex(c => c.Cnpj == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
            case "InscricaoMunicipal":
            {
                var index = agremiacoes!.FindLastIndex(c => c.InscricaoMunicipal == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
            case "InscricaoEstadual":
            {
                var index = agremiacoes!.FindLastIndex(c => c.InscricaoEstadual == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
            case "Pais":
            {
                var index = agremiacoes!.FindLastIndex(c => c.Pais == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
            case "Estado":
            {
                var index = agremiacoes!.FindLastIndex(c => c.Estado == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
            case "Cidade":
            {
                var index = agremiacoes!.FindLastIndex(c => c.Cidade == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
            case "Regiao":
            {
                var index = agremiacoes!.FindLastIndex(c => c.RegiaoNome == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
            default:
            {
                var index = agremiacoes!.FindLastIndex(c => c.Nome == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
        }
    }

    private int GetIndex(List<AgremiacaoFiltro>? agremiacoes, string nomeParametro, string nome)
    {
        switch (nomeParametro)
        {
            case "Sigla":
            {
                var index = agremiacoes!.FindIndex(c => c.Sigla == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            case "Fantasia":
            {
                var index = agremiacoes!.FindIndex(c => c.Fantasia == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            case "Responsavel":
            {
                var index = agremiacoes!.FindIndex(c => c.Responsavel == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            case "Representante":
            {
                var index = agremiacoes!.FindIndex(c => c.Representante == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            case "Cep":
            {
                var index = agremiacoes!.FindIndex(c => c.Cep == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            case "Endereco":
            {
                var index = agremiacoes!.FindIndex(c => c.Endereco == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            case "Bairro":
            {
                var index = agremiacoes!.FindIndex(c => c.Bairro == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            case "Complemento":
            {
                var index = agremiacoes!.FindIndex(c => c.Complemento == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            case "Telefone":
            {
                var index = agremiacoes!.FindIndex(c => c.Telefone == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            case "Email":
            {
                var index = agremiacoes!.FindIndex(c => c.Email == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            case "Cnpj":
            {
                var index = agremiacoes!.FindIndex(c => c.Cnpj == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            case "InscricaoMunicipal":
            {
                var index = agremiacoes!.FindIndex(c => c.InscricaoMunicipal == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            case "InscricaoEstadual":
            {
                var index = agremiacoes!.FindIndex(c => c.InscricaoEstadual == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            case "Pais":
            {
                var index = agremiacoes!.FindIndex(c => c.Pais == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            case "Estado":
            {
                var index = agremiacoes!.FindIndex(c => c.Estado == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            case "Cidade":
            {
                var index = agremiacoes!.FindIndex(c => c.Cidade == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            case "Regiao":
            {
                var index = agremiacoes!.FindIndex(c => c.RegiaoNome == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            default:
            {
                var index = agremiacoes!.FindIndex(c => c.Nome == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
        }
    }
}