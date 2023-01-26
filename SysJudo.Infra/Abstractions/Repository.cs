using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts;
using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using SysJudo.Infra.Context;
using SysJudo.Infra.Extensions;

namespace SysJudo.Infra.Abstractions;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity, IAggregateRoot, new()
{
    private bool _isDisposed;
    private readonly DbSet<TEntity> _dbSet;
    protected readonly BaseApplicationDbContext Context;

    public Repository(BaseApplicationDbContext context)
    {
        _dbSet = context.Set<TEntity>();
        Context = context;
    }

    public IUnitOfWork UnitOfWork => Context;
    public virtual async Task<IResultadoPaginado<TEntity>> Buscar(IBuscaPaginada<TEntity> filtro, 
        CancellationToken cancellationToken = default)
    {
        var queryable = _dbSet.AsQueryable();
        
        filtro.AplicarFiltro(ref queryable);
        filtro.AplicarOrdenacao(ref queryable);
        
        return await queryable.BuscarPaginadoAsync(filtro.Pagina, filtro.TamanhoPagina, cancellationToken);
    }
    
    public async Task<IResultadoPaginado<TEntity>> Buscar(IQueryable<TEntity> queryable, IBuscaPaginada<TEntity> filtro
        , CancellationToken cancellationToken = default)
    {
        filtro.AplicarFiltro(ref queryable);
        filtro.AplicarOrdenacao(ref queryable);
        
        return await queryable.BuscarPaginadoAsync(filtro.Pagina, filtro.TamanhoPagina, cancellationToken);
    }
    
    public virtual async Task<List<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTrackingWithIdentityResolution().Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> FirstOrDefault(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTrackingWithIdentityResolution().Where(predicate)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<int> Count(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) 
        => await _dbSet.CountAsync(predicate, cancellationToken);

    public async Task<bool> Any(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) 
        => await _dbSet.AnyAsync(predicate, cancellationToken);
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            Context.Dispose();
        }

        _isDisposed = true;
    }
    
    ~Repository()
    {
        Dispose(false);
    }
}

public class RepositoryFiltro<TEntityFiltrto> : IRepositoryFiltro<TEntityFiltrto> where TEntityFiltrto : EntityFiltro, IAggregateRoot, new()
{
    private bool _isDisposed;
    private readonly DbSet<TEntityFiltrto> _dbSet;
    protected readonly BaseApplicationDbContext Context;

    public RepositoryFiltro(BaseApplicationDbContext context)
    {
        _dbSet = context.Set<TEntityFiltrto>();
        Context = context;
    }

    public IUnitOfWork UnitOfWork => Context;

    public virtual async Task<IResultadoPaginado<TEntityFiltrto>> Buscar(IBuscaPaginadaFiltro<TEntityFiltrto> filtro, 
        CancellationToken cancellationToken = default)
    {
        var queryable = _dbSet.AsQueryable();
        
        filtro.AplicarFiltro(ref queryable);
        filtro.AplicarOrdenacao(ref queryable);
        
        return await queryable.BuscarPaginadoAsync(filtro.Pagina, filtro.TamanhoPagina, cancellationToken);
    }
    
    public async Task<IResultadoPaginado<TEntityFiltrto>> Buscar(IQueryable<TEntityFiltrto> queryable, IBuscaPaginadaFiltro<TEntityFiltrto> filtro
        , CancellationToken cancellationToken = default)
    {
        filtro.AplicarFiltro(ref queryable);
        filtro.AplicarOrdenacao(ref queryable);
        
        return await queryable.BuscarPaginadoAsync(filtro.Pagina, filtro.TamanhoPagina, cancellationToken);
    }
    
    public virtual async Task<List<TEntityFiltrto>> Buscar(Expression<Func<TEntityFiltrto, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTrackingWithIdentityResolution().Where(predicate).ToListAsync(cancellationToken);
    }
    
    public async Task<TEntityFiltrto?> FirstOrDefault(Expression<Func<TEntityFiltrto, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTrackingWithIdentityResolution().Where(predicate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            Context.Dispose();
        }

        _isDisposed = true;
    }
    
    ~RepositoryFiltro()
    {
        Dispose(false);
    }
}