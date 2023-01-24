using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Usuario;
using SysJudo.Application.Notifications;

namespace SysJudo.Api.Controllers.V1.Administrador;

[Route("v{version:apiVersion}/Administrador/[controller]")]
public class UsuarioController : MainController
{
    private readonly IUsuarioService _usuarioService;
    
    public UsuarioController(INotificator notificator, IUsuarioService usuarioService) : base(notificator)
    {
        _usuarioService = usuarioService;
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Listar Usuario.", Tags = new [] { "Administrador - Usuario" })]
    [ProducesResponseType(typeof(PagedDto<UsuarioDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Buscar([FromQuery] BuscarUsuarioDto dto)
    {
        var usuario = await _usuarioService.Buscar(dto);
        return OkResponse(usuario);
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter Usuario.", Tags = new [] { "Administrador - Usuario" })]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var usuario = await _usuarioService.ObterPorId(id);
        return OkResponse(usuario);
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Cadastrar Usuario.", Tags = new [] { "Administrador - Usuario" })]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromBody] CreateUsuarioDto dto)
    {
        var usuario = await _usuarioService.Adicionar(dto);
        return CreatedResponse("", usuario);
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar Usuario.", Tags = new [] { "Administrador - Usuario" })]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Alterar(int id, [FromBody] UpdateUsuarioDto dto)
    {
        var usuario = await _usuarioService.Alterar(id, dto);
        return OkResponse(usuario);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Remover Usuario.", Tags = new[] { "Administrador - Usuario" })]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(int id)
    {
        await _usuarioService.Remover(id);
        return NoContentResponse();
    }
}