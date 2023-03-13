using System.Text;
using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Agremiacao;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Filtros;
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

    public async Task<PagedDto<AgremiacaoFiltroDto>> Filtrar(List<FiltragemAgremiacaoDto> dto,
        List<Agremiacao> agremiacoes = null!, int tamanho = 0, int aux = 0)
    {
        tamanho = dto.Count;

        if (aux < tamanho)
        {
            #region Sigla

            if (dto[aux].NomeParametro == "Sigla")
            {
                agremiacoes = await PossuiAgremiacao(agremiacoes);
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                            var and = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                            .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                agremiacoes = await PossuiAgremiacao(agremiacoes);
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                            var and = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                            .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                agremiacoes = await PossuiAgremiacao(agremiacoes);
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                            var and = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                            .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                agremiacoes = await PossuiAgremiacao(agremiacoes);
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                            var and = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                            .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                agremiacoes = await PossuiAgremiacao(agremiacoes);
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                            var and = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                            .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                agremiacoes = await PossuiAgremiacao(agremiacoes);
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                            var and = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                            .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                agremiacoes = await PossuiAgremiacao(agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => dto[aux].ValorString!.Contains(c.Complemento));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => dto[aux].ValorString!.Contains(c.Complemento));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => dto[aux].ValorString!.Contains(c.Complemento));
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                            var and = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                            .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                agremiacoes = await PossuiAgremiacao(agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => dto[aux].ValorString!.Contains(c.Telefone));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.FindAll(c => dto[aux].ValorString!.Contains(c.Telefone));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => dto[aux].ValorString!.Contains(c.Telefone));
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                            var and = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                            .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                agremiacoes = await PossuiAgremiacao(agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => dto[aux].ValorString!.Contains(c.Email));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.FindAll(c => dto[aux].ValorString!.Contains(c.Email));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => dto[aux].ValorString!.Contains(c.Email));
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                            var and = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                            .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                agremiacoes = await PossuiAgremiacao(agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => dto[aux].ValorString!.Contains(c.Cnpj));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.FindAll(c => dto[aux].ValorString!.Contains(c.Cnpj));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => dto[aux].ValorString!.Contains(c.Cnpj));
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                            var and = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                            .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                agremiacoes = await PossuiAgremiacao(agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => dto[aux].ValorString!.Contains(c.Representante));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.FindAll(c => dto[aux].ValorString!.Contains(c.Representante));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => dto[aux].ValorString!.Contains(c.Representante));
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                            var and = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                            .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                agremiacoes = await PossuiAgremiacao(agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => dto[aux].ValorString!.Contains(c.Pais.Descricao));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.FindAll(c => dto[aux].ValorString!.Contains(c.Pais.Descricao));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => dto[aux].ValorString!.Contains(c.Pais.Descricao));
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                            var and = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                            .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                agremiacoes = await PossuiAgremiacao(agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => dto[aux].ValorString!.Contains(c.Cidade.Descricao));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.FindAll(c => dto[aux].ValorString!.Contains(c.Cidade.Descricao));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains =
                            agremiacoes.FindAll(c => dto[aux].ValorString!.Contains(c.Cidade.Descricao));
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                            var and = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                            .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                agremiacoes = await PossuiAgremiacao(agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => dto[aux].ValorString!.Contains(c.Estado.Descricao));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.FindAll(c => dto[aux].ValorString!.Contains(c.Estado.Descricao));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains =
                            agremiacoes.FindAll(c => dto[aux].ValorString!.Contains(c.Estado.Descricao));
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                            var and = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                            .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                agremiacoes = await PossuiAgremiacao(agremiacoes);
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => dto[aux].ValorString!.Contains(c.Regiao.Descricao));
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.FindAll(c => dto[aux].ValorString!.Contains(c.Regiao.Descricao));
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains =
                            agremiacoes.FindAll(c => dto[aux].ValorString!.Contains(c.Regiao.Descricao));
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MenorIgualQue
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var menor = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, menor, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorIgualQue =
                            agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                        return await Filtrar(dto, filtroMenorIgualQue, tamanho, ++aux);

                    //MaiorQue
                    case 6:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!)).ToList();
                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //MaiorIgualQue
                    case 7:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorIgualQue =
                            agremiacoes.Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                            var and = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await PossuiAgremiacao();
                            var or = agremiacaoLista.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                                .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.Take(GetIndex(agremiacoes, dto[aux].ValorString2!)).ToList()
                            .Skip(GetIndex(agremiacoes, dto[aux].ValorString!) + 1).ToList();
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
                agremiacoes = await PossuiAgremiacao(agremiacoes);
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                agremiacoes = await PossuiAgremiacao(agremiacoes);
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
                            var agremiacaoLista = await PossuiAgremiacao();
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
            return Mapper.Map<PagedDto<AgremiacaoFiltroDto>>(
                await _filtroRepository.Buscar(new BuscarAgremiacaoFiltroDto()));
        }

        return Mapper.Map<PagedDto<AgremiacaoFiltroDto>>(
            await _filtroRepository.Buscar(new BuscarAgremiacaoFiltroDto()));
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

    private async Task<List<Agremiacao>> PossuiAgremiacao(List<Agremiacao>? agremiacoes = null)
    {
        if (agremiacoes == null)
        {
            agremiacoes = await _agremiacaoRepository.ObterTodos();
        }

        return agremiacoes.OrderBy(c => c.Nome).ToList();
    }

    private int GetIndex(List<Agremiacao>? agremiacoes, string nome)
    {
        return agremiacoes!.FindIndex(c => c.Nome == nome);
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
}