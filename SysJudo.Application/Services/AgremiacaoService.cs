using System.Text;
using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Agremiacao;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Notifications;
using SysJudo.Core.Enums;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Contracts.Repositories.RepositoriesFiltros;
using SysJudo.Domain.Entities;
using SysJudo.Domain.Entities.EntitiesFiltros;

namespace SysJudo.Application.Services;

public class AgremiacaoService : BaseService, IAgremiacaoService
{
    private readonly IAgremiacaoRepository _agremiacaoRepository;
    private readonly IAgremiacaoFiltroRepository _filtroRepository;
    private readonly IFileService _fileService;

    public AgremiacaoService(IMapper mapper, INotificator notificator, IAgremiacaoRepository agremiacaoRepository,
        IFileService fileService, IAgremiacaoFiltroRepository filtroRepository) : base(mapper, notificator)
    {
        _agremiacaoRepository = agremiacaoRepository;
        _fileService = fileService;
        _filtroRepository = filtroRepository;
    }

    public async Task<List<AgremiacaoFiltroDto>> Filtrar(List<FiltragemAgremiacaoDto> dto,
        List<Agremiacao> agremiacoes = null!, int tamanho = 0, int aux = 0)
    {
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
                            var and = agremiacoes.FindAll(c => c.Fantasia != null && c.Fantasia.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Fantasia != null && c.Fantasia.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Fantasia != null && c.Fantasia.Contains(dto[aux].ValorString!));
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
                            var and = agremiacoes.FindAll(c => c.InscricaoMunicipal != null && c.InscricaoMunicipal.Contains(dto[aux].ValorString!));
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
                            agremiacoes.FindAll(c => c.InscricaoMunicipal != null && c.InscricaoMunicipal.Contains(dto[aux].ValorString!));
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
                            var and = agremiacoes.FindAll(c => c.InscricaoEstadual != null &&  c.InscricaoEstadual!.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.InscricaoEstadual != null && c.InscricaoEstadual!.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains =
                            agremiacoes.FindAll(c => c.InscricaoEstadual != null && c.InscricaoEstadual!.Contains(dto[aux].ValorString!));
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
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Complemento.Contains(dto[aux].ValorString!));
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
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Complemento != dto[aux].ValorString);
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
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Telefone != dto[aux].ValorString);
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
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Email != dto[aux].ValorString);
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
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Cnpj != dto[aux].ValorString);
                            agremiacoes.AddRange(or);
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
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Representante != dto[aux].ValorString);
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
                            var and = agremiacoes.FindAll(c => c.Pais.Descricao.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Pais.Descricao.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Pais.Descricao.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Pais.Descricao == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Pais.Descricao == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.Pais.Descricao == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Pais.Descricao != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Pais.Descricao != dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Pais.Descricao != dto[aux].ValorString);
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
                            var and = agremiacoes.FindAll(c => c.Cidade.Descricao.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Cidade.Descricao.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains =
                            agremiacoes.FindAll(c => c.Cidade.Descricao.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Cidade.Descricao == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Cidade.Descricao == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.Cidade.Descricao == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Cidade.Descricao != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Cidade.Descricao != dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Cidade.Descricao != dto[aux].ValorString);
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
                            var and = agremiacoes.FindAll(c => c.Estado.Descricao.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Estado.Descricao.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains =
                            agremiacoes.FindAll(c => c.Estado.Descricao.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Estado.Descricao == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Estado.Descricao == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.Estado.Descricao == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Estado.Descricao != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Estado.Descricao != dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Estado.Descricao != dto[aux].ValorString);
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
                            var and = agremiacoes.FindAll(c => c.Regiao.Descricao.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Regiao.Descricao.Contains(dto[aux].ValorString!));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains =
                            agremiacoes.FindAll(c => c.Regiao.Descricao.Contains(dto[aux].ValorString!));
                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Igual
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Regiao.Descricao == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.Regiao.Descricao == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.Regiao.Descricao == dto[aux].ValorString);
                        return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                    //Diferente
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Regiao.Descricao != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Regiao.Descricao != dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Regiao.Descricao != dto[aux].ValorString);
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
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.DataFiliacao > dto[aux].DataInicial);
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
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
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
                            var and = agremiacoes.FindAll(c => c.DataCnpj != null && c.DataCnpj == dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao(dto[aux].NomeParametro);
                            var or = agremiacaoLista.FindAll(c => c.DataCnpj != null && c.DataCnpj == dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.DataCnpj != null && c.DataCnpj == dto[aux].DataInicial);
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
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
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
                            var or = agremiacaoLista.FindAll(c => c.DataAta != null && c.DataAta == dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroIgual = agremiacoes.FindAll(c => c.DataAta != null && c.DataAta == dto[aux].DataInicial);
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
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
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

        var agremiacoesFiltro = Mapper.Map<List<AgremiacaoFiltro>>(agremiacoes);
        await _filtroRepository.RemoverTodos();
        foreach (var agremiacao in agremiacoesFiltro.DistinctBy(c => c.Id))
        {
            agremiacao.PaisNome = agremiacoes.FirstOrDefault(c => c.IdPais == agremiacao.IdPais)!.Pais.Descricao;
            agremiacao.EstadoNome =
                agremiacoes.FirstOrDefault(c => c.IdEstado == agremiacao.IdEstado)!.Estado.Descricao;
            agremiacao.CidadeNome =
                agremiacoes.FirstOrDefault(c => c.IdCidade == agremiacao.IdCidade)!.Cidade.Descricao;
            agremiacao.RegiaoNome =
                agremiacoes.FirstOrDefault(c => c.IdRegiao == agremiacao.IdRegiao)!.Regiao.Descricao;

            _filtroRepository.Cadastrar(agremiacao);
        }

        if (await _filtroRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<List<AgremiacaoFiltroDto>>(
                await _filtroRepository.Listar());
        }

        return Mapper.Map<List<AgremiacaoFiltroDto>>(
            await _filtroRepository.Listar());
    }

    public async Task<AgremiacaoDto?> Cadastrar(CadastrarAgremiacaoDto dto)
    {
        if (!ValidarAnexos(dto))
        {
            return null;
        }

        var agremiacao = Mapper.Map<Agremiacao>(dto);
        if (!await Validar(agremiacao))
        {
            return null;
        }

        StringBuilder links = new StringBuilder();
        if (dto.Documentos != null)
        {
            foreach (var documento in dto.Documentos)
            {
                if (documento is { Length: > 0 })
                {
                    links.Append(agremiacao.DocumentosUri + "&" +
                                 await _fileService.Upload(documento, EUploadPath.FotosAgremiacao));
                }
            }
        }

        agremiacao.DocumentosUri = links.ToString();

        if (dto.Foto is { Length: > 0 })
        {
            agremiacao.Foto = await _fileService.Upload(dto.Foto, EUploadPath.FotosAgremiacao);
        }

        _agremiacaoRepository.Cadastrar(agremiacao);
        if (await _agremiacaoRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<AgremiacaoDto>(agremiacao);
        }

        Notificator.Handle("Não foi possível cadastrar a agremiação");
        return null;
    }

    public async Task<AgremiacaoDto?> Alterar(int id, AlterarAgremiacaoDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("Os ids não conferem");
            return null;
        }

        var agremiacao = await _agremiacaoRepository.Obter(id);
        if (agremiacao == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        Mapper.Map(dto, agremiacao);
        if (!await Validar(agremiacao))
        {
            return null;
        }

        if (dto.Foto is { Length: > 0 } && !await ManterFoto(dto.Foto, agremiacao))
        {
            return null;
        }

        StringBuilder links = new StringBuilder();
        if (dto.Documentos.Count != 0)
        {
            foreach (var documento in dto.Documentos)
            {
                if (documento is { Length: > 0 })
                {
                    links.Append(agremiacao.DocumentosUri + "&" +
                                 await _fileService.Upload(documento, EUploadPath.FotosAgremiacao));
                }
            }
        }

        agremiacao.DocumentosUri = links.ToString();

        _agremiacaoRepository.Alterar(agremiacao);

        if (await _agremiacaoRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<AgremiacaoDto>(agremiacao);
        }

        Notificator.Handle("Não foi possível alterar a agremiação");
        return null;
    }

    #region Exportar

    public async Task<string> Exportar(ExportarAgremiacaoDto dto)
    {
        var linha = 2;
        var agremiacoes = await _filtroRepository.Listar();
        var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("planilhaAgremiacoes");

        foreach (var agremiacao in agremiacoes)
        {
            var contador = 1;
            if (dto.Nome)
            {
                ws.Cell(1, contador).Value = "Nome";
                ws.Cell(linha, contador).Value = agremiacao.Nome;
                contador++;
            }

            if (dto.Sigla)
            {
                ws.Cell(1, contador).Value = "Sigla";
                ws.Cell(linha, contador).Value = agremiacao.Sigla;
                contador++;
            }

            if (dto.Fantasia)
            {
                ws.Cell(1, contador).Value = "Fantasia";
                ws.Cell(linha, contador).Value = agremiacao.Fantasia;
                contador++;
            }

            if (dto.Responsavel)
            {
                ws.Cell(1, contador).Value = "Responsavel";
                ws.Cell(linha, contador).Value = agremiacao.Responsavel;
                contador++;
            }

            if (dto.Representante)
            {
                ws.Cell(1, contador).Value = "Representante";
                ws.Cell(linha, contador).Value = agremiacao.Representante;
                contador++;
            }

            if (dto.DataFiliacao)
            {
                ws.Cell(1, contador).Value = "DataFiliacao";
                ws.Cell(linha, contador).Value = agremiacao.DataFiliacao.ToString();
                contador++;
            }

            if (dto.DataNascimento)
            {
                ws.Cell(1, contador).Value = "DataNascimento";
                ws.Cell(linha, contador).Value = agremiacao.DataNascimento.ToString();
                contador++;
            }

            if (dto.Cep)
            {
                ws.Cell(1, contador).Value = "Cep";
                ws.Cell(linha, contador).Value = agremiacao.Cep;
                contador++;
            }

            if (dto.Endereco)
            {
                ws.Cell(1, contador).Value = "Endereco";
                ws.Cell(linha, contador).Value = agremiacao.Endereco;
                contador++;
            }

            if (dto.Endereco)
            {
                ws.Cell(1, contador).Value = "Bairro";
                ws.Cell(linha, contador).Value = agremiacao.Endereco;
                contador++;
            }

            if (dto.Complemento)
            {
                ws.Cell(1, contador).Value = "Complemento";
                ws.Cell(linha, contador).Value = agremiacao.Complemento;
                contador++;
            }

            if (dto.IdCidade)
            {
                ws.Cell(1, contador).Value = "Cidade";
                ws.Cell(linha, contador).Value = agremiacao.CidadeNome;
                contador++;
            }

            if (dto.IdEstado)
            {
                ws.Cell(1, contador).Value = "Estado";
                ws.Cell(linha, contador).Value = agremiacao.EstadoNome;
                contador++;
            }

            if (dto.IdRegiao)
            {
                ws.Cell(1, contador).Value = "Regiao";
                ws.Cell(linha, contador).Value = agremiacao.RegiaoNome;
                contador++;
            }

            if (dto.IdPais)
            {
                ws.Cell(1, contador).Value = "Pais";
                ws.Cell(linha, contador).Value = agremiacao.PaisNome;
                contador++;
            }

            if (dto.Telefone)
            {
                ws.Cell(1, contador).Value = "Telefone";
                ws.Cell(linha, contador).Value = agremiacao.Telefone;
                contador++;
            }

            if (dto.Email)
            {
                ws.Cell(1, contador).Value = "Email";
                ws.Cell(linha, contador).Value = agremiacao.Email;
                contador++;
            }

            if (dto.Cnpj)
            {
                ws.Cell(1, contador).Value = "Cnpj";
                ws.Cell(linha, contador).Value = agremiacao.Cnpj;
            }

            linha++;
        }

        ws.Columns().AdjustToContents();
        return await _fileService.UploadExcel(workbook, EUploadPath.FotosAgremiacao);
    }

    #endregion

    public async Task<PagedDto<AgremiacaoDto>> Buscar(BuscarAgremiacaoDto dto)
    {
        var agremiacao = await _agremiacaoRepository.Buscar(dto);
        return Mapper.Map<PagedDto<AgremiacaoDto>>(agremiacao);
    }

    public async Task<List<AgremiacaoFiltroDto>> Pesquisar(string valor)
    {
        var agremiacoes = await _filtroRepository.Pesquisar(valor);

        if (agremiacoes == null)
        {
            return new List<AgremiacaoFiltroDto>();
        }

        return Mapper.Map<List<AgremiacaoFiltroDto>>(agremiacoes);
    }

    public async Task<AgremiacaoDto?> ObterPorId(int id)
    {
        var agremiacao = await _agremiacaoRepository.Obter(id);
        
        if (agremiacao == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }
        
        var agremiacaoDto = Mapper.Map<AgremiacaoDto>(agremiacao);
        var documentos = agremiacao.DocumentosUri.Split('&').ToList();
        foreach (var documento in documentos)
        {
            agremiacaoDto.Documentos.Add(new DocumentosDto
            {
                Nome = "Documento",
                Link = documento
            });
        }


        return agremiacaoDto;
    }

    public async Task LimparFiltro()
    {
        await _filtroRepository.RemoverTodos();

        var agremiacoes = await _agremiacaoRepository.ObterTodos();
        if (agremiacoes == null)
        {
            Notificator.Handle("Não existe agremiações");
        }

        var agremiacoesFiltro = Mapper.Map<List<AgremiacaoFiltro>>(agremiacoes!);
        await _filtroRepository.RemoverTodos();
        foreach (var agremiacao in agremiacoesFiltro.DistinctBy(c => c.Id))
        {
            agremiacao.PaisNome = agremiacoes.FirstOrDefault(c => c.IdPais == agremiacao.IdPais)!.Pais.Descricao;
            agremiacao.EstadoNome =
                agremiacoes.FirstOrDefault(c => c.IdEstado == agremiacao.IdEstado)!.Estado.Descricao;
            agremiacao.CidadeNome =
                agremiacoes.FirstOrDefault(c => c.IdCidade == agremiacao.IdCidade)!.Cidade.Descricao;
            agremiacao.RegiaoNome =
                agremiacoes.FirstOrDefault(c => c.IdRegiao == agremiacao.IdRegiao)!.Regiao.Descricao;

            _filtroRepository.Cadastrar(agremiacao);
        }

        if (!await _agremiacaoRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível limpar o filtro");
        }
    }

    public async Task Deletar(int id)
    {
        var cliente = await _agremiacaoRepository.Obter(id);
        if (cliente == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        _agremiacaoRepository.Deletar(cliente);
        if (!await _agremiacaoRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível remover a agremiação");
        }
    }

    public async Task Anotar(int id, AnotarAgremiacaoDto dto)
    {
        var agremiacao = await _agremiacaoRepository.Obter(id);
        if (agremiacao == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        agremiacao.Anotacoes = dto.Anotacoes;
        _agremiacaoRepository.Alterar(agremiacao);
        if (!await _agremiacaoRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível alterar anotação");
        }
    }

    public async Task EnviarDocumentos(int id, EnviarDocumentosDto dto)
    {
        var agremiacao = await _agremiacaoRepository.Obter(id);
        if (agremiacao == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        StringBuilder links = new StringBuilder();
        foreach (var documento in dto.Documentos)
        {
            if (documento is { Length: > 0 })
            {
                links.Append("&" +
                             await _fileService.Upload(documento, EUploadPath.FotosAgremiacao));
            }
        }

        agremiacao.DocumentosUri += links.ToString();
        _agremiacaoRepository.Alterar(agremiacao);
        if (!await _agremiacaoRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível enviar documentos.");
        }
    }

    private async Task<List<Agremiacao>> PossuiAgremiacao(string nomeParametro, List<Agremiacao>? agremiacoes = null)
    {
        if (agremiacoes == null)
        {
            agremiacoes = await _agremiacaoRepository.ObterTodos();
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
            case "Pais": return agremiacoes.OrderBy(c => c.Pais.Descricao).ToList();
            case "Estado": return agremiacoes.OrderBy(c => c.Estado.Descricao).ToList();
            case "Cidade": return agremiacoes.OrderBy(c => c.Cidade.Descricao).ToList();
            case "Regiao": return agremiacoes.OrderBy(c => c.Regiao.Descricao).ToList();
            default: return agremiacoes.OrderBy(c => c.Id).ToList();
        }
    }

    private int GetLastIndex(List<Agremiacao>? agremiacoes, string nomeParametro, string nome)
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
            case "Nome":
            {
                var index = agremiacoes!.FindLastIndex(c => c.Nome == nome);
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
                var index = agremiacoes!.FindLastIndex(c => c.Pais.Descricao == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
            case "Estado":
            {
                var index = agremiacoes!.FindLastIndex(c => c.Estado.Descricao == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
            case "Cidade":
            {
                var index = agremiacoes!.FindLastIndex(c => c.Cidade.Descricao == nome);
                if (index < 0)
                {
                    return agremiacoes.Count + 1;
                }

                return index;
            }
            case "Regiao":
            {
                var index = agremiacoes!.FindLastIndex(c => c.Regiao.Descricao == nome);
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

    private int GetIndex(List<Agremiacao>? agremiacoes, string nomeParametro, string nome)
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
            case "Nome":
            {
                var index = agremiacoes!.FindIndex(c => c.Nome == nome);
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
                var index = agremiacoes!.FindIndex(c => c.Pais.Descricao == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            case "Estado":
            {
                var index = agremiacoes!.FindIndex(c => c.Estado.Descricao == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            case "Cidade":
            {
                var index = agremiacoes!.FindIndex(c => c.Cidade.Descricao == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            case "Regiao":
            {
                var index = agremiacoes!.FindIndex(c => c.Regiao.Descricao == nome);
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

    private async Task<bool> Validar(Agremiacao agremiacao)
    {
        if (!agremiacao.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }

        var existente = await _agremiacaoRepository.FirstOrDefault(s =>
            (s.Sigla == agremiacao.Sigla || s.Nome == agremiacao.Nome) && s.Id != agremiacao.Id);
        if (existente != null)
        {
            Notificator.Handle("Já existe uma agremiação cadastrada com essa sigla e/ou nome");
        }

        return !Notificator.HasNotification;
    }

    #region ManterAnexos

    private async Task<bool> ManterFoto(IFormFile foto, Agremiacao agremiacao)
    {
        agremiacao.Foto = await _fileService.Upload(foto, EUploadPath.FotosAgremiacao);
        return true;
    }

    #endregion

    private bool ValidarAnexos(CadastrarAgremiacaoDto dto)
    {
        if (dto.Foto?.Length > 10000000)
        {
            Notificator.Handle("Foto deve ter no máximo 10Mb");
        }

        if (dto.Foto != null && dto.Foto.FileName.Split(".").Last() != "jfif" &&
            dto.Foto.FileName.Split(".").Last() != "png" && dto.Foto.FileName.Split(".").Last() != "jpg" 
            && dto.Foto.FileName.Split(".").Last() != "jpeg")
        {
            Notificator.Handle("Foto deve do tipo png, jfif ou jpg");
        }

        return !Notificator.HasNotification;
    }
    //
    // public async void Pesquisar(string valor)
    // {
    //     var agremiacao = await _agremiacaoRepository.ObterTodos();
    //
    //     var valores = agremiacao.Select(c => c.Sigla == valor || c.DataAta == DateOnly.Parse(valor));
    // }
}