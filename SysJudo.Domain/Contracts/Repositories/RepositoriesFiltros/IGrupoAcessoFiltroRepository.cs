using SysJudo.Domain.Entities.EntitiesFiltros;

namespace SysJudo.Domain.Contracts.Repositories.RepositoriesFiltros;

public interface IGrupoAcessoFiltroRepository : IRepositoryFiltro<GrupoDeAcessoFiltro>
{
    void Cadastrar(GrupoDeAcessoFiltro grupoDeAcesso);
    Task<List<GrupoDeAcessoFiltro>> Listar();
    Task RemoverTodos();
    Task<List<GrupoDeAcessoFiltro>> Pesquisar(string valor);
}