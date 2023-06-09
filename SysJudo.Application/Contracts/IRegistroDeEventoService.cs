using SysJudo.Application.Dto.RegistroDeEvento;

namespace SysJudo.Application.Contracts;

public interface IRegistroDeEventoService
{
    Task<List<RegistroDeEventoDto>> ObterTodos();
    Task<RegistroDeEventoDto?> ObterPorId(int id);
    Task RemoverTodos();
}