using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories;

public class EstadoRepository : Repository<Estado>, IEstadoRepository
{
    public EstadoRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void Adicionar(Estado estado)
    {
        Context.Estados.Add(estado);
    }

    public void Alterar(Estado estado)
    {
        Context.Estados.Update(estado);
    }

    public async Task<Estado?> ObterPorId(int id)
    {
        return await Context.Estados.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IResultadoPaginado<Estado?>> Buscar(IBuscaPaginada<Estado> filtro)
    {
        var query = Context.Estados.AsQueryable();
        return await base.Buscar(query, filtro);
    }

    public void Remover(Estado estado)
    {
        Context.Estados.Remove(estado);
    }
}