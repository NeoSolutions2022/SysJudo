using SysJudo.Application.Dto.Agremiacao;
using SysJudo.Application.Dto.Base;
using SysJudo.Domain.Entities.EntitiesFiltros;

namespace SysJudo.Application.Contracts;

public interface IAgremiacaoService
{
    Task<List<AgremiacaoFiltroDto>> Filtrar(List<FiltragemAgremiacaoDto> dto, List<AgremiacaoFiltro>? agremiacoes = null, int tamanho = 0, int aux = 0);
    Task<AgremiacaoDto?> Cadastrar(CadastrarAgremiacaoDto dto);
    Task<AgremiacaoDto?> Alterar(int id, AlterarAgremiacaoDto dto);
    Task<PagedDto<AgremiacaoDto>> Buscar(BuscarAgremiacaoDto dto);
    Task<List<AgremiacaoFiltroDto>> Pesquisar(string valor, List<AgremiacaoFiltro>? agremiacoes = null);
    Task<AgremiacaoDto?> ObterPorId(int id);
    Task LimparFiltro();
    Task Deletar(int id);
    Task<string> Exportar(ExportarAgremiacaoDto dto);
    Task Anotar(int id, AnotarAgremiacaoDto anotacao);
    Task EnviarDocumentos(int id, EnviarDocumentosDto dto);
    Task DeletarDocumento(int id, int documentoId);
    Task DownloadDocumento(DownloadDocumentoDto nomeDocumento);
}