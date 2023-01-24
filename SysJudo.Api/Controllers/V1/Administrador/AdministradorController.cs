using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Administrador;
using SysJudo.Application.Notifications;

namespace SysJudo.Api.Controllers.V1.Administrador;

[Route("v{version:apiVersion}/Administrador/[controller]")]
public class AdministradorController : MainController
{
    private readonly IAdministradorService _administradorService;
    
    public AdministradorController(INotificator notificator, IAdministradorService administradorService) : base(notificator)
    {
        _administradorService = administradorService;
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter um Administrador.", Tags = new [] { "Administrador - Administrador" })]
    [ProducesResponseType(typeof(AdministradorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var administrador = await _administradorService.ObterPorId(id);
        return OkResponse(administrador);
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Cadastrar Administrador.", Tags = new [] { "Administrador - Administrador" })]
    [ProducesResponseType(typeof(AdministradorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromBody] CreateAdministradorDto dto)
    {
        var administrador = await _administradorService.Adicionar(dto);
        return CreatedResponse("", administrador);
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar Administrador.", Tags = new [] { "Administrador - Administrador" })]
    [ProducesResponseType(typeof(AdministradorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Alterar(int id, [FromBody] UpdateAdministradorDto dto)
    {
        var administrador = await _administradorService.Alterar(id, dto);
        return OkResponse(administrador);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Remover Administrador.", Tags = new[] { "Administrador - Administrador" })]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(int id)
    {
        await _administradorService.Remover(id);
        return NoContentResponse();
    }
}