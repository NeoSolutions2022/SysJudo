using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Repositories.RepositoriesFiltros;
using SysJudo.Domain.Entities.EntitiesFiltros;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories.RepositoriesFiltros;

public class AgremiacaoFiltroRepository : RepositoryFiltro<AgremiacaoFiltro>, IAgremiacaoFiltroRepository
{
    public AgremiacaoFiltroRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void Cadastrar(AgremiacaoFiltro agremiacao)
    {
        Context.AgremiacoesFiltro.Add(agremiacao);
    }

    public async Task<List<AgremiacaoFiltro>> Listar()
    {
        return await Context.AgremiacoesFiltro
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task RemoverTodos()
    {
        var todos = await Context.AgremiacoesFiltro.AsNoTracking().ToListAsync();
        Context.AgremiacoesFiltro.RemoveRange(todos);
    }
}