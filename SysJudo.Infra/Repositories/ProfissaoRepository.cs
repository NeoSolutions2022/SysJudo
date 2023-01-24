using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories;

public class ProfissaoRepository : Repository<Profissao>, IProfissaoRepository
{
    public ProfissaoRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void Adicionar(Profissao profissao)
    {
        Context.Profissoes.Add(profissao);
    }

    public void Alterar(Profissao profissao)
    {
        Context.Profissoes.Update(profissao);
    }

    public async Task<Profissao?> ObterPorId(int id)
    {
        return await Context.Profissoes.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IResultadoPaginado<Profissao?>> Buscar(IBuscaPaginada<Profissao> filtro)
    {
        var query = Context.Profissoes.AsQueryable();
        return await base.Buscar(query, filtro);
    }

    public void Remover(Profissao profissao)
    {
        Context.Profissoes.Remove(profissao);
    }
}