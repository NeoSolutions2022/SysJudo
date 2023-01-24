using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Contracts.Repositories;

public interface IUsuarioRepository : IRepository<Usuario>
{
    void Adicionar(Usuario usuario);
    void Alterar(Usuario usuario);
    Task<Usuario?> ObterPorId(int id);
    Task<Usuario?> ObterPorEmail(string email);
    Task<IResultadoPaginado<Usuario>> Buscar(IBuscaPaginada<Usuario> filtro);
    void Remover(Usuario usuario);
}