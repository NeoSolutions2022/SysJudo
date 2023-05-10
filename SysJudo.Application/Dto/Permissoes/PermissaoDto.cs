namespace SysJudo.Application.Dto.Permissoes;

public class PermissaoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public string Categoria { get; set; } = null!;
}
