using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Contracts.Repositories;

public interface ISistemaRepository : IRepository<Sistema>
{
    void Adicionar(Sistema sistema);
    void Alterar(Sistema sistema);
    Task<Sistema?> ObterPorId(int id);
    Task<IResultadoPaginado<Sistema?>> Buscar(IBuscaPaginada<Sistema> filtro);
    void Remover(Sistema sistema);
}