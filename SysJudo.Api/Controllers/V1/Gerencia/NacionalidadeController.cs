using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Nacionalidade;
using SysJudo.Application.Notifications;

namespace SysJudo.Api.Controllers.V1.Gerencia;

[Route("v{version:apiVersion}/Gerencia/[controller]")]
public class NacionalidadeController : MainController
{
    private readonly INacionalidadeService _nacionalidadeService;
    public NacionalidadeController(INotificator notificator, INacionalidadeService nacionalidadeService) : base(notificator)
    {
        _nacionalidadeService = nacionalidadeService;
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Listar Nacionalidade.", Tags = new[] { "Gerencia - Nacionalidade" })]
    [ProducesResponseType(typeof(PagedDto<NacionalidadeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Buscar([FromQuery] BuscarNacionalidadeDto dto)
    {
        var Nacionalidade = await _nacionalidadeService.Buscar(dto);
        return OkResponse(Nacionalidade);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter Nacionalidade.", Tags = new[] { "Gerencia - Nacionalidade" })]
    [ProducesResponseType(typeof(NacionalidadeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var Nacionalidade = await _nacionalidadeService.ObterPorId(id);
        return OkResponse(Nacionalidade);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Cadastrar Nacionalidade.", Tags = new[] { "Gerencia - Nacionalidade" })]
    [ProducesResponseType(typeof(NacionalidadeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromBody] CreateNacionalidadeDto dto)
    {
        var Nacionalidade = await _nacionalidadeService.Adicionar(dto);
        return CreatedResponse("", Nacionalidade);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar Nacionalidade.", Tags = new[] { "Gerencia - Nacionalidade" })]
    [ProducesResponseType(typeof(NacionalidadeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Alterar(int id, [FromBody] UpdateNacionalidadeDto dto)
    {
        var Nacionalidade = await _nacionalidadeService.Alterar(id, dto);
        return OkResponse(Nacionalidade);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Remover Nacionalidade.", Tags = new[] { "Gerencia - Nacionalidade" })]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(int id)
    {
        await _nacionalidadeService.Remover(id);
        return NoContentResponse();
    }
}