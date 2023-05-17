using System.Text;
using AutoMapper;
using ClosedXML.Excel;
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
            if (pesquisa.OperadorLogico is 1 or null)
            {
                operadorLogico = "E";
            }
            else
            {
                operadorLogico = "OU";
            }

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
                        valor = pesquisa.DataInicial.ToString();
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
                        valor = pesquisa.DataInicial.ToString();
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
                        valor = pesquisa.DataInicial.ToString();
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
                        valor = pesquisa.DataInicial.ToString();
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
                        valor = pesquisa.DataInicial.ToString();
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
                        valor = pesquisa.DataInicial.ToString();
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
                        valor = pesquisa.DataInicial.ToString();
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
                        valor = pesquisa.DataInicial.ToString();
                        valor2 = pesquisa.DataFinal.ToString();
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
            FuncaoMenuId = null
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
            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                DataHoraEvento = DateTime.Now,
                ComputadorId = ObterIp(),
                Descricao =
                    $"Sigla={agremiacao.Sigla};Nome={agremiacao.Nome};Fantasia={agremiacao.Fantasia};Responsavel={agremiacao.Responsavel};Representante={agremiacao.Representante};DataFiliacao={agremiacao.DataFiliacao};DataNascimento={agremiacao.DataNascimento};Cep={agremiacao.Cep};Endereco={agremiacao.Endereco};Bairro={agremiacao.Bairro};Complemento={agremiacao.Complemento};Cidade={agremiacao.Cidade};Estado={agremiacao.Estado};Pais={agremiacao.Pais};Telefone={agremiacao.Telefone};Email={agremiacao.Email};Cnpj={agremiacao.Cnpj};InscricaoMunicipal={agremiacao.InscricaoMunicipal};InscricaoEstadual={agremiacao.InscricaoEstadual};DataCnpj={agremiacao.DataCnpj};DataAta={agremiacao.DataAta};Foto={agremiacao.Foto};AlvaraLocacao={agremiacao.AlvaraLocacao};Estatuto={agremiacao.Estatuto};ContratoSocial={agremiacao.ContratoSocial};DocumentacaoAtualizada={agremiacao.DocumentacaoAtualizada};Regiao={regiao?.Descricao};Anotacoes={agremiacao.Anotacoes};{nomeDoc};",
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

        var regiaoInicial = await _regiaoRepository.ObterPorId(agremiacao.IdRegiao);
        var agremiacaoInicial = agremiacao;
        await _agremiacaoRepository.UnitOfWork.Commit();

        Mapper.Map(dto, agremiacao);
        if (!await Validar(agremiacao))
        {
            return null;
        }

        var regiao = await _regiaoRepository.ObterPorId(agremiacao.IdRegiao);

        if (dto.Foto is { Length: > 0 } && !await ManterFoto(dto.Foto, agremiacao))
        {
            return null;
        }

        _agremiacaoRepository.Alterar(agremiacao);

        if (await _agremiacaoRepository.UnitOfWork.Commit())
        {
            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                DataHoraEvento = DateTime.Now,
                ComputadorId = ObterIp(),
                Descricao =
                    $"Sigla={agremiacaoInicial.Sigla};Sigla={agremiacao.Sigla};Nome={agremiacaoInicial.Nome};Nome={agremiacao.Nome};Fantasia={agremiacaoInicial.Fantasia};Fantasia={agremiacao.Fantasia};Responsavel={agremiacaoInicial.Responsavel};Responsavel={agremiacao.Responsavel};Representante={agremiacaoInicial.Representante};Representante={agremiacao.Representante};DataFiliacao={agremiacaoInicial.DataFiliacao};DataFiliacao={agremiacao.DataFiliacao};DataNascimento={agremiacaoInicial.DataNascimento};DataNascimento={agremiacao.DataNascimento};Cep={agremiacaoInicial.Cep};Cep={agremiacao.Cep};Endereco={agremiacaoInicial.Endereco};Endereco={agremiacao.Endereco};Bairro={agremiacaoInicial.Bairro};Bairro={agremiacao.Bairro};Complemento={agremiacaoInicial.Complemento};Complemento={agremiacao.Complemento};Cidade={agremiacaoInicial.Cidade};Cidade={agremiacao.Cidade};Estado={agremiacaoInicial.Estado};Estado={agremiacao.Estado};Pais={agremiacaoInicial.Pais};Pais={agremiacao.Pais};Telefone={agremiacaoInicial.Telefone};Telefone={agremiacao.Telefone};Email={agremiacaoInicial.Email};Email={agremiacao.Email};Cnpj={agremiacaoInicial.Cnpj};Cnpj={agremiacao.Cnpj};InscricaoMunicipal={agremiacaoInicial.InscricaoMunicipal};InscricaoMunicipal={agremiacao.InscricaoMunicipal};InscricaoEstadual={agremiacaoInicial.InscricaoEstadual};InscricaoEstadual={agremiacao.InscricaoEstadual};DataCnpj={agremiacaoInicial.DataCnpj};DataCnpj={agremiacao.DataCnpj};DataAta={agremiacaoInicial.DataAta};DataAta={agremiacao.DataAta};Foto={agremiacaoInicial.Foto};Foto={agremiacao.Foto};AlvaraLocacao={agremiacaoInicial.AlvaraLocacao};AlvaraLocacao={agremiacao.AlvaraLocacao};Estatuto={agremiacaoInicial.Estatuto};Estatuto={agremiacao.Estatuto};ContratoSocial={agremiacaoInicial.ContratoSocial};ContratoSocial={agremiacao.ContratoSocial};DocumentacaoAtualizada={agremiacaoInicial.DocumentacaoAtualizada};DocumentacaoAtualizada={agremiacao.DocumentacaoAtualizada};Regiao={regiaoInicial?.Descricao};Regiao={regiao?.Descricao};Anotacoes={agremiacaoInicial.Anotacoes};Anotacoes={agremiacao.Anotacoes};",
                ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
                TipoOperacaoId = 5,
                UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
                AdministradorNome = null,
                UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                AdministradorId = null,
                FuncaoMenuId = null
            });
            await _agremiacaoRepository.UnitOfWork.Commit();

            return Mapper.Map<AgremiacaoDto>(agremiacao);
        }

        Notificator.Handle("Não foi possível alterar a agremiação");
        return null;
    }

    #region Exportar

    public async Task<string> Exportar(ExportarAgremiacaoDto dto)
    {
        var descricao = new StringBuilder();
        descricao.Append("Colounas=");
        var linha = 2;
        var agremiacoes = await _filtroRepository.Listar();
        var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("CADAGRE" + DateTime.Now.ToString("yyyy/MM/DD HH:mm:ss"));

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
                ws.Cell(linha, contador).Value = agremiacao.Fantasia;
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
                descricao.Append("DataFiliacao;");
                ws.Cell(1, contador).Value = "DataFiliacao";
                ws.Cell(linha, contador).Value = agremiacao.DataFiliacao.ToString("DD/MM/YYY");
                contador++;
            }

            if (dto.DataNascimento)
            {
                descricao.Append("DataNascimento;");
                ws.Cell(1, contador).Value = "DataNascimento";
                ws.Cell(linha, contador).Value = agremiacao.DataNascimento.ToString("DD/MM/YYY");
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

            if (dto.Endereco)
            {
                descricao.Append("Endereco;");
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

            if (dto.IdCidade)
            {
                descricao.Append("IdCidade;");
                ws.Cell(1, contador).Value = "Cidade";
                ws.Cell(linha, contador).Value = agremiacao.Cidade;
                contador++;
            }

            if (dto.IdEstado)
            {
                descricao.Append("IdEstado;");
                ws.Cell(1, contador).Value = "Estado";
                ws.Cell(linha, contador).Value = agremiacao.Estado;
                contador++;
            }

            if (dto.IdRegiao)
            {
                descricao.Append("IdRegiao;");
                ws.Cell(1, contador).Value = "Regiao";
                ws.Cell(linha, contador).Value = agremiacao.RegiaoNome;
                contador++;
            }

            if (dto.IdPais)
            {
                descricao.Append("IdPais;");
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
            FuncaoMenuId = null
        });

        await RegistroDeEventos.UnitOfWork.Commit();
        return await _fileService.UploadExcel(workbook, EUploadPath.FotosAgremiacao);
    }

    #endregion

    public async Task<PagedDto<AgremiacaoDto>> Buscar(BuscarAgremiacaoDto dto)
    {
        var agremiacao = await _agremiacaoRepository.Buscar(dto);
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
            FuncaoMenuId = null
        });

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
            FuncaoMenuId = null
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
            FuncaoMenuId = 8
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
            FuncaoMenuId = null
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
                Descricao = "Adicionar anotacoes em agremiacao",
                ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
                TipoOperacaoId = 11,
                UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
                AdministradorNome = null,
                UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                AdministradorId = null,
                FuncaoMenuId = null
            });
            await RegistroDeEventos.UnitOfWork.Commit();
            return;
        }

        Notificator.Handle("Não foi possível alterar anotação");
    }

    public async Task EnviarDocumentos(int id, EnviarDocumentosDto dto)
    {
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
                FuncaoMenuId = null
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
                FuncaoMenuId = null
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