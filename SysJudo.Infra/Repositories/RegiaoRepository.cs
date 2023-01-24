using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories;

public class RegiaoRepository : Repository<Regiao>, IRegiaoRepository
{
    public RegiaoRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void Adicionar(Regiao regiao)
    {
        Context.Regioes.Add(regiao);
    }

    public void Alterar(Regiao regiao)
    {
        Context.Regioes.Update(regiao);
    }

    public async Task<Regiao?> ObterPorId(int id)
    {
        return await Context.Regioes.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IResultadoPaginado<Regiao?>> Buscar(IBuscaPaginada<Regiao> filtro)
    {
        var query = Context.Regioes.AsQueryable();
        return await base.Buscar(query, filtro);
    }

    public void Remover(Regiao regiao)
    {
        Context.Regioes.Remove(regiao);
    }
}