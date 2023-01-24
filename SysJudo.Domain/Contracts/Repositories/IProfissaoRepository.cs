using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Contracts.Repositories;

public interface IProfissaoRepository : IRepository<Profissao>
{
    void Adicionar(Profissao profissao);
    void Alterar(Profissao profissao);
    Task<Profissao?> ObterPorId(int id);
    Task<IResultadoPaginado<Profissao?>> Buscar(IBuscaPaginada<Profissao> filtro);
    void Remover(Profissao profissao);
}