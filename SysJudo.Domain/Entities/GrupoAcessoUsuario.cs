using System.Runtime;

namespace SysJudo.Domain.Entities;

public class GrupoAcessoUsuario
{
    public int UsuarioId { get; set; }
    public int GrupoAcessoId { get; set; }
    
    public virtual GrupoAcesso GrupoAcesso { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;
}