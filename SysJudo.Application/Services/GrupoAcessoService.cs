using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.GruposDeAcesso;
using SysJudo.Application.Notifications;
using SysJudo.Core.Extension;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Contracts.Repositories.RepositoriesFiltros;
using SysJudo.Domain.Entities;
using SysJudo.Domain.Entities.EntitiesFiltros;

namespace SysJudo.Application.Services;

public class GrupoAcessoService : BaseService, IGrupoAcessoService
{
    private readonly IPermissaoRepository _permissaoRepository;
    private readonly IGrupoAcessoRepository _grupoAcessoRepository;
    private readonly IGrupoAcessoFiltroRepository _filtroRepository;
    private readonly HttpContextAccessor _httpContextAccessor;

    public GrupoAcessoService(IMapper mapper, INotificator notificator, IPermissaoRepository permissaoRepository,
        IGrupoAcessoRepository grupoAcessoRepository, IRegistroDeEventoRepository registroDeEventoRepository,
        IGrupoAcessoFiltroRepository filtroRepository, IOptions<HttpContextAccessor> httpContextAccessor)
        : base(mapper, notificator, registroDeEventoRepository)
    {
        _permissaoRepository = permissaoRepository;
        _grupoAcessoRepository = grupoAcessoRepository;
        _filtroRepository = filtroRepository;
        _httpContextAccessor = httpContextAccessor.Value;
    }

    public async Task<PagedDto<GrupoAcessoDto>> Buscar(BuscarGrupoAcessoDto dto)
    {
        var grupoAcessos = await _grupoAcessoRepository.Buscar(dto);
        return Mapper.Map<PagedDto<GrupoAcessoDto>>(grupoAcessos);
    }

    public async Task<GrupoAcessoDto?> ObterPorId(int id)
    {
        var grupoAcesso = await ObterGrupoAcesso(id);
        return grupoAcesso != null ? Mapper.Map<GrupoAcessoDto>(grupoAcesso) : null;
    }

    public async Task<GrupoAcessoDto?> Cadastrar(CadastrarGrupoAcessoDto dto)
    {
        var grupoAcesso = Mapper.Map<GrupoAcesso>(dto);

        if (!await Validar(grupoAcesso))
        {
            return null;
        }

        _grupoAcessoRepository.Cadastrar(grupoAcesso);
        RegistroDeEventos.Adicionar(new RegistroDeEvento
        {
            DataHoraEvento = DateTime.Now,
            ComputadorId = ObterIp(),
            Descricao = "Adicionar registro de evento",
            ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
            TipoOperacaoId = 4,
            UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
            AdministradorNome = null,
            UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
            AdministradorId = null,
            FuncaoMenuId = null
        });

        return await CommitChanges() ? Mapper.Map<GrupoAcessoDto>(grupoAcesso) : null;
    }

    public async Task<GrupoAcessoDto?> Alterar(int id, AlterarGrupoAcessoDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("IDs não conferem.");
            return null;
        }

        var grupoAcesso = await ObterGrupoAcesso(id);
        if (grupoAcesso == null)
        {
            return null;
        }

        Mapper.Map(dto, grupoAcesso);
        if (!await Validar(grupoAcesso))
        {
            return null;
        }

        _grupoAcessoRepository.Editar(grupoAcesso);
        RegistroDeEventos.Adicionar(new RegistroDeEvento
        {
            DataHoraEvento = DateTime.Now,
            ComputadorId = ObterIp(),
            Descricao = "Alterar registro de evento",
            ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
            TipoOperacaoId = 5,
            UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
            AdministradorNome = null,
            UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
            AdministradorId = null,
            FuncaoMenuId = null
        });

        return await CommitChanges() ? Mapper.Map<GrupoAcessoDto>(grupoAcesso) : null;
    }

    public async Task<List<GrupoDeAcessoFiltroDto>> Filtrar(List<FiltragemGrupoDeAcessoDto> dto,
        List<GrupoAcesso>? agremiacoes = null, int tamanho = 0, int aux = 0)
    {
        {
            tamanho = dto.Count;

            if (aux < tamanho)
            {
                #region Nome

                if (dto[aux].NomeParametro == "Nome")
                {
                    agremiacoes = await PossuiGrupoDeAcesso(dto[aux].NomeParametro, agremiacoes);
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
                                var agremiacaoLista = await PossuiGrupoDeAcesso(dto[aux].NomeParametro);
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
                                var agremiacaoLista = await PossuiGrupoDeAcesso(dto[aux].NomeParametro);
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
                                var agremiacaoLista = await PossuiGrupoDeAcesso(dto[aux].NomeParametro);
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
                                    .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!))
                                    .ToList();
                                return await Filtrar(dto, menor, tamanho, ++aux);
                            }

                            //Or
                            if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                            {
                                var agremiacaoLista = await PossuiGrupoDeAcesso(dto[aux].NomeParametro);
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
                                var agremiacaoLista = await PossuiGrupoDeAcesso(dto[aux].NomeParametro);
                                var or = agremiacaoLista
                                    .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                          1)
                                    .ToList();
                                agremiacoes.AddRange(or);
                                return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                            }

                            var filtroMenorIgualQue =
                                agremiacoes.Take(
                                        GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) +
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
                                var agremiacaoLista = await PossuiGrupoDeAcesso(dto[aux].NomeParametro);
                                var or = agremiacaoLista
                                    .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                          1)
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
                                var agremiacaoLista = await PossuiGrupoDeAcesso(dto[aux].NomeParametro);
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
                                    .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                    .ToList()
                                    .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                    .ToList();
                                return await Filtrar(dto, and, tamanho, ++aux);
                            }

                            //Or
                            if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                            {
                                var agremiacaoLista = await PossuiGrupoDeAcesso(dto[aux].NomeParametro);
                                var or = agremiacaoLista
                                    .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                    .ToList()
                                    .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                          1)
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

                #region Descricao

                if (dto[aux].NomeParametro == "Descricao")
                {
                    agremiacoes = await PossuiGrupoDeAcesso(dto[aux].NomeParametro, agremiacoes);
                    switch (dto[aux].OperacaoId)
                    {
                        //contains
                        case 1:
                            //And
                            if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                            {
                                var and = agremiacoes.FindAll(c => c.Descricao.Contains(dto[aux].ValorString!));
                                return await Filtrar(dto, and, tamanho, ++aux);
                            }

                            //Or
                            if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                            {
                                var agremiacaoLista = await PossuiGrupoDeAcesso(dto[aux].NomeParametro);
                                var or = agremiacaoLista.FindAll(c => c.Descricao.Contains(dto[aux].ValorString!));
                                agremiacoes.AddRange(or);
                                return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                            }

                            var filtroContains = agremiacoes.FindAll(c => c.Descricao.Contains(dto[aux].ValorString!));
                            return await Filtrar(dto, filtroContains, tamanho, ++aux);

                        //Igual
                        case 2:
                            //And
                            if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                            {
                                var and = agremiacoes.FindAll(c => c.Descricao == dto[aux].ValorString);
                                return await Filtrar(dto, and, tamanho, ++aux);
                            }

                            //Or
                            if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                            {
                                var agremiacaoLista = await PossuiGrupoDeAcesso(dto[aux].NomeParametro);
                                var or = agremiacaoLista.FindAll(c => c.Descricao == dto[aux].ValorString);
                                agremiacoes.AddRange(or);
                                return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                            }

                            var filtroIgual = agremiacoes.FindAll(c => c.Descricao == dto[aux].ValorString);
                            return await Filtrar(dto, filtroIgual, tamanho, ++aux);

                        //Diferente
                        case 3:
                            //And
                            if (aux != 0 && dto[aux - 1].OperadorLogico == 1)
                            {
                                var and = agremiacoes.FindAll(c => c.Descricao != dto[aux].ValorString);
                                return await Filtrar(dto, and, tamanho, ++aux);
                            }

                            //Or
                            if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                            {
                                var agremiacaoLista = await PossuiGrupoDeAcesso(dto[aux].NomeParametro);
                                var or = agremiacaoLista.FindAll(c => c.Descricao != dto[aux].ValorString);
                                agremiacoes.AddRange(or);
                                return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                            }

                            var filtroDiferente = agremiacoes.FindAll(c => c.Descricao != dto[aux].ValorString);
                            return await Filtrar(dto, filtroDiferente, tamanho, ++aux);

                        //MenorQue
                        case 4:
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
                                var agremiacaoLista = await PossuiGrupoDeAcesso(dto[aux].NomeParametro);
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
                                var agremiacaoLista = await PossuiGrupoDeAcesso(dto[aux].NomeParametro);
                                var or = agremiacaoLista
                                    .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                          1)
                                    .ToList();
                                agremiacoes.AddRange(or);
                                return await Filtrar(dto, agremiacoes, tamanho, ++aux);
                            }

                            var filtroMenorIgualQue =
                                agremiacoes.Take(
                                        GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) +
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
                                var agremiacaoLista = await PossuiGrupoDeAcesso(dto[aux].NomeParametro);
                                var or = agremiacaoLista
                                    .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                          1)
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
                                var agremiacaoLista = await PossuiGrupoDeAcesso(dto[aux].NomeParametro);
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
                                    .Take(GetIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                    .ToList()
                                    .Skip(GetLastIndex(agremiacoes, dto[aux].NomeParametro, dto[aux].ValorString!) + 1)
                                    .ToList();
                                return await Filtrar(dto, and, tamanho, ++aux);
                            }

                            //Or
                            if (aux != 0 && dto[aux - 1].OperadorLogico == 2)
                            {
                                var agremiacaoLista = await PossuiGrupoDeAcesso(dto[aux].NomeParametro);
                                var or = agremiacaoLista
                                    .Take(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString2!))
                                    .ToList()
                                    .Skip(GetLastIndex(agremiacaoLista, dto[aux].NomeParametro, dto[aux].ValorString!) +
                                          1)
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
            }

            var grupoAcessoFiltro = Mapper.Map<List<GrupoDeAcessoFiltro>>(agremiacoes);
            await _filtroRepository.RemoverTodos();
            foreach (var agremiacao in grupoAcessoFiltro.DistinctBy(c => c.Id))
            {
                _filtroRepository.Cadastrar(agremiacao);
            }

            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                DataHoraEvento = DateTime.Now,
                ComputadorId = ObterIp(),
                Descricao = "Filtrar grupo de acesso",
                ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
                TipoOperacaoId = 13,
                UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
                AdministradorNome = null,
                UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                AdministradorId = null,
                FuncaoMenuId = 97
            });

            if (await _filtroRepository.UnitOfWork.Commit())
            {
                return Mapper.Map<List<GrupoDeAcessoFiltroDto>>(
                    await _filtroRepository.Listar());
            }

            return Mapper.Map<List<GrupoDeAcessoFiltroDto>>(
                await _filtroRepository.Listar());
        }
    }

    public async Task Reativar(int id)
    {
        var grupoAcesso = await ObterGrupoAcesso(id);
        if (grupoAcesso == null)
        {
            return;
        }

        grupoAcesso.Desativado = false;
        _grupoAcessoRepository.Editar(grupoAcesso);
        await CommitChanges();
    }

    public async Task Desativar(int id)
    {
        var grupoAcesso = await ObterGrupoAcesso(id);
        if (grupoAcesso == null)
        {
            return;
        }

        if (grupoAcesso.Administrador)
        {
            Notificator.Handle("Não é possível desativar o Grupo de Acesso padrão de Administrador.");
            return;
        }

        grupoAcesso.Desativado = true;
        _grupoAcessoRepository.Editar(grupoAcesso);
        await CommitChanges();
    }

    public async Task<List<GrupoDeAcessoFiltroDto>> Pesquisar(string valor)
    {
        var grupoAcesso = await _filtroRepository.Pesquisar(valor);

        if (grupoAcesso.Count == 0)
        {
            return new List<GrupoDeAcessoFiltroDto>();
        }

        await _filtroRepository.RemoverTodos();
        foreach (var grupo in grupoAcesso.DistinctBy(c => c.Id))
        {
            _filtroRepository.Cadastrar(grupo);
        }

        RegistroDeEventos.Adicionar(new RegistroDeEvento
        {
            DataHoraEvento = DateTime.Now,
            ComputadorId = ObterIp(),
            Descricao = "Pesquisar grupo de acesso",
            ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
            TipoOperacaoId = 14,
            UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
            AdministradorNome = null,
            UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
            AdministradorId = null,
            FuncaoMenuId = 97
        });

        if (await _filtroRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<List<GrupoDeAcessoFiltroDto>>(
                await _filtroRepository.Listar());
        }

        return Mapper.Map<List<GrupoDeAcessoFiltroDto>>(grupoAcesso);
    }

    public async Task LimparFiltro()
    {
        await _filtroRepository.RemoverTodos();

        var grupoAcesso = await _grupoAcessoRepository.ObterTodos();
        if (grupoAcesso == null)
        {
            Notificator.Handle("Não existe grupos de acesso");
            return;
        }

        var grupoAcessoFiltro = Mapper.Map<List<GrupoDeAcessoFiltro>>(grupoAcesso!);
        await _filtroRepository.RemoverTodos();
        foreach (var agremiacao in grupoAcessoFiltro.DistinctBy(c => c.Id))
        {
            _filtroRepository.Cadastrar(agremiacao);
        }

        if (!await _grupoAcessoRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível limpar o filtro");
        }
    }

    private async Task<bool> Validar(GrupoAcesso grupoAcesso)
    {
        if (!grupoAcesso.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }

        var existente = await _grupoAcessoRepository.FirstOrDefault(c
            => c.Nome == grupoAcesso.Nome && c.Descricao == grupoAcesso.Descricao && c.Id != grupoAcesso.Id);
        if (existente != null)
        {
            Notificator.Handle(
                $"Já existe um grupo {(existente.Desativado ? "desativado" : "ativo")} com o mesmo nome e tipo.");
        }

        var permissoesIds = grupoAcesso.Permissoes.Select(c => c.PermissaoId).Distinct();
        var countPermissoes = await _permissaoRepository.Count(p => permissoesIds.Contains(p.Id));
        if (permissoesIds.Count() != countPermissoes)
        {
            Notificator.Handle("Uma ou mais permissões são inválidas.");
        }

        return !Notificator.HasNotification;
    }

    private async Task<GrupoAcesso?> ObterGrupoAcesso(int id)
    {
        var grupoAcesso = await _grupoAcessoRepository.ObterPorId(id);
        if (grupoAcesso == null)
        {
            Notificator.HandleNotFoundResource();
        }

        return grupoAcesso;
    }

    private async Task<bool> CommitChanges()
    {
        if (await _grupoAcessoRepository.UnitOfWork.Commit())
        {
            return true;
        }

        Notificator.Handle("Não foi possível salvar alterações!");
        return false;
    }

    private async Task<List<GrupoAcesso>> PossuiGrupoDeAcesso(string nomeParametro,
        List<GrupoAcesso>? gruposAcesso = null)
    {
        if (gruposAcesso == null)
        {
            gruposAcesso = await _grupoAcessoRepository.ObterTodos();
        }

        switch (nomeParametro)
        {
            case "Nome": return gruposAcesso.OrderBy(c => c.Nome).ToList();
            case "Descricao": return gruposAcesso.OrderBy(c => c.Descricao).ToList();
            default: return gruposAcesso.OrderBy(c => c.Id).ToList();
        }
    }

    private int GetLastIndex(List<GrupoAcesso>? gruposAcesso, string nomeParametro, string nome)
    {
        switch (nomeParametro)
        {
            case "Descricao":
            {
                var index = gruposAcesso!.FindLastIndex(c => c.Descricao == nome);
                if (index < 0)
                {
                    return gruposAcesso.Count + 1;
                }

                return index;
            }
            default:
            {
                var index = gruposAcesso!.FindLastIndex(c => c.Nome == nome);
                if (index < 0)
                {
                    return gruposAcesso.Count + 1;
                }

                return index;
            }
        }
    }

    private int GetIndex(List<GrupoAcesso>? gruposAcesso, string nomeParametro, string nome)
    {
        switch (nomeParametro)
        {
            case "Descricao":
            {
                var index = gruposAcesso!.FindIndex(c => c.Descricao == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
            default:
            {
                var index = gruposAcesso!.FindIndex(c => c.Nome == nome);
                if (index < 0)
                {
                    return -1;
                }

                return index;
            }
        }
    }
}