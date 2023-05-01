using AutoMapper;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Cliente;
using SysJudo.Application.Notifications;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Services;

public class ClienteService : BaseService, IClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IMapper mapper, INotificator notificator, IClienteRepository clienteRepository,
        IRegistroDeEventoRepository registroDeEventoRepository) : base(mapper, notificator, registroDeEventoRepository)
    {
        _clienteRepository = clienteRepository;
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