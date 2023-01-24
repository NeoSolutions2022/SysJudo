using Microsoft.AspNetCore.Http;
using SysJudo.Core.Enums;

namespace SysJudo.Core.Extension;

public static class HttpContextAccessorExtensions
{
    public static bool UsuarioAutenticado(this IHttpContextAccessor? contextAccessor)
    {
        return contextAccessor?.HttpContext?.User?.UsuarioAutenticado() ?? false;
    }
    
    public static int? ObterUsuarioId(this IHttpContextAccessor? contextAccessor)
    {
        var id = contextAccessor?.HttpContext?.User?.ObterUsuarioId() ?? string.Empty;
        return string.IsNullOrWhiteSpace(id) ? null : int.Parse(id);
    }
    
    public static int? ObterClienteId(this IHttpContextAccessor? contextAccessor)
    {
        var clienteId = contextAccessor?.HttpContext?.User?.ObterClienteId() ?? string.Empty;
        return string.IsNullOrWhiteSpace(clienteId) ? null : int.Parse(clienteId); 
    }

    public static ETipoUsuario? ObterTipoUsuario(this IHttpContextAccessor? contextAccessor)
    {
        var tipo = contextAccessor?.HttpContext?.User?.ObterTipoUsuario() ?? string.Empty;
        return string.IsNullOrWhiteSpace(tipo) ? null : Enum.Parse<ETipoUsuario>(tipo);
    }
    
    public static bool EhAdministrador(this IHttpContextAccessor? contextAccessor)
    {
        return ObterTipoUsuario(contextAccessor) is ETipoUsuario.Administrador;
    }
}