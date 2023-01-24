using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Contracts.Repositories;

public interface IEmissoresIdentidadeRepository : IRepository<EmissoresIdentidade>
{
    void Adicionar(EmissoresIdentidade emissoresIdentidade);
    void Alterar(EmissoresIdentidade emissoresIdentidade);
    Task<EmissoresIdentidade?> ObterPorId(int id);
    Task<IResultadoPaginado<EmissoresIdentidade?>> Buscar(IBuscaPaginada<EmissoresIdentidade> filtro);
    void Remover(EmissoresIdentidade emissoresIdentidade);
}