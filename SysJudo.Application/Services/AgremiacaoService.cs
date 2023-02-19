﻿using System.Text;
using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
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

    public async Task<PagedDto<AgremiacaoDto>> Filtrar(List<FiltragemAgremiacaoDto> dto,
        List<Agremiacao> agremiacoes = null, int tamanho = 0, int aux = 0)
    {
        tamanho = dto.Count;
        if (agremiacoes == null)
        {
            agremiacoes = await _agremiacaoRepository.ObterTodos();
        }

        if (aux < tamanho)
        {
            #region Sigla

            if (dto[aux].NomeParametro == "Sigla")
            {
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Sigla == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Sigla == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Sigla == dto[aux].ValorString);
                        if (filtroContains.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Diferente
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Sigla != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Sigla != dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Sigla != dto[aux].ValorString);
                        if (filtroDiferente.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);
                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Nome

            if (dto[aux].NomeParametro == "Nome")
            {
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Nome == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Nome == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Nome == dto[aux].ValorString);
                        if (filtroContains.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Diferente
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Nome != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Nome != dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Nome != dto[aux].ValorString);
                        if (filtroDiferente.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);
                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Responsavel

            if (dto[aux].NomeParametro == "Responsavel")
            {
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Responsavel == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Responsavel == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Responsavel == dto[aux].ValorString);
                        if (filtroContains.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Diferente
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Responsavel != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Responsavel != dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Responsavel != dto[aux].ValorString);
                        if (filtroDiferente.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);
                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Cep

            if (dto[aux].NomeParametro == "Cep")
            {
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Cep == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Cep == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Cep == dto[aux].ValorString);
                        if (filtroContains.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Diferente
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Cep != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Cep != dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Cep != dto[aux].ValorString);
                        if (filtroDiferente.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);
                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Endereco

            if (dto[aux].NomeParametro == "Endereco")
            {
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Endereco == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Endereco == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Endereco == dto[aux].ValorString);
                        if (filtroContains.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Diferente
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Endereco != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Endereco != dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Endereco != dto[aux].ValorString);
                        if (filtroDiferente.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);
                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Bairro

            if (dto[aux].NomeParametro == "Bairro")
            {
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Bairro == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Bairro == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Bairro == dto[aux].ValorString);
                        if (filtroContains.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Diferente
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Bairro != dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Bairro != dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.Bairro != dto[aux].ValorString);
                        if (filtroDiferente.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);
                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Complemento

            if (dto[aux].NomeParametro == "Complemento")
            {
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Complemento == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Complemento == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Complemento == dto[aux].ValorString);
                        if (filtroContains.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Diferente
                    case 2:
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
                        if (filtroDiferente.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);
                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Telefone

            if (dto[aux].NomeParametro == "Telefone")
            {
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Telefone == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Telefone == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Telefone == dto[aux].ValorString);
                        if (filtroContains.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Diferente
                    case 2:
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
                        if (filtroDiferente.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);
                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Email

            if (dto[aux].NomeParametro == "Email")
            {
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Email == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Email == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Email == dto[aux].ValorString);
                        if (filtroContains.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Diferente
                    case 2:
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
                        if (filtroDiferente.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);
                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Cnpj

            if (dto[aux].NomeParametro == "Cnpj")
            {
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Cnpj == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Cnpj == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Cnpj == dto[aux].ValorString);
                        if (filtroContains.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Diferente
                    case 2:
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
                        if (filtroDiferente.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);
                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region Representante

            if (dto[aux].NomeParametro == "Representante")
            {
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.Representante == dto[aux].ValorString);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.Representante == dto[aux].ValorString);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.Representante == dto[aux].ValorString);
                        if (filtroContains.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Diferente
                    case 2:
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
                        if (filtroDiferente.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);
                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region IdPais

            if (dto[aux].NomeParametro == "IdPais")
            {
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.IdPais == dto[aux].ValorId1);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.IdPais == dto[aux].ValorId1);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.IdPais == dto[aux].ValorId1);
                        if (filtroContains.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Diferente
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.IdPais != dto[aux].ValorId1);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.IdPais != dto[aux].ValorId1);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.IdPais != dto[aux].ValorId1);
                        if (filtroDiferente.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c => c.IdPais < dto[aux].ValorId1);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.IdPais < dto[aux].ValorId1);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.FindAll(c => c.IdPais < dto[aux].ValorId1);
                        if (filtroMenorQue.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MaiorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c => c.IdPais > dto[aux].ValorId1);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.IdPais > dto[aux].ValorId1);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.FindAll(c => c.IdPais > dto[aux].ValorId1);
                        if (filtroMaiorQue.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //Entre
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c =>
                                c.IdPais < dto[aux].ValorId1 && c.IdPais > dto[aux].ValorId2);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c =>
                                c.IdPais < dto[aux].ValorId1 && c.IdPais > dto[aux].ValorId2);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.FindAll(c =>
                            c.IdPais < dto[aux].ValorId1 && c.IdPais > dto[aux].ValorId2);
                        if (filtroEntre.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);
                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region IdCidade

            if (dto[aux].NomeParametro == "IdCidade")
            {
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.IdCidade == dto[aux].ValorId1);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.IdCidade == dto[aux].ValorId1);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.IdCidade == dto[aux].ValorId1);
                        if (filtroContains.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Diferente
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.IdCidade != dto[aux].ValorId1);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.IdCidade != dto[aux].ValorId1);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.IdCidade != dto[aux].ValorId1);
                        if (filtroDiferente.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c => c.IdCidade < dto[aux].ValorId1);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.IdCidade < dto[aux].ValorId1);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.FindAll(c => c.IdCidade < dto[aux].ValorId1);
                        if (filtroMenorQue.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MaiorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c => c.IdCidade > dto[aux].ValorId1);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.IdCidade > dto[aux].ValorId1);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.FindAll(c => c.IdCidade > dto[aux].ValorId1);
                        if (filtroMaiorQue.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //Entre
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c =>
                                c.IdCidade < dto[aux].ValorId1 && c.IdCidade > dto[aux].ValorId2);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c =>
                                c.IdCidade < dto[aux].ValorId1 && c.IdCidade > dto[aux].ValorId2);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.FindAll(c =>
                            c.IdCidade < dto[aux].ValorId1 && c.IdCidade > dto[aux].ValorId2);
                        if (filtroEntre.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);
                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region IdEstado

            if (dto[aux].NomeParametro == "IdEstado")
            {
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.IdEstado == dto[aux].ValorId1);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.IdEstado == dto[aux].ValorId1);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.IdEstado == dto[aux].ValorId1);
                        if (filtroContains.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Diferente
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.IdEstado != dto[aux].ValorId1);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.IdEstado != dto[aux].ValorId1);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.IdEstado != dto[aux].ValorId1);
                        if (filtroDiferente.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c => c.IdEstado < dto[aux].ValorId1);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.IdEstado < dto[aux].ValorId1);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.FindAll(c => c.IdEstado < dto[aux].ValorId1);
                        if (filtroMenorQue.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MaiorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c => c.IdEstado > dto[aux].ValorId1);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.IdEstado > dto[aux].ValorId1);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.FindAll(c => c.IdEstado > dto[aux].ValorId1);
                        if (filtroMaiorQue.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //Entre
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c =>
                                c.IdEstado < dto[aux].ValorId1 && c.IdEstado > dto[aux].ValorId2);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c =>
                                c.IdEstado < dto[aux].ValorId1 && c.IdEstado > dto[aux].ValorId2);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.FindAll(c =>
                            c.IdEstado < dto[aux].ValorId1 && c.IdEstado > dto[aux].ValorId2);
                        if (filtroEntre.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroEntre, tamanho, ++aux);
                    default:
                        Notificator.Handle("Operação inválida");
                        break;
                }
            }

            #endregion

            #region IdRegiao

            if (dto[aux].NomeParametro == "IdRegiao")
            {
                switch (dto[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.IdRegiao == dto[aux].ValorId1);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.IdRegiao == dto[aux].ValorId1);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.IdRegiao == dto[aux].ValorId1);
                        if (filtroContains.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

                    //Diferente
                    case 2:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                        {
                            var and = agremiacoes.FindAll(c => c.IdRegiao != dto[aux].ValorId1);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.IdRegiao != dto[aux].ValorId1);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.IdRegiao != dto[aux].ValorId1);
                        if (filtroDiferente.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                    //MenorQue
                    case 3:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c => c.IdRegiao < dto[aux].ValorId1);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.IdRegiao < dto[aux].ValorId1);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.FindAll(c => c.IdRegiao < dto[aux].ValorId1);
                        if (filtroMenorQue.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MaiorQue
                    case 4:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c => c.IdRegiao > dto[aux].ValorId1);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.IdRegiao > dto[aux].ValorId1);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMaiorQue = agremiacoes.FindAll(c => c.IdRegiao > dto[aux].ValorId1);
                        if (filtroMaiorQue.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //Entre
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c =>
                                c.IdRegiao < dto[aux].ValorId1 && c.IdRegiao > dto[aux].ValorId2);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c =>
                                c.IdRegiao < dto[aux].ValorId1 && c.IdRegiao > dto[aux].ValorId2);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.FindAll(c =>
                            c.IdRegiao < dto[aux].ValorId1 && c.IdRegiao > dto[aux].ValorId2);
                        if (filtroEntre.Count == 0)
                        {
                            break;
                        }

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
                switch (dto[aux].OperacaoId)
                {
                    //contains
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
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.DataFiliacao == dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.DataFiliacao == dto[aux].DataInicial);
                        if (filtroContains.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

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
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.DataFiliacao != dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.DataFiliacao != dto[aux].DataInicial);
                        if (filtroDiferente.Count == 0)
                        {
                            break;
                        }

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
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.DataFiliacao < dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.FindAll(c => c.DataFiliacao < dto[aux].DataInicial);
                        if (filtroMenorQue.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MaiorQue
                    case 4:
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
                        if (filtroMaiorQue.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //Entre
                    case 5:
                        //And
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 1 && dto[aux].OperacoesMatematicas)
                        {
                            var and = agremiacoes.FindAll(c =>
                                c.DataFiliacao < dto[aux].DataFinal && c.DataFiliacao > dto[aux].DataInicial);
                            return await Filtrar(dto, and, tamanho, ++aux);
                        }

                        //Or
                        if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                        {
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c =>
                                c.DataFiliacao < dto[aux].DataFinal && c.DataFiliacao > dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.FindAll(c =>
                            c.DataFiliacao < dto[aux].DataFinal && c.DataFiliacao > dto[aux].DataInicial);
                        if (filtroEntre.Count == 0)
                        {
                            break;
                        }

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
                switch (dto[aux].OperacaoId)
                {
                    //contains
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
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.DataNascimento == dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroContains = agremiacoes.FindAll(c => c.DataNascimento == dto[aux].DataInicial);
                        if (filtroContains.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroContains, tamanho, ++aux);

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
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.DataNascimento != dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroDiferente = agremiacoes.FindAll(c => c.DataNascimento != dto[aux].DataInicial);
                        if (filtroDiferente.Count == 0)
                        {
                            break;
                        }

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
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c => c.DataNascimento < dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroMenorQue = agremiacoes.FindAll(c => c.DataNascimento < dto[aux].DataInicial);
                        if (filtroMenorQue.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroMenorQue, tamanho, ++aux);

                    //MaiorQue
                    case 4:
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
                        if (filtroMaiorQue.Count == 0)
                        {
                            break;
                        }

                        return await Filtrar(dto, filtroMaiorQue, tamanho, ++aux);

                    //Entre
                    case 5:
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
                            var agremiacaoLista = await _agremiacaoRepository.ObterTodos();
                            var or = agremiacaoLista.FindAll(c =>
                                c.DataNascimento < dto[aux].DataFinal && c.DataNascimento > dto[aux].DataInicial);
                            agremiacoes.AddRange(or);
                            return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                        }

                        var filtroEntre = agremiacoes.FindAll(c =>
                            c.DataNascimento < dto[aux].DataFinal && c.DataNascimento > dto[aux].DataInicial);
                        if (filtroEntre.Count == 0)
                        {
                            break;
                        }

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
        foreach (var agremiacao in agremiacoesFiltro)
        {
            _filtroRepository.Cadastrar(agremiacao);
        }
        
        if (await _filtroRepository.UnitOfWork.Commit())
        {
            var filtro = await _filtroRepository.Buscar(new BuscarAgremiacaoFiltroDto());
            return Mapper.Map<PagedDto<AgremiacaoDto>>(filtro);
        }
        
        Notificator.Handle("Não foi possível salvar as agremiações filtradas!");
        return null;
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
        //workbook.AddWorksheet("planilhaAgremiacoes");
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
                ws.Cell(1, contador).Value = "IdCidade";
                ws.Cell(linha, contador).Value = agremiacao.IdCidade;
                contador++;
            }
            
            if (dto.IdEstado)
            {
                ws.Cell(1, contador).Value = "IdEstado";
                ws.Cell(linha, contador).Value = agremiacao.IdEstado;
                contador++;
            }
            
            if (dto.IdRegiao)
            {
                ws.Cell(1, contador).Value = "IdRegiao";
                ws.Cell(linha, contador).Value = agremiacao.IdRegiao;
                contador++;
            }
            
            if (dto.IdPais)
            {
                ws.Cell(1, contador).Value = "IdPais";
                ws.Cell(linha, contador).Value = agremiacao.IdPais;
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
        if (agremiacao != null)
        {
            return Mapper.Map<AgremiacaoDto>(agremiacao);
        }

        Notificator.HandleNotFoundResource();
        return null;
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
            dto.Foto.FileName.Split(".").Last() != "png" && dto.Foto.FileName.Split(".").Last() != "jpg")
        {
            Notificator.Handle("Foto deve do tipo png, jfif ou jpg");
        }

        return !Notificator.HasNotification;
    }
}