using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Auth;
using SysJudo.Application.Notifications;

namespace SysJudo.Api.Controllers.V1.Administrador;

[AllowAnonymous]
[Route("v{version:apiVersion}/Administrador/[controller]")]
public class UsuarioAuthController : BaseController
{
    private readonly IAuthService _authService;
    public UsuarioAuthController(INotificator notificator, IAuthService authService) : base(notificator)
    {
        _authService = authService;
    }
    
    [HttpPost("Login")]
    [SwaggerOperation(Summary = "Login.", Tags = new [] { "Administrador - Usuario autenticação" })]
    [ProducesResponseType(typeof(UsuarioAutenticadoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginUsuarioDto)
    {
        var token = await _authService.Login(loginUsuarioDto);
        return token != null ? OkResponse(token) : Unauthorized(new[] { "Usuário e/ou senha incorretos" });
    }
}