using Microsoft.AspNetCore.Http;
using SysJudo.Core.Enums;
using SysJudo.Core.Extension;

namespace SysJudo.Core.Authorization;

public interface IAuthenticatedUser
{
    public int Id { get; }
    public int ClienteId { get; }
    public ETipoUsuario? TipoUsuario { get; }

    public bool UsuarioLogado { get; }
    public bool UsuarioComum { get; }
    public bool UsuarioAdministrador { get; }
}

public class  AuthenticatedUser : IAuthenticatedUser
{
    public int Id { get; } = -1;
    public int ClienteId { get; } = -1;
    public ETipoUsuario? TipoUsuario { get; }

    public bool UsuarioLogado => Id > 0;
    public bool UsuarioComum => TipoUsuario is ETipoUsuario.Comum;
    public bool UsuarioAdministrador => TipoUsuario is ETipoUsuario.Administrador;
    
    public AuthenticatedUser()
    { }
    
    public AuthenticatedUser(IHttpContextAccessor httpContextAccessor)
    {
        Id = httpContextAccessor.ObterUsuarioId()!.Value;
        ClienteId = httpContextAccessor.EhAdministrador() ? -1 : httpContextAccessor.ObterClienteId()!.Value;
        TipoUsuario = httpContextAccessor.ObterTipoUsuario()!.Value;
    }
}