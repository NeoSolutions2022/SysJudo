using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SysJudo.Api.Responses;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.GruposDeAcesso;
using SysJudo.Application.Notifications;
using SysJudo.Core.Authorization;

namespace SysJudo.Api.Controllers.V1.Gerencia;

public class GruposAcessoController : BaseController
{
    private readonly IGrupoAcessoService _grupoAcessoService;
    public GruposAcessoController(INotificator notificator, IGrupoAcessoService grupoAcessoService) : base(notificator)
    {
        _grupoAcessoService = grupoAcessoService;
    }
    
    [HttpGet("{id}")]
    [ClaimsAuthorize("GrupoAcesso", "GrupoAcesso")]
    // [ClaimsAuthorize(PermissoesBackend.ConfiguracoesGruposAcesso, EPermissaoTipo.Read)]
    [SwaggerOperation(Summary = "Obter um grupo de acesso.", Tags = new [] { "Configurações - Grupos de acesso" })]
    [ProducesResponseType(typeof(GrupoAcessoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId( int id)
    {
        var grupoAcesso = await _grupoAcessoService.ObterPorId(id);
        return OkResponse(grupoAcesso);
    }

    [HttpGet]
    // [ClaimsAuthorize(PermissoesBackend.ConfiguracoesGruposAcesso, EPermissaoTipo.Read)]
    [SwaggerOperation(Summary = "Buscar um grupo de acesso.", Tags = new [] { "Configurações - Grupos de acesso" })]
    [ProducesResponseType(typeof(PagedDto<GrupoAcessoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<PagedDto<GrupoAcessoDto>> Buscar([FromQuery] BuscarGrupoAcessoDto dto)
    {
        return await _grupoAcessoService.Buscar(dto);
    }
    
    [HttpPost]
    [ClaimsAuthorize("GrupoAcesso", "GrupoAcesso")]
    //[ClaimsAuthorize(PermissoesBackend.ConfiguracoesGruposAcesso, EPermissaoTipo.Write)]
    [SwaggerOperation(Summary = "Adicionar de um grupo de acesso.", Tags = new [] { "Configurações - Grupos de acesso" })]
    [ProducesResponseType(typeof(GrupoAcessoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Adicionar([FromBody] CadastrarGrupoAcessoDto dto)
    {
        var grupoAcesso = await _grupoAcessoService.Cadastrar(dto);
        return CreatedResponse(string.Empty, grupoAcesso);
    }
    
    [HttpPut("{id}")]
    [ClaimsAuthorize("GrupoAcesso", "GrupoAcesso")]
    // [ClaimsAuthorize(PermissoesBackend.ConfiguracoesGruposAcesso, EPermissaoTipo.Write)]
    [SwaggerOperation(Summary = "Alterar um grupo de acesso.", Tags = new [] { "Configurações - Grupos de acesso" })]
    [ProducesResponseType(typeof(GrupoAcessoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Alterar(int id, [FromBody] AlterarGrupoAcessoDto dto)
    {
        var grupoAcesso = await _grupoAcessoService.Alterar(id, dto);
        return OkResponse(grupoAcesso);
    }
    
    [HttpDelete("{id}")]
    [ClaimsAuthorize("GrupoAcesso", "GrupoAcesso")]
    // [ClaimsAuthorize(PermissoesBackend.ConfiguracoesGruposAcesso, EPermissaoTipo.Delete)]
    [SwaggerOperation(Summary = "Desativar um grupo de acesso.", Tags = new [] { "Configurações - Grupos de acesso" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desativar(int id)
    {
        await _grupoAcessoService.Desativar(id);
        return NoContentResponse();
    }
    
    [HttpPatch("{id}/reativar")]
    [ClaimsAuthorize("GrupoAcesso", "GrupoAcesso")]
    // [ClaimsAuthorize(PermissoesBackend.ConfiguracoesGruposAcesso, EPermissaoTipo.Write)]
    [SwaggerOperation(Summary = "Reativar um grupo de acesso.", Tags = new [] { "Configurações - Grupos de acesso" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reativar(int id)
    {
        await _grupoAcessoService.Reativar(id);
        return NoContentResponse();
    }
    
    [HttpGet("pesquisar-{valor}")]
    [ClaimsAuthorize("GrupoAcesso", "GrupoAcesso")]
    [SwaggerOperation(Summary = "Pesquisar grupo de acesso.", Tags = new[] { "Configurações - Grupos de acesso" })]
    [ProducesResponseType(typeof(List<GrupoAcessoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Buscar(string valor)
    {
        var grupoAcesso = await _grupoAcessoService.Pesquisar(valor);
        return OkResponse(grupoAcesso);
    }
    
    [HttpPost("filtrar/grupo-acesso")]
    [ClaimsAuthorize("GrupoAcesso", "GrupoAcesso")]
    [SwaggerOperation(Summary = "Filtrar grupo de acesso.", Tags = new[] { "Configurações - Grupos de acesso" })]
    [ProducesResponseType(typeof(List<GrupoAcessoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Filtragem([FromBody] List<FiltragemGrupoDeAcessoDto> dtos)
    {
        var agremiacaoLista = await _grupoAcessoService.Filtrar(dtos);
        return OkResponse(agremiacaoLista);
    }

    [HttpPost("limpar-grupo-acesso")]
    [ClaimsAuthorize("GrupoAcesso", "GrupoAcesso")]
    [SwaggerOperation(Summary = "Limpar grupo de acesso.", Tags = new[] { "Configurações - Grupos de acesso" })]
    [ProducesResponseType(typeof(List<GrupoAcessoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Filtragem()
    {
        await _grupoAcessoService.LimparFiltro();
        return NoContentResponse();
    }
    
}