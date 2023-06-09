using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories;

public class RegistroDeEventosRepository : Repository<RegistroDeEvento>,IRegistroDeEventoRepository
{
    public RegistroDeEventosRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void Adicionar(RegistroDeEvento registroDeEvento)
    {
        Context.RegistroDeEventos.Add(registroDeEvento);
    }

    public void Update(RegistroDeEvento registroDeEvento)
    {
        Context.RegistroDeEventos.Update(registroDeEvento);
    }

    public void RemoverTodos()
    {
        Context.Database.ExecuteSqlRaw("DELETE FROM RegistroDeEventos");
    }
    
    public async Task<RegistroDeEvento?> ObterPorId(int id)
    {
        return await Context.RegistroDeEventos.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<RegistroDeEvento>?> ObterTodos()
    {
        return await Context.RegistroDeEventos
            .OrderByDescending(c => c.DataHoraEvento)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IResultadoPaginado<RegistroDeEvento>> Buscar(IBuscaPaginada<RegistroDeEvento> filtro)
    {
        var query = Context.RegistroDeEventos.AsQueryable();
        return await base.Buscar(query, filtro);
    }

    public void Remover(RegistroDeEvento registroDeEvento)
    {
        Context.RegistroDeEventos.Remove(registroDeEvento);
    }
}