using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Nacionalidade;

namespace SysJudo.Application.Contracts;

public interface INacionalidadeService
{
    Task<NacionalidadeDto?> Adicionar(CreateNacionalidadeDto dto);
    Task<NacionalidadeDto?> Alterar(int id, UpdateNacionalidadeDto dto);
    Task<PagedDto<NacionalidadeDto>> Buscar(BuscarNacionalidadeDto dto);
    Task<NacionalidadeDto?> ObterPorId(int id);
    Task Remover(int id);
}