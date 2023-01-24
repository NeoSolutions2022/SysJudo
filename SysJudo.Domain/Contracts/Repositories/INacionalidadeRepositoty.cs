using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Contracts.Repositories;

public interface INacionalidadeRepositoty : IRepository<Nacionalidade>
{
    void Adicionar(Nacionalidade nacionalidade);
    void Alterar(Nacionalidade nacionalidade);
    Task<Nacionalidade?> ObterPorId(int id);
    Task<IResultadoPaginado<Nacionalidade?>> Buscar(IBuscaPaginada<Nacionalidade> filtro);
    void Remover(Nacionalidade nacionalidade);
}