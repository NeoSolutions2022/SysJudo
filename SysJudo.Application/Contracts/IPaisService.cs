using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Pais;

namespace SysJudo.Application.Contracts;

public interface IPaisService
{
    Task<PaisDto?> Adicionar(CreatePaisDto dto);
    Task<PaisDto?> Alterar(int id, UpdatePaisDto dto);
    Task<PagedDto<PaisDto>> Buscar(BuscarPaisDto dto);
    Task<PaisDto?> ObterPorId(int id);
    Task Remover(int id);
}