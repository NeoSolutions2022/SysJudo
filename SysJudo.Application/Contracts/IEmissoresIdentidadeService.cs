using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.EmissoresIdentidade;

namespace SysJudo.Application.Contracts;

public interface IEmissoresIdentidadeService
{
    Task<EmissoresIdentidadeDto?> Adicionar(CreateEmissoresIdentidadeDto dto);
    Task<EmissoresIdentidadeDto?> Alterar(int id, UpdateEmissoresIdentidadeDto dto);
    Task<PagedDto<EmissoresIdentidadeDto>> Buscar(BuscarEmissoresIdentidadeDto dto);
    Task<EmissoresIdentidadeDto?> ObterPorId(int id);
    Task Remover(int id);
}