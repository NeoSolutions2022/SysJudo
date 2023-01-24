using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories;

public class PaisRepository : Repository<Pais>, IPaisRepository
{
    public PaisRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void Adicionar(Pais pais)
    {
        Context.Paises.Add(pais);
    }

    public void Alterar(Pais pais)
    {
        Context.Paises.Update(pais);
    }

    public async Task<Pais?> ObterPorId(int id)
    {
        return await Context.Paises.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IResultadoPaginado<Pais?>> Buscar(IBuscaPaginada<Pais> filtro)
    {
        var query = Context.Paises.AsQueryable();
        return await base.Buscar(query, filtro);
    }

    public void Remover(Pais pais)
    {
        Context.Paises.Remove(pais);
    }
}