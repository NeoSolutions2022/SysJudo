namespace SysJudo.Domain.Entities;

public class GrupoAcessoPermissao
{
    public int GrupoAcessoId { get; set; }
    public int PermissaoId { get; set; }
    public string Tipo { get; set; } = null!;

    public DateTime CriadoEm { get; set; }
    public int? CriadoPor { get; set; }
    public bool CriadoPorAdmin { get; set; }
    public DateTime AtualizadoEm { get; set; }
    public int? AtualizadoPor { get; set; }

    public virtual GrupoAcesso GrupoAcesso { get; set; } = null!;
    public virtual Permissao Permissao { get; set; } = null!;
    
}