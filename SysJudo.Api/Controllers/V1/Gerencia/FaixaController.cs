using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Faixa;
using SysJudo.Application.Notifications;

namespace SysJudo.Api.Controllers.V1.Gerencia;

[Route("v{version:apiVersion}/Gerencia/[controller]")]
public class FaixaController : MainController
{
    private readonly IFaixaService _faixaService;
    public FaixaController(INotificator notificator, IFaixaService faixaService) : base(notificator)
    {
        _faixaService = faixaService;
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Listar Faixa.", Tags = new [] { "Gerencia - Faixa" })]
    [ProducesResponseType(typeof(PagedDto<FaixaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Buscar([FromQuery] BuscarFaixaDto dto)
    {
        var faixa = await _faixaService.Buscar(dto);
        return OkResponse(faixa);
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter Faixa.", Tags = new [] { "Gerencia - Faixa" })]
    [ProducesResponseType(typeof(FaixaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var faixa = await _faixaService.ObterPorId(id);
        return OkResponse(faixa);
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Cadastrar Faixa.", Tags = new [] { "Gerencia - Faixa" })]
    [ProducesResponseType(typeof(FaixaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromBody] CreateFaixaDto dto)
    {
        var faixa = await _faixaService.Adicionar(dto);
        return CreatedResponse("", faixa);
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar Faixa.", Tags = new [] { "Gerencia - Faixa" })]
    [ProducesResponseType(typeof(FaixaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Alterar(int id, [FromBody] UpdateFaixaDto dto)
    {
        var faixa = await _faixaService.Alterar(id, dto);
        return OkResponse(faixa);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Remover Faixa.", Tags = new[] { "Gerencia - Faixa" })]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(int id)
    {
        await _faixaService.Remover(id);
        return NoContentResponse();
    }
}