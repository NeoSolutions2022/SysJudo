using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Administrador;
using SysJudo.Application.Dto.Auth;
using SysJudo.Application.Notifications;

namespace SysJudo.Api.Controllers.V1.Administrador;

[AllowAnonymous]
[Route("v{version:apiVersion}/Administrador/[controller]")]
public class AuthController : BaseController
{
    private readonly IAdministradorAuthService _authService;
    public AuthController(INotificator notificator, IAdministradorAuthService authService) : base(notificator)
    {
        _authService = authService;
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Login - Administrador.", Tags = new [] { "Administrador - Autenticação" })]
    [ProducesResponseType(typeof(UsuarioAutenticadoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginAdministradorDto loginAdministrador)
    {
        var token = await _authService.Login(loginAdministrador);
        return token != null ? OkResponse(token) : Unauthorized(new[] { "Usuário e/ou senha incorretos" });
    }
}