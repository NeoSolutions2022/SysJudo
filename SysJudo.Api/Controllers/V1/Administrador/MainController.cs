using Microsoft.AspNetCore.Authorization;
using SysJudo.Application.Notifications;
using SysJudo.Core.Authorization;
using SysJudo.Core.Enums;

namespace SysJudo.Api.Controllers.V1.Administrador;

[Authorize]
[ClaimsAuthorize("TipoUsuario", ETipoUsuario.Administrador)]
public class MainController : BaseController

{
    protected MainController(INotificator notificator) : base(notificator)
    {
    }
}