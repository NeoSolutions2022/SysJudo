using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Contracts.Repositories;

public interface ICidadeRepository : IRepository<Cidade>
{
    void Adicionar(Cidade cidade);
    void Alterar(Cidade cidade);
    Task<Cidade?> ObterPorId(int id);
    Task<IResultadoPaginado<Cidade?>> Buscar(IBuscaPaginada<Cidade> filtro);
    void Remover(Cidade cidade);
}