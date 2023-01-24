using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories;

public class ClienteRepository : Repository<Cliente>, IClienteRepository
{
    public ClienteRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void Adicionar(Cliente cliente)
    {
        Context.Clientes.Add(cliente);
    }

    public void Alterar(Cliente cliente)
    {
        Context.Clientes.Update(cliente);
    }

    public async Task<Cliente?> ObterPorId(int id)
    {
        return await Context.Clientes.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IResultadoPaginado<Cliente?>> Buscar(IBuscaPaginada<Cliente> filtro)
    {
        var query = Context.Clientes.AsQueryable();
        return await base.Buscar(query, filtro);
    }

    public void Remover(Cliente cliente)
    {
        Context.Clientes.Remove(cliente);
    }

    public async Task<List<Cliente>> Listagem()
    {
        return await Context.Clientes
            .AsNoTracking()
            .ToListAsync();
    }
}