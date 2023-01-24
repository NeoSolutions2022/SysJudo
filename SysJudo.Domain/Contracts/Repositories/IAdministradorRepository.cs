using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Contracts.Repositories;

public interface IAdministradorRepository : IRepository<Administrador>
{
    void Adicionar(Administrador administrador);
    void Alterar(Administrador administrador);
    Task<Administrador?> ObterPorId(int id);
    Task<Administrador?> ObterPorEmail(string email);
    void Remover(Administrador administrador);
}