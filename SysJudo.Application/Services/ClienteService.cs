using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Cliente;
using SysJudo.Application.Notifications;
using SysJudo.Core.Extension;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Services;

public class ClienteService : BaseService, IClienteService
{
    private readonly HttpContextAccessor _httpContextAccessor;
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IMapper mapper, INotificator notificator, IClienteRepository clienteRepository,
        IRegistroDeEventoRepository registroDeEventoRepository, IOptions<HttpContextAccessor> httpContextAccessor) : base(mapper, notificator, registroDeEventoRepository)
    {
        _clienteRepository = clienteRepository;
        _httpContextAccessor = httpContextAccessor.Value;
    }

    public async Task<ClienteDto?> Adicionar(CreateClienteDto dto)
    {
        var cliente = Mapper.Map<Cliente>(dto);
        if (!await Validar(cliente))
        {
            return null;
        }

        _clienteRepository.Adicionar(cliente);
        if (await _clienteRepository.UnitOfWork.Commit())
        {
            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                DataHoraEvento = DateTime.Now,
                ComputadorId = _httpContextAccessor.HttpContext?.User.ObterComputadorIp(),
                Descricao = "Adicionar cliente",
                ClienteId = null,
                TipoOperacaoId = 4,
                UsuarioId = null,
                AdministradorId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                FuncaoMenuId = 99
            });

            await RegistroDeEventos.UnitOfWork.Commit();
            return Mapper.Map<ClienteDto>(cliente);
        }

        Notificator.Handle("Não foi possível cadastrar o cliente");
        return null;
    }

    public async Task<ClienteDto?> Alterar(int id, UpdateClienteDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("Os ids não conferem");
            return null;
        }

        var cliente = await _clienteRepository.ObterPorId(id);
        if (cliente == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        Mapper.Map(dto, cliente);
        if (!await Validar(cliente))
        {
            return null;
        }

        _clienteRepository.Alterar(cliente);
        if (await _clienteRepository.UnitOfWork.Commit())
        {
            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                DataHoraEvento = DateTime.Now,
                ComputadorId = _httpContextAccessor.HttpContext?.User.ObterComputadorIp(),
                Descricao = "Alterar cliente",
                ClienteId = null,
                TipoOperacaoId = 5,
                UsuarioId = null,
                AdministradorId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                FuncaoMenuId = null
            });

            await RegistroDeEventos.UnitOfWork.Commit();
            return Mapper.Map<ClienteDto>(cliente);
        }

        Notificator.Handle("Não possível alterar o cliente");
        return null;
    }

    public async Task<PagedDto<ClienteDto>> Buscar(BuscarClienteDto dto)
    {
        var cliente = await _clienteRepository.Buscar(dto);
        return Mapper.Map<PagedDto<ClienteDto>>(cliente);
    }

    public async Task<ClienteDto?> ObterPorId(int id)
    {
        var cliente = await _clienteRepository.ObterPorId(id);
        if (cliente != null)
        {
            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                DataHoraEvento = DateTime.Now,
                ComputadorId = _httpContextAccessor.HttpContext?.User.ObterComputadorIp(),
                Descricao = "Visualizar cliente",
                ClienteId = null,
                TipoOperacaoId = 7,
                UsuarioId = null,
                AdministradorId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                FuncaoMenuId = null
            });

            await RegistroDeEventos.UnitOfWork.Commit();
            return Mapper.Map<ClienteDto>(cliente);
        }

        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task Remover(int id)
    {
        var cliente = await _clienteRepository.ObterPorId(id);
        if (cliente == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        _clienteRepository.Remover(cliente);
        if (!await _clienteRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível remover o cliete");
        }
        
        RegistroDeEventos.Adicionar(new RegistroDeEvento
        {
            DataHoraEvento = DateTime.Now,
            ComputadorId = _httpContextAccessor.HttpContext?.User.ObterComputadorIp(),
            Descricao = "Remover cliente",
            ClienteId = null,
            TipoOperacaoId = 6,
            UsuarioId = null,
            AdministradorId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
            FuncaoMenuId = null
        });

        await RegistroDeEventos.UnitOfWork.Commit();
    }

    private async Task<bool> Validar(Cliente cliente)
    {
        if (!cliente.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }

        var existente = await _clienteRepository.FirstOrDefault(s =>
            (s.Sigla == cliente.Sigla || s.Nome == cliente.Nome) && s.Id != cliente.Id);
        if (existente != null)
        {
            Notificator.Handle("Já existe um cliente cadastrado com essa sigla e/ou nome");
        }

        return !Notificator.HasNotification;
    }
}