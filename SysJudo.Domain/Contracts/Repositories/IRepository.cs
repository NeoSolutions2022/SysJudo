using System.Linq.Expressions;
using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Contracts.Repositories;

public interface IRepository<T> : IDisposable where T : BaseEntity, IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
    Task<IResultadoPaginado<T>> Buscar(IBuscaPaginada<T> filtro, CancellationToken cancellationToken = default);
    Task<IResultadoPaginado<T>> Buscar(IQueryable<T> queryable, IBuscaPaginada<T> filtro,
        CancellationToken cancellationToken = default);
    Task<List<T>> Buscar(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefault(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> Count(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> Any(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}

public interface IRepositoryFiltro<T> : IDisposable where T : EntityFiltro, IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
    Task<IResultadoPaginado<T>> Buscar(IBuscaPaginadaFiltro<T> filtro, CancellationToken cancellationToken = default);
    Task<IResultadoPaginado<T>> Buscar(IQueryable<T> queryable, IBuscaPaginadaFiltro<T> filtro,
        CancellationToken cancellationToken = default);
    Task<List<T>> Buscar(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefault(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}