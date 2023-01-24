using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Atleta;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Notifications;

namespace SysJudo.Api.Controllers.V1.Gerencia;

[Route("v{version:apiVersion}/Gerencia/[controller]")]
public class AtletaController : MainController
{
    private readonly IAtletaService _atletaService;
    public AtletaController(INotificator notificator, IAtletaService atletaService) : base(notificator)
    {
        _atletaService = atletaService;
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Listar Atleta.", Tags = new[] { "Gerencia - Atleta" })]
    [ProducesResponseType(typeof(PagedDto<AtletaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Buscar([FromQuery] BuscarAtletaDto dto)
    {
        var atleta = await _atletaService.Buscar(dto);
        return OkResponse(atleta);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter Atleta.", Tags = new[] { "Gerencia - Atleta" })]
    [ProducesResponseType(typeof(AtletaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var atleta = await _atletaService.ObterPorId(id);
        return OkResponse(atleta);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Cadastrar Atleta.", Tags = new[] { "Gerencia - Atleta" })]
    [ProducesResponseType(typeof(AtletaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromForm] CreateAtletaDto dto)
    {
        var atleta = await _atletaService.Adicionar(dto);
        return CreatedResponse("", atleta);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar Atleta.", Tags = new[] { "Gerencia - Atleta" })]
    [ProducesResponseType(typeof(AtletaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Alterar(int id, [FromForm] UpdateAtletaDto dto)
    {
        var atleta = await _atletaService.Alterar(id, dto);
        return OkResponse(atleta);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Remover Atleta.", Tags = new[] { "Gerencia - Atleta" })]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(int id)
    {
        await _atletaService.Remover(id);
        return NoContentResponse();
    }
}