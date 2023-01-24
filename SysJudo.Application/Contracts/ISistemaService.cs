using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Sistema;

namespace SysJudo.Application.Contracts;

public interface ISistemaService
{
    Task<SistemaDto?> Adicionar(CreateSistemaDto dto);
    Task<SistemaDto?> Alterar(int id, UpdateSistemaDto dto);
    Task<PagedDto<SistemaDto>> Buscar(BuscarSistemaDto dto);
    Task<SistemaDto?> ObterPorId(int id);
    Task Remover(int id);
}