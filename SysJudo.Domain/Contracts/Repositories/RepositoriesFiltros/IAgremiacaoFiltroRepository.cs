using SysJudo.Domain.Entities.EntitiesFiltros;

namespace SysJudo.Domain.Contracts.Repositories.RepositoriesFiltros;

public interface IAgremiacaoFiltroRepository : IRepositoryFiltro<AgremiacaoFiltro>
{
    void Cadastrar(AgremiacaoFiltro agremiacao);
    Task RemoverTodos();
}