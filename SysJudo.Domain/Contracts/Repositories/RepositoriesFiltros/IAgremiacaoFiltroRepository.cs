using SysJudo.Domain.Entities.EntitiesFiltros;

namespace SysJudo.Domain.Contracts.Repositories.RepositoriesFiltros;

public interface IAgremiacaoFiltroRepository : IRepositoryFiltro<AgremiacaoFiltro>
{
    void Cadastrar(AgremiacaoFiltro agremiacao);
    void CadastrarTodos(List<AgremiacaoFiltro> agremiacao);
    void LimparFiltro();
    Task<List<AgremiacaoFiltro>> Listar();
    Task RemoverTodos();
    Task<List<AgremiacaoFiltro>> Pesquisar(string valor);
}