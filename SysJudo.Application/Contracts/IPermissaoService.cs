using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Permissoes;

namespace SysJudo.Application.Contracts;

public interface IPermissaoService
{
    Task<PagedDto<PermissaoDto>> Buscar(BuscarPermissaoDto dto);
    Task<PermissaoDto?> ObterPorId(int id);
    Task<PermissaoDto?> Adicionar(CadastrarPermissaoDto dto);
    Task<PermissaoDto?> Alterar(int id, AlterarPermissaoDto dto);
    Task Deletar(int id);
}