using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories;

public class AdministradorRepository : Repository<Administrador>, IAdministradorRepository
{
    public AdministradorRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void Adicionar(Administrador administrador)
    {
        Context.Administradores.Add(administrador);
    }

    public void Alterar(Administrador administrador)
    {
        Context.Administradores.Update(administrador);
    }

    public async Task<Administrador?> ObterPorId(int id)
    {
        return await Context.Administradores.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Administrador?> ObterPorEmail(string email)
    {
        return await Context.Administradores.FirstOrDefaultAsync(a => a.Email == email);
    }

    public void Remover(Administrador administrador)
    {
        Context.Administradores.Remove(administrador);
    }
}