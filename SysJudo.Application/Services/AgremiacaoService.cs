using System.Text;
using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Math;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Agremiacao;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Notifications;
using SysJudo.Core.Enums;
using SysJudo.Core.Extension;
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
    private readonly HttpContextAccessor _httpContextAccessor;
    private readonly IRegiaoRepository _regiaoRepository;

    public AgremiacaoService(IMapper mapper, INotificator notificator, IAgremiacaoRepository agremiacaoRepository,
        IFileService fileService, IAgremiacaoFiltroRepository filtroRepository,
        IRegistroDeEventoRepository registroDeEventoRepository, IOptions<HttpContextAccessor> httpContextAccessor,
        IRegiaoRepository regiaoRepository) :
        base(mapper, notificator, registroDeEventoRepository)
    {
        _regiaoRepository = regiaoRepository;
        _httpContextAccessor = httpContextAccessor.Value;
        _agremiacaoRepository = agremiacaoRepository;
        _fileService = fileService;
        _filtroRepository = filtroRepository;
    }

    public async Task<List<AgremiacaoFiltroDto>> Filtrar(List<FiltragemAgremiacaoDto> dto,
        List<Agremiacao>? agremiacoes = null!, int tamanho = 0, int aux = 0)
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
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Pais != dto[aux].ValorString);
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
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Cidade != dto[aux].ValorString);
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
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Estado != dto[aux].ValorString);
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
            agremiacao.Pais = agremiacoes.FirstOrDefault(c => c.Id == agremiacao.Id)!.Pais;
            agremiacao.Estado =
                agremiacoes.FirstOrDefault(c => c.Id == agremiacao.Id)!.Estado;
            agremiacao.Cidade =
                agremiacoes.FirstOrDefault(c => c.Id == agremiacao.Id)!.Cidade;
            agremiacao.RegiaoNome =
                agremiacoes.FirstOrDefault(c => c.IdRegiao == agremiacao.IdRegiao)!.Regiao.Descricao;

            _filtroRepository.Cadastrar(agremiacao);
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

        var nomeDoc = new StringBuilder();
        nomeDoc.Append("Documentos anexados=");
        var links = new StringBuilder();
        if (dto.Documentos != null)
        {
            foreach (var documento in dto.Documentos)
            {
                if (documento is { Length: > 0 })
                {
                    links.Append(agremiacao.DocumentosUri + "&" +
                                 await _fileService.Upload(documento, EUploadPath.FotosAgremiacao));
                    nomeDoc.Append($"{documento.FileName};");
                }
            }
        }

        agremiacao.DocumentosUri = links.ToString();

        if (dto.Foto is { Length: > 0 })
        {
            agremiacao.Foto = await _fileService.Upload(dto.Foto, EUploadPath.FotosAgremiacao);
        }

        _agremiacaoRepository.Cadastrar(agremiacao);
        var regiao = await _regiaoRepository.ObterPorId(agremiacao.IdRegiao);
        if (await _agremiacaoRepository.UnitOfWork.Commit())
        {
            var dataCnpj = "NULL";
            if (agremiacao.DataCnpj != null)
            {
                dataCnpj = new DateTime(agremiacao.DataCnpj.Value.Year, agremiacao.DataCnpj.Value.Month,
                    agremiacao.DataCnpj.Value.Day).ToString("dd/MM/yyyy");
            }

            var dataAta = "NULL";
            if (agremiacao.DataAta != null)
            {
                dataAta = new DateTime(agremiacao.DataAta.Value.Year, agremiacao.DataAta.Value.Month,
                    agremiacao.DataAta.Value.Day).ToString("dd/MM/yyyy");
            }

            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                DataHoraEvento = DateTime.Now,
                ComputadorId = ObterIp(),
                Descricao =
                    $"Sigla={agremiacao.Sigla};Nome={agremiacao.Nome};Fantasia={agremiacao.Fantasia};Responsavel={agremiacao.Responsavel};Representante={agremiacao.Representante};Data de filiacao={new DateTime(agremiacao.DataFiliacao.Year, agremiacao.DataFiliacao.Month, agremiacao.DataFiliacao.Day):dd/MM/yyyy};Data de nascimento={new DateTime(agremiacao.DataNascimento.Year, agremiacao.DataNascimento.Month, agremiacao.DataNascimento.Day):dd/MM/yyyy};Cep={agremiacao.Cep};Endereco={agremiacao.Endereco};Bairro={agremiacao.Bairro};Complemento={agremiacao.Complemento};Cidade={agremiacao.Cidade};Estado={agremiacao.Estado};Pais={agremiacao.Pais};Telefone={agremiacao.Telefone};Email={agremiacao.Email};Cnpj={agremiacao.Cnpj};Inscricao municipal={agremiacao.InscricaoMunicipal};Inscricao estadual={agremiacao.InscricaoEstadual};Data Cnpj={dataCnpj};Data Ata={dataAta};Foto={agremiacao.Foto};Alvara de locacao={agremiacao.AlvaraLocacao};Estatuto={agremiacao.Estatuto};Contrato social={agremiacao.ContratoSocial};Documentacao atualizada={agremiacao.DocumentacaoAtualizada};Regiao={regiao?.Descricao};Anotacoes={agremiacao.Anotacoes};{nomeDoc};",
                ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
                TipoOperacaoId = 4,
                UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
                AdministradorNome = null,
                UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                AdministradorId = null,
                FuncaoMenuId = 2
            });
            await RegistroDeEventos.UnitOfWork.Commit();
            return Mapper.Map<AgremiacaoDto>(agremiacao);
        }

        Notificator.Handle("Não foi possível cadastrar a agremiação");
        return null;
    }

    public async Task<AgremiacaoDto?> Alterar(int id, AlterarAgremiacaoDto dto)
    {
        var caminhoFoto = "";
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

        var agremiacaoInicial = await _agremiacaoRepository.Obter(id);
        var regiaoInicial = await _regiaoRepository.ObterPorId(agremiacao.IdRegiao);
        var regiao = await _regiaoRepository.ObterPorId(dto.IdRegiao);

        if (dto.Foto is { Length: > 0 })
        {
            caminhoFoto = await ManterFoto(dto.Foto);
        }

        RegistroDeEventos.Adicionar(new RegistroDeEvento
        {
            DataHoraEvento = DateTime.Now,
            ComputadorId = ObterIp(),
            Descricao =
                $"Sigla={agremiacaoInicial!.Sigla};Nome={agremiacaoInicial.Nome};Fantasia={agremiacaoInicial.Fantasia};Responsavel={agremiacaoInicial.Responsavel};Representante={agremiacaoInicial.Representante};DataFiliacao={agremiacaoInicial.DataFiliacao};DataNascimento={agremiacaoInicial.DataNascimento};Cep={agremiacaoInicial.Cep};Endereco={agremiacaoInicial.Endereco};Bairro={agremiacaoInicial.Bairro};Complemento={agremiacaoInicial.Complemento};Cidade={agremiacaoInicial.Cidade};Estado={agremiacaoInicial.Estado};Pais={agremiacaoInicial.Pais};Telefone={agremiacaoInicial.Telefone};Email={agremiacaoInicial.Email};Cnpj={agremiacaoInicial.Cnpj};InscricaoMunicipal={agremiacaoInicial.InscricaoMunicipal};InscricaoEstadual={agremiacaoInicial.InscricaoEstadual};DataCnpj={agremiacaoInicial.DataCnpj};DataAta={agremiacaoInicial.DataAta};Foto={agremiacaoInicial.Foto};AlvaraLocacao={agremiacaoInicial.AlvaraLocacao};Estatuto={agremiacaoInicial.Estatuto};ContratoSocial={agremiacaoInicial.ContratoSocial};DocumentacaoAtualizada={agremiacaoInicial.DocumentacaoAtualizada};Regiao={regiaoInicial?.Descricao};Anotacoes={agremiacaoInicial.Anotacoes};<br>" +
                $"Sigla={dto.Sigla};Nome={dto.Nome};Fantasia={dto.Fantasia};Responsavel={dto.Responsavel};Representante={dto.Representante};DataFiliacao={dto.DataFiliacao};DataNascimento={dto.DataNascimento};Cep={dto.Cep};Endereco={dto.Endereco};Bairro={dto.Bairro};Complemento={dto.Complemento};Cidade={dto.Cidade};Estado={dto.Estado};Pais={dto.Pais};Telefone={dto.Telefone};Email={dto.Email};Cnpj={dto.Cnpj};InscricaoMunicipal={dto.InscricaoMunicipal};InscricaoEstadual={dto.InscricaoEstadual};DataCnpj={dto.DataCnpj};DataAta={dto.DataAta};Foto={caminhoFoto};AlvaraLocacao={dto.AlvaraLocacao};Estatuto={dto.Estatuto};ContratoSocial={dto.ContratoSocial};DocumentacaoAtualizada={dto.DocumentacaoAtualizada};Regiao={regiao?.Descricao};Anotacoes={dto.Anotacoes}",
            ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
            TipoOperacaoId = 5,
            UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
            AdministradorNome = null,
            UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
            AdministradorId = null,
            FuncaoMenuId = 2
        });

        Mapper.Map(dto, agremiacao);
        if (!await Validar(agremiacao))
        {
            return null;
        }

        agremiacao.Foto = caminhoFoto;
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
        var descricao = new StringBuilder();
        descricao.Append("Colunas= ");
        var linha = 2;
        var agremiacoes = await _filtroRepository.Listar();
        var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("CADAGRE" + DateTime.Now.ToString("yyyyMMDDHHmmss"));

        #region Ordenacao

        if (dto.Ordenacao != null)
        {
            if (dto.Ordenacao.Propriedade == "Nome")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Nome, obj2.Nome, StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Nome, obj1.Nome, StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Sigla")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Nome, obj2.Nome, StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Nome, obj1.Nome, StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Fantasia")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Nome, obj2.Nome, StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Nome, obj1.Nome, StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Responsavel")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Nome, obj2.Nome, StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Nome, obj1.Nome, StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Representante")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Nome, obj2.Nome, StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Nome, obj1.Nome, StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "DataFiliacao")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Nome, obj2.Nome, StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Nome, obj1.Nome, StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "DataNascimento")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Nome, obj2.Nome, StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Nome, obj1.Nome, StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Cep")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Nome, obj2.Nome, StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Nome, obj1.Nome, StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Endereco")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Nome, obj2.Nome, StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Nome, obj1.Nome, StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Bairro")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Nome, obj2.Nome, StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Nome, obj1.Nome, StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Complemento")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Nome, obj2.Nome, StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Nome, obj1.Nome, StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "IdCidade")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Nome, obj2.Nome, StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Nome, obj1.Nome, StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "IdEstado")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Nome, obj2.Nome, StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Nome, obj1.Nome, StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "IdRegiao")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Nome, obj2.Nome, StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Nome, obj1.Nome, StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "IdPais")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Nome, obj2.Nome, StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Nome, obj1.Nome, StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Telefone")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Nome, obj2.Nome, StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Nome, obj1.Nome, StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Email")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Nome, obj2.Nome, StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Nome, obj1.Nome, StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Cnpj")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Nome, obj2.Nome, StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Nome, obj1.Nome, StringComparison.Ordinal));
                }
            }
        }

        #endregion

        foreach (var agremiacao in agremiacoes)
        {
            var contador = 1;
            if (dto.Nome)
            {
                descricao.Append("Nome;");
                ws.Cell(1, contador).Value = "Nome";
                ws.Cell(linha, contador).Value = agremiacao.Nome;
                contador++;
            }

            if (dto.Sigla)
            {
                descricao.Append("Sigla;");
                ws.Cell(1, contador).Value = "Sigla";
                ws.Cell(linha, contador).Value = agremiacao.Sigla;
                contador++;
            }

            if (dto.Fantasia)
            {
                descricao.Append("Fantasia;");
                ws.Cell(1, contador).Value = "Fantasia";
                ws.Cell(linha, contador).Value = agremiacao.Fantasia == null ? "NULL" : agremiacao.Fantasia;
                contador++;
            }

            if (dto.Responsavel)
            {
                descricao.Append("Responsavel;");
                ws.Cell(1, contador).Value = "Responsavel";
                ws.Cell(linha, contador).Value = agremiacao.Responsavel;
                contador++;
            }

            if (dto.Representante)
            {
                descricao.Append("Representante;");
                ws.Cell(1, contador).Value = "Representante";
                ws.Cell(linha, contador).Value = agremiacao.Representante;
                contador++;
            }

            if (dto.DataFiliacao)
            {
                descricao.Append("Data Filiacao;");
                ws.Cell(1, contador).Value = "Data Filiacao";
                ws.Cell(linha, contador).Value = new DateTime(agremiacao.DataFiliacao.Year,
                    agremiacao.DataFiliacao.Month,
                    agremiacao.DataFiliacao.Day).ToString("dd/MM/yyyy");
                contador++;
            }

            if (dto.DataNascimento)
            {
                descricao.Append("Data Nascimento;");
                ws.Cell(1, contador).Value = "Data Nascimento";
                ws.Cell(linha, contador).Value = new DateTime(agremiacao.DataNascimento.Year,
                    agremiacao.DataNascimento.Month,
                    agremiacao.DataNascimento.Day).ToString("dd/MM/yyyy");
                contador++;
            }

            if (dto.Cep)
            {
                descricao.Append("Cep;");
                ws.Cell(1, contador).Value = "Cep";
                ws.Cell(linha, contador).Value = agremiacao.Cep;
                contador++;
            }

            if (dto.Endereco)
            {
                descricao.Append("Endereco;");
                ws.Cell(1, contador).Value = "Endereco";
                ws.Cell(linha, contador).Value = agremiacao.Endereco;
                contador++;
            }

            if (dto.Bairro)
            {
                descricao.Append("Bairro;");
                ws.Cell(1, contador).Value = "Bairro";
                ws.Cell(linha, contador).Value = agremiacao.Endereco;
                contador++;
            }

            if (dto.Complemento)
            {
                descricao.Append("Complemento;");
                ws.Cell(1, contador).Value = "Complemento";
                ws.Cell(linha, contador).Value = agremiacao.Complemento;
                contador++;
            }

            if (dto.Cidade)
            {
                descricao.Append("Cidade;");
                ws.Cell(1, contador).Value = "Cidade";
                ws.Cell(linha, contador).Value = agremiacao.Cidade;
                contador++;
            }

            if (dto.Estado)
            {
                descricao.Append("Estado;");
                ws.Cell(1, contador).Value = "Estado";
                ws.Cell(linha, contador).Value = agremiacao.Estado;
                contador++;
            }

            if (dto.IdRegiao)
            {
                descricao.Append("Regiao;");
                ws.Cell(1, contador).Value = "Regiao";
                ws.Cell(linha, contador).Value = agremiacao.RegiaoNome;
                contador++;
            }

            if (dto.Pais)
            {
                descricao.Append("Pais;");
                ws.Cell(1, contador).Value = "Pais";
                ws.Cell(linha, contador).Value = agremiacao.Pais;
                contador++;
            }

            if (dto.Telefone)
            {
                descricao.Append("Telefone;");
                ws.Cell(1, contador).Value = "Telefone";
                ws.Cell(linha, contador).Value = agremiacao.Telefone;
                contador++;
            }

            if (dto.Email)
            {
                descricao.Append("Email;");
                ws.Cell(1, contador).Value = "Email";
                ws.Cell(linha, contador).Value = agremiacao.Email;
                contador++;
            }

            if (dto.InscricaoMunicipal)
            {
                descricao.Append("Inscricao Municipal;");
                ws.Cell(1, contador).Value = "Inscricao Municipal";
                ws.Cell(linha, contador).Value =
                    agremiacao.InscricaoMunicipal == null ? "NULL" : agremiacao.InscricaoMunicipal;
                contador++;
            }

            if (dto.InscricaoEstadual)
            {
                descricao.Append("Inscricao Estadual;");
                ws.Cell(1, contador).Value = "Inscricao Estadual";
                ws.Cell(linha, contador).Value =
                    agremiacao.InscricaoEstadual == null ? "NULL" : agremiacao.InscricaoEstadual;
                contador++;
            }

            if (dto.Nome)
            {
                descricao.Append("Nome;");
                ws.Cell(1, contador).Value = "Nome";
                ws.Cell(linha, contador).Value = agremiacao.Nome;
                contador++;
            }

            if (dto.DataCnpj)
            {
                descricao.Append("Data Cnpj;");
                ws.Cell(1, contador).Value = "Data Cnpj";
                ws.Cell(linha, contador).Value = agremiacao.DataCnpj == null
                    ? "NULL"
                    : new DateTime(agremiacao.DataCnpj.Value.Year,
                        agremiacao.DataCnpj.Value.Month,
                        agremiacao.DataCnpj.Value.Day).ToString("dd/MM/yyyy");
                contador++;
            }

            if (dto.DataAta)
            {
                descricao.Append("Data Ata;");
                ws.Cell(1, contador).Value = "Data Ata";
                ws.Cell(linha, contador).Value = agremiacao.DataAta == null
                    ? "NULL"
                    : new DateTime(agremiacao.DataAta.Value.Year,
                        agremiacao.DataAta.Value.Month,
                        agremiacao.DataAta.Value.Day).ToString("dd/MM/yyyy");
                contador++;
            }

            if (dto.Foto)
            {
                descricao.Append("Foto;");
                ws.Cell(1, contador).Value = "Foto";
                ws.Cell(linha, contador).Value = agremiacao.Foto == null ? "NULL" : agremiacao.Foto;
                contador++;
            }

            if (dto.AlvaraLocacao)
            {
                descricao.Append("Alvara Locacao;");
                ws.Cell(1, contador).Value = "Alvara Locacao";
                ws.Cell(linha, contador).Value = agremiacao.AlvaraLocacao;
                contador++;
            }

            if (dto.Estatuto)
            {
                descricao.Append("Estatuto;");
                ws.Cell(1, contador).Value = "Estatuto";
                ws.Cell(linha, contador).Value = agremiacao.Estatuto;
                contador++;
            }

            if (dto.ContratoSocial)
            {
                descricao.Append("Contrato Social;");
                ws.Cell(1, contador).Value = "Contrato Social";
                ws.Cell(linha, contador).Value = agremiacao.ContratoSocial;
                contador++;
            }

            if (dto.DocumentacaoAtualizada)
            {
                descricao.Append("Documentacao Atualizada;");
                ws.Cell(1, contador).Value = "Documentacao Atualizada";
                ws.Cell(linha, contador).Value = agremiacao.DocumentacaoAtualizada;
                contador++;
            }

            if (dto.Anotacoes)
            {
                descricao.Append("Anotacoes;");
                ws.Cell(1, contador).Value = "Anotacoes";
                ws.Cell(linha, contador).Value = agremiacao.Anotacoes == null ? "NULL" : agremiacao.Anotacoes;
                contador++;
            }

            if (dto.Cnpj)
            {
                descricao.Append("Cnpj;");
                ws.Cell(1, contador).Value = "Cnpj";
                ws.Cell(linha, contador).Value = agremiacao.Cnpj;
            }

            linha++;
        }

        ws.Columns().AdjustToContents();

        RegistroDeEventos.Adicionar(new RegistroDeEvento
        {
            DataHoraEvento = DateTime.Now,
            ComputadorId = ObterIp(),
            Descricao = $"{descricao}",
            ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
            TipoOperacaoId = 10,
            UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
            AdministradorId = null,
            FuncaoMenuId = 2
        });

        await RegistroDeEventos.UnitOfWork.Commit();
        return await _fileService.UploadExcel(workbook, EUploadPath.FotosAgremiacao);
    }

    #endregion

    public async Task<PagedDto<AgremiacaoDto>> Buscar(BuscarAgremiacaoDto dto)
    {
        var agremiacao = await _agremiacaoRepository.Buscar(dto);
        await RegistroDeEventos.UnitOfWork.Commit();
        return Mapper.Map<PagedDto<AgremiacaoDto>>(agremiacao);
    }

    public async Task<List<AgremiacaoFiltroDto>> Pesquisar(string valor)
    {
        var agremiacoes = await _filtroRepository.Pesquisar(valor);

        if (agremiacoes.Count == 0)
        {
            return new List<AgremiacaoFiltroDto>();
        }

        await _filtroRepository.RemoverTodos();
        foreach (var agremiacao in agremiacoes.DistinctBy(c => c.Id))
        {
            _filtroRepository.Cadastrar(agremiacao);
        }

        RegistroDeEventos.Adicionar(new RegistroDeEvento
        {
            DataHoraEvento = DateTime.Now,
            ComputadorId = ObterIp(),
            Descricao = $"Valor={valor}",
            ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
            TipoOperacaoId = 14,
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
        RegistroDeEventos.Adicionar(new RegistroDeEvento
        {
            DataHoraEvento = DateTime.Now,
            ComputadorId = ObterIp(),
            Descricao = "Visualizar agremiacao",
            ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
            TipoOperacaoId = 7,
            UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
            AdministradorNome = null,
            UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
            AdministradorId = null,
            FuncaoMenuId = 2
        });

        await RegistroDeEventos.UnitOfWork.Commit();

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
            agremiacao.Pais = agremiacoes.FirstOrDefault(c => c.Id == agremiacao.Id)!.Pais;
            agremiacao.Estado =
                agremiacoes.FirstOrDefault(c => c.Id == agremiacao.Id)!.Estado;
            agremiacao.Cidade =
                agremiacoes.FirstOrDefault(c => c.Id == agremiacao.Id)!.Cidade;
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
        var agremiacao = await _agremiacaoRepository.Obter(id);
        if (agremiacao == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        var regiao = await _regiaoRepository.ObterPorId(agremiacao.IdRegiao);
        RegistroDeEventos.Adicionar(new RegistroDeEvento
        {
            DataHoraEvento = DateTime.Now,
            ComputadorId = ObterIp(),
            Descricao =
                $"Sigla={agremiacao.Sigla};Nome={agremiacao.Nome};Fantasia={agremiacao.Fantasia};Responsavel={agremiacao.Responsavel};Representante={agremiacao.Representante};DataFiliacao={agremiacao.DataFiliacao};DataNascimento={agremiacao.DataNascimento};Cep={agremiacao.Cep};Endereco={agremiacao.Endereco};Bairro={agremiacao.Bairro};Complemento={agremiacao.Complemento};Cidade={agremiacao.Cidade};Estado={agremiacao.Estado};Pais={agremiacao.Pais};Telefone={agremiacao.Telefone};Email={agremiacao.Email};Cnpj={agremiacao.Cnpj};InscricaoMunicipal={agremiacao.InscricaoMunicipal};InscricaoEstadual={agremiacao.InscricaoEstadual};DataCnpj={agremiacao.DataCnpj};DataAta={agremiacao.DataAta};Foto={agremiacao.Foto};AlvaraLocacao={agremiacao.AlvaraLocacao};Estatuto={agremiacao.Estatuto};ContratoSocial={agremiacao.ContratoSocial};DocumentacaoAtualizada={agremiacao.DocumentacaoAtualizada};Regiao={regiao?.Descricao};Anotacoes={agremiacao.Anotacoes};",
            ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
            TipoOperacaoId = 6,
            UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
            AdministradorNome = null,
            UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
            AdministradorId = null,
            FuncaoMenuId = 2
        });
        await RegistroDeEventos.UnitOfWork.Commit();
        _agremiacaoRepository.Deletar(agremiacao);
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
        if (await _agremiacaoRepository.UnitOfWork.Commit())
        {
            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                DataHoraEvento = DateTime.Now,
                ComputadorId = ObterIp(),
                Descricao = $"Adicionar anotacoes em agremiacao: {agremiacao.Anotacoes}",
                ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
                TipoOperacaoId = 11,
                UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
                AdministradorNome = null,
                UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                AdministradorId = null,
                FuncaoMenuId = 2
            });
            await RegistroDeEventos.UnitOfWork.Commit();
            return;
        }

        Notificator.Handle("Não foi possível alterar anotação");
    }

    public async Task EnviarDocumentos(int id, EnviarDocumentosDto dto)
    {
        if (!dto.Documentos.Any())
        {
            Notificator.Handle("Nenhum arquivo foi informado");
            return;
        }
        
        var agremiacao = await _agremiacaoRepository.Obter(id);
        if (agremiacao == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        var nome = new StringBuilder();
        nome.Append("Documento anexado = ");
        StringBuilder links = new StringBuilder();
        foreach (var documento in dto.Documentos)
        {
            if (documento is { Length: > 0 })
            {
                links.Append("&" +
                             await _fileService.Upload(documento, EUploadPath.FotosAgremiacao));
                nome.Append($"{documento.FileName}; ");
            }
        }

        agremiacao.DocumentosUri += links.ToString();
        _agremiacaoRepository.Alterar(agremiacao);
        if (await _agremiacaoRepository.UnitOfWork.Commit())
        {
            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                Descricao = nome.ToString(),
                DataHoraEvento = DateTime.Now,
                ComputadorId = ObterIp(),
                ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
                TipoOperacaoId = 8,
                UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
                AdministradorNome = null,
                UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                AdministradorId = null,
                FuncaoMenuId = 2
            });

            await RegistroDeEventos.UnitOfWork.Commit();
            return;
        }

        Notificator.Handle("Não foi possível enviar documentos.");
    }

    public async Task DeletarDocumento(int id, int documentoId)
    {
        var agremiacao = await _agremiacaoRepository.Obter(id);

        if (agremiacao is null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        if (agremiacao.DocumentosUri == "&" || agremiacao.DocumentosUri == string.Empty ||
            agremiacao.DocumentosUri == null)
        {
            Notificator.Handle("Não há anexos ou documento não existe.");
            return;
        }

        var nome = new StringBuilder();
        nome.Append("Documento desanexado=");
        var documentos = agremiacao.DocumentosUri.Split('&').ToList();
        var remover = documentos[documentoId];
        nome.Append($"{remover};");
        documentos.Remove(remover);
        remover = documentos[0];
        documentos.Remove(remover);
        StringBuilder links = new StringBuilder();
        for (int i = 0; i <= documentos.Count - 1; i++)
        {
            links.Append("&" + documentos[i]);
        }

        agremiacao.DocumentosUri = links.ToString();
        _agremiacaoRepository.Alterar(agremiacao);
        if (await _agremiacaoRepository.UnitOfWork.Commit())
        {
            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                DataHoraEvento = DateTime.Now,
                ComputadorId = ObterIp(),
                Descricao = $"{nome}",
                ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
                TipoOperacaoId = 6,
                UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                AdministradorId = null,
                UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
                AdministradorNome = null,
                FuncaoMenuId = 2
            });
            return;
        }

        Notificator.Handle("Não foi possível remover documentos.");
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
            case "Pais": return agremiacoes.OrderBy(c => c.Pais).ToList();
            case "Estado": return agremiacoes.OrderBy(c => c.Estado).ToList();
            case "Cidade": return agremiacoes.OrderBy(c => c.Cidade).ToList();
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

    private async Task<string> ManterFoto(IFormFile foto)
    {
        var caminho = await _fileService.Upload(foto, EUploadPath.FotosAgremiacao);
        return caminho;
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