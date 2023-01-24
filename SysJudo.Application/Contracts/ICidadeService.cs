using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Cidade;

namespace SysJudo.Application.Contracts;

public interface ICidadeService
{
    Task<CidadeDto?> Adicionar(CreateCidadeDto dto);
    Task<CidadeDto?> Alterar(int id, UpdateCidadeDto dto);
    Task<PagedDto<CidadeDto>> Buscar(BuscarCidadeDto dto);
    Task<CidadeDto?> ObterPorId(int id);
    Task Remover(int id);
}