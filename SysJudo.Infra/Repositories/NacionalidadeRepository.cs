using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories;

public class NacionalidadeRepository : Repository<Nacionalidade>, INacionalidadeRepositoty
{
    public NacionalidadeRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void Adicionar(Nacionalidade nacionalidade)
    {
        Context.Nacionalidades.Add(nacionalidade);
    }

    public void Alterar(Nacionalidade nacionalidade)
    {
        Context.Nacionalidades.Update(nacionalidade);
    }

    public async Task<Nacionalidade?> ObterPorId(int id)
    {
        return await Context.Nacionalidades.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IResultadoPaginado<Nacionalidade?>> Buscar(IBuscaPaginada<Nacionalidade> filtro)
    {
        var query = Context.Nacionalidades.AsQueryable();
        return await base.Buscar(query, filtro);
    }

    public void Remover(Nacionalidade nacionalidade)
    {
        Context.Nacionalidades.Remove(nacionalidade);
    }
}