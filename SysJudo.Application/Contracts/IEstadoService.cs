using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Estado;

namespace SysJudo.Application.Contracts;

public interface IEstadoService
{
    Task<EstadoDto?> Adicionar(CreateEstadoDto dto);
    Task<EstadoDto?> Alterar(int id, UpdadeEstadoDto dto);
    Task<PagedDto<EstadoDto>> Buscar(BuscarEstadoDto dto);
    Task<EstadoDto?> ObterPorId(int id);
    Task Remover(int id);
}