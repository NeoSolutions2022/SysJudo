using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Contracts.Repositories;

public interface IPermissaoRepository : IRepository<Permissao>
{
    Task<Permissao?> ObterPorId(int id);
    void Cadastrar(Permissao permissao);
    void Alterar(Permissao permissao);
    void Deletar(Permissao permissao);
}