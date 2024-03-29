using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Agremiacao;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Notifications;
using SysJudo.Core.Authorization;

namespace SysJudo.Api.Controllers.V1.Gerencia;

[Route("v{version:apiVersion}/Gerencia/[controller]")]
public class AgremiacaoController : MainController
{
    private readonly IAgremiacaoService _service;

    public AgremiacaoController(INotificator notificator, IAgremiacaoService service) : base(notificator)
    {
        _service = service;
    }

    [HttpGet]
    [ClaimsAuthorize(PermissoesBackend.AgremiacoesListar, EPermissaoTipo.Write)]
    [SwaggerOperation(Summary = "Listar Agremiação.", Tags = new[] { "Gerencia - Agremiação" })]
    [ProducesResponseType(typeof(PagedDto<AgremiacaoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Buscar([FromQuery] BuscarAgremiacaoDto dto)
    {
        var agremiacao = await _service.Buscar(dto);
        return OkResponse(agremiacao);
    }
    
    [HttpGet("pesquisar-{valor}")]
    [ClaimsAuthorize(PermissoesBackend.AgremiacoesPesquisar, EPermissaoTipo.Write)]
    [SwaggerOperation(Summary = "Pesquisar Agremiação.", Tags = new[] { "Gerencia - Agremiação" })]
    [ProducesResponseType(typeof(List<AgremiacaoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Buscar(string valor)
    {
        var agremiacao = await _service.Pesquisar(valor);
        return OkResponse(agremiacao);
    }

    [HttpPost("filtrar/agremiacao")]
    //[ClaimsAuthorize(PermissoesBackend.AgremiacoesFiltrar, EPermissaoTipo.Write)]
    [SwaggerOperation(Summary = "Filtrar Agremiação.", Tags = new[] { "Gerencia - Agremiação" })]
    [ProducesResponseType(typeof(List<AgremiacaoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Filtragem([FromBody] List<FiltragemAgremiacaoDto> dtos)
    {
        var agremiacaoLista = await _service.Filtrar(dtos);
        return OkResponse(agremiacaoLista);
    }

    [HttpPost("limpar-filtro")]
    [ClaimsAuthorize(PermissoesBackend.AgremiacoesLimparFiltro, EPermissaoTipo.Write)]
    [SwaggerOperation(Summary = "Limpar filtro.", Tags = new[] { "Gerencia - Agremiação" })]
    [ProducesResponseType(typeof(List<AgremiacaoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Filtragem()
    {
        await _service.LimparFiltro();
        return NoContentResponse();
    }

    [HttpGet("{id}")]
    [ClaimsAuthorize(PermissoesBackend.AgremiacoesListar, EPermissaoTipo.Write)]
    [SwaggerOperation(Summary = "Obter Agremiação.", Tags = new[] { "Gerencia - Agremiação" })]
    [ProducesResponseType(typeof(AgremiacaoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var agremiacao = await _service.ObterPorId(id);
        return OkResponse(agremiacao);
    }

    [HttpGet("exportar")]
   // [ClaimsAuthorize(PermissoesBackend.AgremiacoesExportar, EPermissaoTipo.Write)]
    [SwaggerOperation(Summary = "Exportar Agremiação.", Tags = new[] { "Gerencia - Agremiação" })]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Exportar([FromQuery] ExportarAgremiacaoDto dto)
    {
        var agremiacao = await _service.Exportar(dto);

        return OkResponse(agremiacao);
    }

    [HttpPost]
    [ClaimsAuthorize(PermissoesBackend.AgremiacoesAdicionar, EPermissaoTipo.Write)]
    [SwaggerOperation(Summary = "Cadastrar Agremiação.", Tags = new[] { "Gerencia - Agremiação" })]
    [ProducesResponseType(typeof(AgremiacaoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromForm] CadastrarAgremiacaoDto dto)
    {
        var agremiacao = await _service.Cadastrar(dto);
        return CreatedResponse("", agremiacao);
    }

    [HttpPut("{id}")]
    [ClaimsAuthorize(PermissoesBackend.AgremiacoesAlterar, EPermissaoTipo.Write)]
    [SwaggerOperation(Summary = "Atualizar Agremiação.", Tags = new[] { "Gerencia - Agremiação" })]
    [ProducesResponseType(typeof(AgremiacaoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Alterar(int id, [FromForm] AlterarAgremiacaoDto dto)
    {
        var agremiacao = await _service.Alterar(id, dto);
        return OkResponse(agremiacao);
    }

    [HttpPatch("{id}")]
    [ClaimsAuthorize(PermissoesBackend.AgremiacoesAnotar, EPermissaoTipo.Write)]
    [SwaggerOperation(Summary = "Anotações Agremiação.", Tags = new[] { "Gerencia - Agremiação" })]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Anotar(int id, [FromBody] AnotarAgremiacaoDto dto)
    {
        await _service.Anotar(id, dto);
        return NoContentResponse();
    }

    [HttpPatch("{id}/removerdocumentos")]
    [ClaimsAuthorize(PermissoesBackend.AgremiacoesRemoverDocs, EPermissaoTipo.Write)]
    [SwaggerOperation(Summary = "Anexos Agremiação.", Tags = new[] { "Gerencia - Agremiação" })]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoverAnexo(int id, int documentoId)
    {
        await _service.DeletarDocumento(id, documentoId);
        return NoContentResponse();
    }
    
    [HttpPatch("{id}/EnviarDocumentos")]
    [ClaimsAuthorize(PermissoesBackend.AgremiacoesEnviarDocs, EPermissaoTipo.Write)]
    [SwaggerOperation(Summary = "Enviar documentos Agremiação.", Tags = new[] { "Gerencia - Agremiação" })]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> EnviarDocumentos(int id, [FromForm] EnviarDocumentosDto dto)
    {
        await _service.EnviarDocumentos(id, dto);
        return NoContentResponse(); 
    }
    
    [HttpPatch("documentos/download")]
    [ClaimsAuthorize(PermissoesBackend.AgremiacoesRemoverDocs, EPermissaoTipo.Write)]
    [SwaggerOperation(Summary = "Registrar download de um documento.", Tags = new[] { "Gerencia - Agremiação" })]
    [ProducesResponseType(typeof(List<AgremiacaoDto>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Filtragem([FromBody] DownloadDocumentoDto dto)
    {
        await _service.DownloadDocumento(dto);
        return OkResponse();
    }

    [HttpDelete("{id}")]
    [ClaimsAuthorize(PermissoesBackend.AgremiacoesRemover, EPermissaoTipo.Write)]
    [SwaggerOperation(Summary = "Remover Agremiação.", Tags = new[] { "Gerencia - Agremiação" })]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(int id)
    {
        await _service.Deletar(id);
        return NoContentResponse();
    }
}