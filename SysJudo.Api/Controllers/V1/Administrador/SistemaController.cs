using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Sistema;
using SysJudo.Application.Notifications;

namespace SysJudo.Api.Controllers.V1.Administrador;

[Route("v{version:apiVersion}/Administrador/[controller]")]
public class SistemaController : MainController
{
    private readonly ISistemaService _sistemaService;
    public SistemaController(INotificator notificator, ISistemaService sistemaService) : base(notificator)
    {
        _sistemaService = sistemaService;
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Listar Sistema.", Tags = new [] { "Administrador - Sistema" })]
    [ProducesResponseType(typeof(PagedDto<SistemaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Buscar([FromQuery] BuscarSistemaDto dto)
    {
        var sistema = await _sistemaService.Buscar(dto);
        return OkResponse(sistema);
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter Sistema.", Tags = new [] { "Administrador - Sistema" })]
    [ProducesResponseType(typeof(SistemaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var sistema = await _sistemaService.ObterPorId(id);
        return OkResponse(sistema);
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Cadastrar Sistema.", Tags = new [] { "Administrador - Sistema" })]
    [ProducesResponseType(typeof(SistemaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromBody] CreateSistemaDto dto)
    {
        var sistema = await _sistemaService.Adicionar(dto);
        return CreatedResponse("", sistema);
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar Sistema.", Tags = new [] { "Administrador - Sistema" })]
    [ProducesResponseType(typeof(SistemaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Alterar(int id, [FromBody] UpdateSistemaDto dto)
    {
        var usuario = await _sistemaService.Alterar(id, dto);
        return OkResponse(usuario);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Remover Sistema.", Tags = new[] { "Administrador - Sistema" })]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(int id)
    {
        await _sistemaService.Remover(id);
        return NoContentResponse();
    }
}