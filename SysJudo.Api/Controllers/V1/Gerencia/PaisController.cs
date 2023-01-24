using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Pais;
using SysJudo.Application.Notifications;

namespace SysJudo.Api.Controllers.V1.Gerencia;

[Route("v{version:apiVersion}/Gerencia/[controller]")]
public class PaisController : MainController
{
    private readonly IPaisService _paisService;
    
    public PaisController(INotificator notificator, IPaisService paisService) : base(notificator)
    {
        _paisService = paisService;
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Listar Pais.", Tags = new [] { "Gerencia - Pais" })]
    [ProducesResponseType(typeof(PagedDto<PaisDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Buscar([FromQuery] BuscarPaisDto dto)
    {
        var pais = await _paisService.Buscar(dto);
        return OkResponse(pais);
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter Pais.", Tags = new [] { "Gerencia - Pais" })]
    [ProducesResponseType(typeof(PaisDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var pais = await _paisService.ObterPorId(id);
        return OkResponse(pais);
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Cadastrar Pais.", Tags = new [] { "Gerencia - Pais" })]
    [ProducesResponseType(typeof(PaisDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromBody] CreatePaisDto dto)
    {
        var pais = await _paisService.Adicionar(dto);
        return CreatedResponse("", pais);
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar Pais.", Tags = new [] { "Gerencia - Pais" })]
    [ProducesResponseType(typeof(PaisDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Alterar(int id, [FromBody] UpdatePaisDto dto)
    {
        var pais = await _paisService.Alterar(id, dto);
        return OkResponse(pais);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Remover Pais.", Tags = new[] { "Gerencia - Pais" })]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(int id)
    {
        await _paisService.Remover(id);
        return NoContentResponse();
    }
}