using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Contracts.Repositories;

public interface IPaisRepository : IRepository<Pais>
{
    void Adicionar(Pais pais);
    void Alterar(Pais pais);
    Task<Pais?> ObterPorId(int id);
    Task<IResultadoPaginado<Pais?>> Buscar(IBuscaPaginada<Pais> filtro);
    void Remover(Pais pais);
}