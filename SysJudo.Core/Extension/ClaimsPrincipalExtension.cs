using System.Security.Claims;
using Newtonsoft.Json;
using SysJudo.Core.Authorization;

namespace SysJudo.Core.Extension;

public static class ClaimsPrincipalExtension
{
    public static List<PermissaoClaim> Permissoes(this ClaimsPrincipal? user)
    {
        if (user is null)
        {
            return new List<PermissaoClaim>();
        }
        
        return user.Claims
            .Where(c => c.Type == "permissoes")
            .SelectMany(c => JsonConvert.DeserializeObject<List<PermissaoClaim>>(c.Value)!)
            .ToList();
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