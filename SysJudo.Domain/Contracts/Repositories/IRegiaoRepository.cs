using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Contracts.Repositories;

public interface IRegiaoRepository : IRepository<Regiao>
{
    void Adicionar(Regiao regiao);
    void Alterar(Regiao regiao);
    Task<Regiao?> ObterPorId(int id);
    Task<IResultadoPaginado<Regiao?>> Buscar(IBuscaPaginada<Regiao> filtro);
    void Remover(Regiao regiao);
}