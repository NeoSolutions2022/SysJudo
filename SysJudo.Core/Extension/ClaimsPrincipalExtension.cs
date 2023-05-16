using System.Security.Claims;
using Newtonsoft.Json;
using SysJudo.Core.Authorization;

namespace SysJudo.Core.Extension;

public static class ClaimsPrincipalExtension
{
    public static bool VerificarPermissao(this ClaimsPrincipal? user, string claimName, string claimValue)
    {
        if (user is null)
        {
            return false;
        }
        
        return user
            .Claims
            .Where(p => p.Type == "permissoes")
            .Any(p => PermissaoClaim.Verificar(p.Value, claimName, claimValue));
    }

    public static bool UsuarioAutenticado(this ClaimsPrincipal? principal)
    {
        return principal?.Identity?.IsAuthenticated ?? false;
    }
    
    public static string? ObterTipoUsuario(this ClaimsPrincipal? principal)
        => GetClaim(principal, "TipoUsuario");
    
    public static string? ObterTipoGrupoAcesso(this ClaimsPrincipal? principal)
        => GetClaim(principal, "GrupoAcesso");
    
    public static string? ObterUsuarioId(this ClaimsPrincipal? principal) 
        => GetClaim(principal, ClaimTypes.NameIdentifier);
    
    public static string? ObterNome(this ClaimsPrincipal? principal) 
        => GetClaim(principal, ClaimTypes.Name);

    public static string? ObterClienteId(this ClaimsPrincipal? principal) 
        => GetClaim(principal, "ClienteId");

    private static string? GetClaim(ClaimsPrincipal? principal, string claimName)
    {
        if (principal == null)
        {
            throw new ArgumentException(null, nameof(principal));
        }

        var claim = principal.FindFirst(claimName);
        return claim?.Value;
    }
}