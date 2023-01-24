using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Estado;
using SysJudo.Application.Notifications;

namespace SysJudo.Api.Controllers.V1.Gerencia;

[Route("v{version:apiVersion}/Gerencia/[controller]")]
public class EstadoController : MainController
{
    private readonly IEstadoService _estadoService;
    
    public EstadoController(INotificator notificator, IEstadoService estadoService) : base(notificator)
    {
        _estadoService = estadoService;
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Listar Estado.", Tags = new [] { "Gerencia - Estado" })]
    [ProducesResponseType(typeof(PagedDto<EstadoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Buscar([FromQuery] BuscarEstadoDto dto)
    {
        var estado = await _estadoService.Buscar(dto);
        return OkResponse(estado);
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter Estado.", Tags = new [] { "Gerencia - Estado" })]
    [ProducesResponseType(typeof(EstadoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var estado = await _estadoService.ObterPorId(id);
        return OkResponse(estado);
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Cadastrar Estado.", Tags = new [] { "Gerencia - Estado" })]
    [ProducesResponseType(typeof(EstadoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromBody] CreateEstadoDto dto)
    {
        var estado = await _estadoService.Adicionar(dto);
        return CreatedResponse("", estado);
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar Estado.", Tags = new [] { "Gerencia - Estado" })]
    [ProducesResponseType(typeof(EstadoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Alterar(int id, [FromBody] UpdadeEstadoDto dto)
    {
        var estado = await _estadoService.Alterar(id, dto);
        return OkResponse(estado);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Remover Estado.", Tags = new[] { "Gerencia - Estado" })]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(int id)
    {
        await _estadoService.Remover(id);
        return NoContentResponse();
    }
}