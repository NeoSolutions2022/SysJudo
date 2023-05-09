using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Contracts.Repositories;

public interface IGrupoAcessoRepository : IRepository<GrupoAcesso>
{
    Task<GrupoAcesso?> ObterPorId(int id);
    Task<List<GrupoAcesso>> ObterTodos();
    void Cadastrar(GrupoAcesso grupoAcesso);
    void Editar(GrupoAcesso grupoAcesso);
}