using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories;

public class EmissoresIdentidadeRepository : Repository<EmissoresIdentidade>, IEmissoresIdentidadeRepository
{
    public EmissoresIdentidadeRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void Adicionar(EmissoresIdentidade emissoresIdentidade)
    {
        Context.EmissoresIdentidades.Add(emissoresIdentidade);
    }

    public void Alterar(EmissoresIdentidade emissoresIdentidade)
    {
        Context.EmissoresIdentidades.Update(emissoresIdentidade);
    }

    public async Task<EmissoresIdentidade?> ObterPorId(int id)
    {
        return await Context.EmissoresIdentidades.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IResultadoPaginado<EmissoresIdentidade?>> Buscar(IBuscaPaginada<EmissoresIdentidade> filtro)
    {
        var query = Context.EmissoresIdentidades.AsQueryable();
        return await base.Buscar(query, filtro);
    }

    public void Remover(EmissoresIdentidade emissoresIdentidade)
    {
        Context.EmissoresIdentidades.Remove(emissoresIdentidade);
    }
}