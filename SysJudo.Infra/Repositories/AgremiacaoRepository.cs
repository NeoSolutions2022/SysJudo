using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories;

public class AgremiacaoRepository : Repository<Agremiacao>,IAgremiacaoRepository
{
    public AgremiacaoRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void Cadastrar(Agremiacao agremiacao)
    {
        Context.Agremiacoes.Add(agremiacao);
    }

    public void Alterar(Agremiacao agremiacao)
    {
        Context.Agremiacoes.Update(agremiacao);
    }

    public async Task<Agremiacao?> Obter(int id)
    {
        return await Context.Agremiacoes.FirstOrDefaultAsync(c => c.Id == id);
    }

    public void Deletar(Agremiacao agremiacao)
    { 
        Context.Agremiacoes.Remove(agremiacao);
    }

    public async Task<IResultadoPaginado<Agremiacao>> Buscar(IBuscaPaginada<Agremiacao> filtro)
    {
        var query = Context.Agremiacoes.AsQueryable();
        return await base.Buscar(query, filtro);
    }

    public async Task<List<Agremiacao>> ObterTodos()
    {
        return await Context.Agremiacoes
            .AsNoTracking()
            .ToListAsync();
    }
}