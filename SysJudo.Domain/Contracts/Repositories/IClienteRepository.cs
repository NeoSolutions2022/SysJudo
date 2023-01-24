using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Contracts.Repositories;

public interface IClienteRepository : IRepository<Cliente>
{
    void Adicionar(Cliente cliente);
    void Alterar(Cliente cliente);
    Task<Cliente?> ObterPorId(int id);
    Task<IResultadoPaginado<Cliente?>> Buscar(IBuscaPaginada<Cliente> filtro);
    void Remover(Cliente cliente);
    Task<List<Cliente>> Listagem();
}