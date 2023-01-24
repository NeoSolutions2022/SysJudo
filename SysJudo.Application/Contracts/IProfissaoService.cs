using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Profissao;

namespace SysJudo.Application.Contracts;

public interface IProfissaoService
{
    Task<ProfissaoDto?> Adicionar(CreateProfissaoDto dto);
    Task<ProfissaoDto?> ObterPorId(int id);
    Task<PagedDto<ProfissaoDto>> Buscar(BuscarProfissaoDto dto);
    Task<ProfissaoDto?> Alterar(int id, UpdateProfissaoDto dto);
    Task Remover(int id);
}