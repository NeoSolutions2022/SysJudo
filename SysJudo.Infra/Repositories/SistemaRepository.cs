using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories;

public class SistemaRepository : Repository<Sistema>, ISistemaRepository
{
    public SistemaRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void Adicionar(Sistema sistema)
    {
        Context.Sistemas.Add(sistema);
    }

    public void Alterar(Sistema sistema)
    {
        Context.Sistemas.Update(sistema);
    }

    public async Task<Sistema?> ObterPorId(int id)
    {
        return await Context.Sistemas.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IResultadoPaginado<Sistema?>> Buscar(IBuscaPaginada<Sistema> filtro)
    {
        var query = Context.Sistemas.AsQueryable();
        return await base.Buscar(query, filtro);
    }

    public void Remover(Sistema sistema)
    {
        Context.Sistemas.Remove(sistema);
    }
}