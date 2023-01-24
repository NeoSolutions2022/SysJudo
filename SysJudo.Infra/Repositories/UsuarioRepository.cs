using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories;

public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void Adicionar(Usuario usuario)
    {
        Context.Usuarios.Add(usuario);
    }

    public void Alterar(Usuario usuario)
    {
        Context.Usuarios.Update(usuario);
    }

    public async Task<Usuario?> ObterPorId(int id)
    {
        return await Context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Usuario?> ObterPorEmail(string email)
    {
        return await Context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IResultadoPaginado<Usuario>> Buscar(IBuscaPaginada<Usuario> filtro)
    {
        var query = Context.Usuarios.AsQueryable();
        return await base.Buscar(query, filtro);
    }

    public void Remover(Usuario usuario)
    {
        Context.Usuarios.Remove(usuario);
    }
}