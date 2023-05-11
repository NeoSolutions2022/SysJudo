using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.RegistroDeEvento;
using SysJudo.Application.Notifications;
using SysJudo.Core.Authorization;

namespace SysJudo.Api.Controllers.V1.Administrador;

public class RegistroDeEventosController : BaseController
{
    private readonly IRegistroDeEventoService _service;
    public RegistroDeEventosController(INotificator notificator, IRegistroDeEventoService service) : base(notificator)
    {
        _service = service;
    }
    
    [HttpGet("{id}")]
    [ClaimsAuthorize("GrupoAcesso", "GrupoAcesso")]
    [SwaggerOperation(Summary = "Obter Registro de evento.", Tags = new [] { "Administrador - Registro de evento" })]
    [ProducesResponseType(typeof(RegistroDeEventoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var registroDeEvento = await _service.ObterPorId(id);
        return OkResponse(registroDeEvento);
    }
    
    [HttpGet]
    [ClaimsAuthorize("GrupoAcesso", "GrupoAcesso")]
    [SwaggerOperation(Summary = "Obter todos Registro de evento.", Tags = new [] { "Administrador - Registro de evento" })]
    [ProducesResponseType(typeof(RegistroDeEventoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterTodos()
    {
        var registroDeEvento = await _service.ObterTodos();
        return OkResponse(registroDeEvento);
    }
}