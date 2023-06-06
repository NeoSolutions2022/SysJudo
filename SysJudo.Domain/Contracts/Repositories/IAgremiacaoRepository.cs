using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Contracts.Repositories;

public interface IAgremiacaoRepository : IRepository<Agremiacao>
{
    void Cadastrar(Agremiacao agremiacao);
    void Alterar(Agremiacao agremiacao);
    Task<Agremiacao?> Obter(int id);
    void Deletar(Agremiacao agremiacao);
    Task<IResultadoPaginado<Agremiacao>> Buscar(IBuscaPaginada<Agremiacao> filtro);
    Task<List<Agremiacao>> ObterTodos();
    Task<List<Agremiacao>> Pesquisar(string valor);
}