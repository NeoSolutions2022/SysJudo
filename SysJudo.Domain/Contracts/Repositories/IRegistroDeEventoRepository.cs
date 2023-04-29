using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Contracts.Repositories;

public interface IRegistroDeEventoRepository : IRepository<RegistroDeEvento>
{
    void Adicionar(RegistroDeEvento registroDeEvento);
    void Update(RegistroDeEvento registroDeEvento);
    Task<RegistroDeEvento?> ObterPorId(int id);
    Task<IResultadoPaginado<RegistroDeEvento>> Buscar(IBuscaPaginada<RegistroDeEvento> filtro);
    void Remover(RegistroDeEvento registroDeEvento);
}