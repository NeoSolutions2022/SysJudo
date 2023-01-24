using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Cidade;
using SysJudo.Application.Notifications;

namespace SysJudo.Api.Controllers.V1.Gerencia;

[Route("v{version:apiVersion}/Gerencia/[controller]")]
public class CidadeController : MainController
{
    private readonly ICidadeService _cidadeService;
    
    public CidadeController(INotificator notificator, ICidadeService cidadeService) : base(notificator)
    {
        _cidadeService = cidadeService;
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Listar Cidade.", Tags = new [] { "Gerencia - Cidade" })]
    [ProducesResponseType(typeof(PagedDto<CidadeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Buscar([FromQuery] BuscarCidadeDto dto)
    {
        var cidade = await _cidadeService.Buscar(dto);
        return OkResponse(cidade);
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter Cidade.", Tags = new [] { "Gerencia - Cidade" })]
    [ProducesResponseType(typeof(CidadeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var cidade = await _cidadeService.ObterPorId(id);
        return OkResponse(cidade);
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Cadastrar Cidade.", Tags = new [] { "Gerencia - Cidade" })]
    [ProducesResponseType(typeof(CidadeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromBody] CreateCidadeDto dto)
    {
        var cidade = await _cidadeService.Adicionar(dto);
        return CreatedResponse("", cidade);
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar Cidade.", Tags = new [] { "Gerencia - Cidade" })]
    [ProducesResponseType(typeof(CidadeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Alterar(int id, [FromBody] UpdateCidadeDto dto)
    {
        var cidade = await _cidadeService.Alterar(id, dto);
        return OkResponse(cidade);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Remover Cidade.", Tags = new[] { "Gerencia - Cidade" })]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(int id)
    {
        await _cidadeService.Remover(id);
        return NoContentResponse();
    }
}