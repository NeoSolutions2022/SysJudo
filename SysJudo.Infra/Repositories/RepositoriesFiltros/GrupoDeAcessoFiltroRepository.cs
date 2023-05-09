using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Repositories.RepositoriesFiltros;
using SysJudo.Domain.Entities.EntitiesFiltros;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories.RepositoriesFiltros;

public class GrupoDeAcessoFiltroRepository : RepositoryFiltro<GrupoDeAcessoFiltro>, IGrupoAcessoFiltroRepository
{
    public GrupoDeAcessoFiltroRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void Cadastrar(GrupoDeAcessoFiltro grupoDeAcesso)
    {
        Context.GruposDeAcessoFiltro.Add(grupoDeAcesso);
    }

    public async Task<List<GrupoDeAcessoFiltro>> Listar()
    {
        return await Context.GruposDeAcessoFiltro
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task RemoverTodos()
    {
        Context.AgremiacoesFiltro.RemoveRange(await Context.AgremiacoesFiltro.AsNoTracking().ToListAsync());
    }

    public async Task<List<GrupoDeAcessoFiltro>> Pesquisar(string valor)
    {
        return await Context.GruposDeAcessoFiltro.Where(c =>
                c.Descricao.Contains(valor) || 
                c.Nome.Contains(valor))
            .AsNoTracking()
            .ToListAsync();
    }
}