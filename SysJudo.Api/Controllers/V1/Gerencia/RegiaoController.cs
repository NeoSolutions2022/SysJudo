using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Regiao;
using SysJudo.Application.Notifications;

namespace SysJudo.Api.Controllers.V1.Gerencia;

[Route("v{version:apiVersion}/Gerencia/[controller]")]
public class RegiaoController : MainController
{
    private readonly IRegiaoService _regiaoService;
    
    public RegiaoController(INotificator notificator , IRegiaoService regiaoService) : base(notificator)
    {
        _regiaoService = regiaoService;
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Listar Região.", Tags = new [] { "Gerencia - Região" })]
    [ProducesResponseType(typeof(PagedDto<RegiaoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Buscar([FromQuery] BuscarRegiaoDto dto)
    {
        var regiao = await _regiaoService.Buscar(dto);
        return OkResponse(regiao);
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter Região.", Tags = new [] { "Gerencia - Região" })]
    [ProducesResponseType(typeof(RegiaoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var regiao = await _regiaoService.ObterPorId(id);
        return OkResponse(regiao);
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Cadastrar Região.", Tags = new [] { "Gerencia - Região" })]
    [ProducesResponseType(typeof(RegiaoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromBody] CreateRegiaoDto dto)
    {
        var regiao = await _regiaoService.Adicionar(dto);
        return CreatedResponse("", regiao);
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar Região.", Tags = new [] { "Gerencia - Região" })]
    [ProducesResponseType(typeof(RegiaoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Alterar(int id, [FromBody] UpdateRegiaoDto dto)
    {
        var regiao = await _regiaoService.Alterar(id, dto);
        return OkResponse(regiao);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Remover Região.", Tags = new[] { "Gerencia - Região" })]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(int id)
    {
        await _regiaoService.Remover(id);
        return NoContentResponse();
    }
}