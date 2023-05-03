using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Contracts.Repositories;

public interface IGrupoAcessoRepository : IRepository<GrupoAcesso>
{
    Task<GrupoAcesso?> ObterPorId(int id);
    void Cadastrar(GrupoAcesso grupoAcesso);
    void Editar(GrupoAcesso grupoAcesso);
}