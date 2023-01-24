using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.EmissoresIdentidade;
using SysJudo.Application.Notifications;

namespace SysJudo.Api.Controllers.V1.Gerencia;

[Route("v{version:apiVersion}/Gerencia/[controller]")]
public class EmissoresIdentidade : MainController
{
    private readonly IEmissoresIdentidadeService _emissoresIdentidadeService;

    public EmissoresIdentidade(INotificator notificator, IEmissoresIdentidadeService emissoresIdentidadeService) :
        base(notificator)
    {
        _emissoresIdentidadeService = emissoresIdentidadeService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Listar Emissores Identidade.", Tags = new[] { "Gerencia - Emissores Identidade" })]
    [ProducesResponseType(typeof(PagedDto<EmissoresIdentidadeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Buscar([FromQuery] BuscarEmissoresIdentidadeDto dto)
    {
        var emissoresIdentidade = await _emissoresIdentidadeService.Buscar(dto);
        return OkResponse(emissoresIdentidade);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter Emissores Identidade.", Tags = new[] { "Gerencia - Emissores Identidade" })]
    [ProducesResponseType(typeof(EmissoresIdentidadeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var emissoresIdentidade = await _emissoresIdentidadeService.ObterPorId(id);
        return OkResponse(emissoresIdentidade);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Cadastrar Emissores Identidade.", Tags = new[] { "Gerencia - Emissores Identidade" })]
    [ProducesResponseType(typeof(EmissoresIdentidadeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromBody] CreateEmissoresIdentidadeDto dto)
    {
        var emissoresIdentidade = await _emissoresIdentidadeService.Adicionar(dto);
        return CreatedResponse("", emissoresIdentidade);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar Emissores Identidade.", Tags = new[] { "Gerencia - Emissores Identidade" })]
    [ProducesResponseType(typeof(EmissoresIdentidadeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Alterar(int id, [FromBody] UpdateEmissoresIdentidadeDto dto)
    {
        var emissoresIdentidade = await _emissoresIdentidadeService.Alterar(id, dto);
        return OkResponse(emissoresIdentidade);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Remover Emissores Identidade.", Tags = new[] { "Gerencia - Emissores Identidade" })]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(int id)
    {
        await _emissoresIdentidadeService.Remover(id);
        return NoContentResponse();
    }
}