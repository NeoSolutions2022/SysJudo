using AutoMapper;
using FluentEmail.Core;
using StackExchange.Redis;
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
    
    public ClienteService(IMapper mapper, INotificator notificator, IClienteRepository clienteRepository) : base(mapper, notificator)
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

    public async Task<List<ClienteDto?>> Filtrar(List<FiltragemDto> filtragens, List<Cliente> clientes = null, int tamanho = 0,
        int aux = 0)
    {
        tamanho = filtragens.Count;

        //Verificando se a lista de clientes é nula
        if (clientes == null)
        {
            clientes = await _clienteRepository.Listagem();
        }

        if (aux < tamanho)
        {
            #region Nome

            if (filtragens[aux].NomeParametro == "Nome")
            {
                switch (filtragens[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        var filtroContains = clientes.FindAll(c => c.Nome == filtragens[aux].Valor1);
                        if (filtroContains.Count == 0)
                        {
                            break;
                        }
                        return await Filtrar(filtragens, filtroContains,  tamanho, ++aux);
                    
                    //Diferente
                    case 2:
                        var filtroDiferente = clientes.FindAll(c => c.Nome != filtragens[aux].Valor1);
                        if (filtroDiferente.Count == 0)
                        {
                            break;
                        }
                        return await Filtrar(filtragens, filtroDiferente,  tamanho, ++aux);
                }
            }

            #endregion

            #region Sigla

            if (filtragens[aux].NomeParametro == "Sigla")
            {
                switch (filtragens[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        var filtro = clientes.FindAll(c => c.Sigla == filtragens[aux].Valor1);
                        return await Filtrar(filtragens, filtro,  tamanho, ++aux);
                    
                    //Diferente
                    case 2:
                        var filtroDiferente = clientes.FindAll(c => c.Sigla != filtragens[aux].Valor1);
                        if (filtroDiferente.Count == 0)
                        {
                            break;
                        }
                        return await Filtrar(filtragens, filtroDiferente,  tamanho, ++aux);
                }
            }

            #endregion

            #region PastaArquivo

            if (filtragens[aux].NomeParametro == "PastaArquivo")
            {
                switch (filtragens[aux].OperacaoId)
                {
                    //contains
                    case 1:
                        var filtro = clientes.FindAll(c => c.PastaArquivo == filtragens[aux].Valor1);
                        if (filtro.Count == 0)
                        {
                            break;
                        }
                        return await Filtrar(filtragens, filtro,  tamanho, ++aux);
                    
                    //Diferente
                    case 2:
                        var filtroDiferente = clientes.FindAll(c => c.PastaArquivo != filtragens[aux].Valor1);
                        if (filtroDiferente.Count == 0)
                        {
                            break;
                        }
                        return await Filtrar(filtragens, filtroDiferente,  tamanho, ++aux);
                }
            }

            #endregion
        }
        return Mapper.Map<List<ClienteDto?>>(clientes);
    }

    private async Task<bool> Validar(Cliente cliente)
    {
        if (!cliente.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }

        var existente = await _clienteRepository.FirstOrDefault(s => (s.Sigla == cliente.Sigla || s.Nome == cliente.Nome) && s.Id != cliente.Id);
        if (existente != null)
        {
            Notificator.Handle("Já existe um cliente cadastrado com essa sigla e/ou nome");
        }

        return !Notificator.HasNotification;
    }
}