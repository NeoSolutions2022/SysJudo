using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Permissoes;
using SysJudo.Application.Notifications;

namespace SysJudo.Api.Controllers.V1.Gerencia;

public class PermissaoController : MainController
{
    private readonly IPermissaoService _permissaoService;
    
    public PermissaoController(INotificator notificator, IPermissaoService permissaoService) : base(notificator)
    {
        _permissaoService = permissaoService;
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter uma Permissão.", Tags = new [] { "Gerencia - Permissões" })]
    [ProducesResponseType(typeof(PermissaoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var permissao = await _permissaoService.ObterPorId(id);
        return OkResponse(permissao);
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Buscar uma permissão.", Tags = new [] { "Gerencia - Permissões" })]
    [ProducesResponseType(typeof(PagedDto<PermissaoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<PagedDto<PermissaoDto>> Buscar([FromQuery] BuscarPermissaoDto dto)
    {
        return await _permissaoService.Buscar(dto);
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Cadastro de uma Permissão.", Tags = new [] { "Gerencia - Permissões" })]
    [ProducesResponseType(typeof(PermissaoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarPermissaoDto dto)
    {
        var permissao = await _permissaoService.Adicionar(dto);
        return CreatedResponse("", permissao);
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Alterar uma Permissão.", Tags = new [] { "Gerencia - Permissões" })]
    [ProducesResponseType(typeof(PermissaoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Alterar(int id, [FromBody] AlterarPermissaoDto dto)
    {
        var permissao = await _permissaoService.Alterar(id, dto);
        return OkResponse(permissao);
    }
    
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Deletar uma Permissão.", Tags = new [] { "Gerencia - Permissões" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desativar(int id)
    {
        await _permissaoService.Deletar(id);
        return NoContentResponse();
    }

    
}