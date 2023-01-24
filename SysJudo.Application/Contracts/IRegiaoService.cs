using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Regiao;

namespace SysJudo.Application.Contracts;

public interface IRegiaoService
{
    Task<RegiaoDto?> Adicionar(CreateRegiaoDto dto);
    Task<RegiaoDto?> Alterar(int id, UpdateRegiaoDto dto);
    Task<PagedDto<RegiaoDto>> Buscar(BuscarRegiaoDto dto);
    Task<RegiaoDto?> ObterPorId(int id);
    Task Remover(int id);
}