using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories;

public class GrupoAcessoRepository : Repository<GrupoAcesso>, IGrupoAcessoRepository
{
    public GrupoAcessoRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public async Task<GrupoAcesso?> ObterPorId(int id)
    {
        return await Context.GruposAcesso
            .Include(ga => ga.Permissoes)
            .ThenInclude(gap => gap.Permissao)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<GrupoAcesso>> ObterTodos()
    {
        return await Context.GruposAcesso
            .AsNoTracking()
            .ToListAsync();
    }

    public void Cadastrar(GrupoAcesso grupoAcesso)
    {
        Context.GruposAcesso.Add(grupoAcesso);
    }

    public void Editar(GrupoAcesso grupoAcesso)
    {
        Context.GruposAcesso.Update(grupoAcesso);
    }
}