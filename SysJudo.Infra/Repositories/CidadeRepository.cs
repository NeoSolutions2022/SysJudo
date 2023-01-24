using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories;

public class CidadeRepository : Repository<Cidade>, ICidadeRepository
{
    public CidadeRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void Adicionar(Cidade cidade)
    {
        Context.Cidades.Add(cidade);
    }

    public void Alterar(Cidade cidade)
    {
        Context.Cidades.Update(cidade);
    }

    public async Task<Cidade?> ObterPorId(int id)
    {
        return await Context.Cidades.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IResultadoPaginado<Cidade?>> Buscar(IBuscaPaginada<Cidade> filtro)
    {
        var query = Context.Cidades.AsQueryable();
        return await base.Buscar(query, filtro);
    }

    public void Remover(Cidade cidade)
    {
        Context.Cidades.Remove(cidade);
    }
}