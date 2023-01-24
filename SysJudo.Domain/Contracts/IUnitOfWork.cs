namespace SysJudo.Domain.Contracts;

public interface IUnitOfWork
{
    Task<bool> Commit();
}