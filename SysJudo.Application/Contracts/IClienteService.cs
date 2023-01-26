using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Cliente;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Contracts;

public interface IClienteService
{
    Task<ClienteDto?> Adicionar(CreateClienteDto dto);
    Task<ClienteDto?> Alterar(int id, UpdateClienteDto dto);
    Task<PagedDto<ClienteDto>> Buscar(BuscarClienteDto dto);
    Task<ClienteDto?> ObterPorId(int id);
    Task Remover(int id); 
}