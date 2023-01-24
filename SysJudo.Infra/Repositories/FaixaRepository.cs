using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories;

public class FaixaRepository : Repository<Faixa>, IFaixaRepository
{
    public FaixaRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void Adicionar(Faixa faixa)
    {
        Context.Faixas.Add(faixa);
    }

    public void Alterar(Faixa faixa)
    {
        Context.Faixas.Update(faixa);
    }

    public async Task<Faixa?> ObterPorId(int id)
    {
        return await Context.Faixas.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IResultadoPaginado<Faixa?>> Buscar(IBuscaPaginada<Faixa> filtro)
    {
        var query = Context.Faixas.AsQueryable();
        return await base.Buscar(query, filtro);
    }

    public void Remover(Faixa faixa)
    {
        Context.Faixas.Remove(faixa);
    }
}