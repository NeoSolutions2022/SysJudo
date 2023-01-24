using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Contracts.Repositories;

public interface IAtletaRepository : IRepository<Atleta>
{
    void Adicionar(Atleta atleta);
    void Alterar(Atleta atleta);
    Task<Atleta?> ObterPorId(int id);
    Task<IResultadoPaginado<Atleta?>> Buscar(IBuscaPaginada<Atleta> filtro);
    void Remover(Atleta atleta);
}