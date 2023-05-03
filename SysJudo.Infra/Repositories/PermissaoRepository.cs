using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories;

public class PermissaoRepository : Repository<Permissao>, IPermissaoRepository
{
    public PermissaoRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public async Task<Permissao?> ObterPorId(int id)
    {
        return await Context.Permissoes.FirstOrDefaultAsync(c => c.Id == id);
    }

    public void Cadastrar(Permissao permissao)
    {
        Context.Permissoes.Add(permissao);
    }

    public void Alterar(Permissao permissao)
    {
        Context.Permissoes.Update(permissao);
    }

    public void Deletar(Permissao permissao)
    {
        Context.Permissoes.Remove(permissao);
    }
}