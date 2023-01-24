using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Contracts.Repositories;

public interface IFaixaRepository : IRepository<Faixa>
{
    void Adicionar(Faixa faixa);
    void Alterar(Faixa faixa);
    Task<Faixa?> ObterPorId(int id);
    Task<IResultadoPaginado<Faixa?>> Buscar(IBuscaPaginada<Faixa> filtro);
    void Remover(Faixa faixa);
}