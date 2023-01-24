using SysJudo.Application.Notifications;
using SysJudo.Core.Authorization;
using SysJudo.Core.Enums;

namespace SysJudo.Api.Controllers.V1;

[ClaimsAuthorize("TipoUsuario", ETipoUsuario.Comum)]
public class MainController : BaseController
{
    protected MainController(INotificator notificator) : base(notificator)
    {
    }
}