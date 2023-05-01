using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.RegistroDeEvento;

namespace SysJudo.Application.Contracts;

public interface IRegistroDeEventoService
{
    Task<RegistroDeEventoDto> Adicionar(AdicionarRegistroDeEvento dto);
    Task<PagedDto<RegistroDeEventoDto>> Buscar(BuscarRegistroDeEventoDto dto);
    Task<RegistroDeEventoDto?> ObterPorId(int id);
    Task Remover(int id);
}