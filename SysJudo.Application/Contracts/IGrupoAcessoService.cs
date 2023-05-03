using SysJudo.Application.Dto.GruposDeAcesso;

namespace SysJudo.Application.Contracts;

public interface IGrupoAcessoService
{
    Task<GrupoAcessoDto?> ObterPorId(int id);
    Task<GrupoAcessoDto?> Cadastrar(CadastrarGrupoAcessoDto dto);
    Task<GrupoAcessoDto?> Alterar(int id, AlterarGrupoAcessoDto dto);
    Task Reativar(int id);
    Task Desativar(int id);
}