using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories;

public class AtletaRepository : Repository<Atleta>, IAtletaRepository
{
    public AtletaRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void Adicionar(Atleta atleta)
    {
        Context.Atletas.Add(atleta);
    }

    public void Alterar(Atleta atleta)
    {
        Context.Atletas.Update(atleta);
    }

    public async Task<Atleta?> ObterPorId(int id)
    {
        return await Context.Atletas.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IResultadoPaginado<Atleta?>> Buscar(IBuscaPaginada<Atleta> filtro)
    {
        var query = Context.Atletas.AsQueryable();
        return await base.Buscar(query, filtro);
    }

    public void Remover(Atleta atleta)
    {
        Context.Atletas.Remove(atleta);
    }
}