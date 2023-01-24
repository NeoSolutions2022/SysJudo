using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Contracts.Repositories;

public interface IEstadoRepository : IRepository<Estado>
{
    void Adicionar(Estado estado);
    void Alterar(Estado estado);
    Task<Estado?> ObterPorId(int id);
    Task<IResultadoPaginado<Estado?>> Buscar(IBuscaPaginada<Estado> filtro);
    void Remover(Estado estado);
}