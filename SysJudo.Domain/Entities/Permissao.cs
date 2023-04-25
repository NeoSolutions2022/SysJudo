namespace SysJudo.Domain.Entities;

public class Permissao
{
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public string Categoria { get; set; } = null!;
    public virtual List<GrupoAcessoPermissao> Grupos { get; set; } = null!;
}