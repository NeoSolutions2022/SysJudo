using SysJudo.Application.Dto.GruposDeAcesso;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Contracts;

public interface IGrupoAcessoService
{
    Task<GrupoAcessoDto?> ObterPorId(int id);
    Task<GrupoAcessoDto?> Cadastrar(CadastrarGrupoAcessoDto dto);
    Task<GrupoAcessoDto?> Alterar(int id, AlterarGrupoAcessoDto dto);
    Task<List<GrupoDeAcessoFiltroDto>> Filtrar(List<FiltragemGrupoDeAcessoDto> dto, List<GrupoAcesso>? agremiacoes = null, int tamanho = 0, int aux = 0);
    Task Reativar(int id);
    Task Desativar(int id);
    Task<List<GrupoDeAcessoFiltroDto>> Pesquisar(string valor);
    Task LimparFiltro();
}