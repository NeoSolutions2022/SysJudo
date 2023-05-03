namespace SysJudo.Application.Dto.GruposDeAcesso;

public class GrupoAcessoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public bool Administrador { get; set; }
    public bool Desativado { get; set; }

    public List<GrupoAcessoPermissaoDto> Permissoes { get; set; } = new();
}

public class GrupoAcessoPermissaoDto
{
    public int GrupoAcessoId { get; set; }
    public int PermissaoId { get; set; }
    public string Tipo { get; set; } = null!;
    public PermissaoDto Permissao { get; set; } = null!;
}

public class PermissaoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public string Categoria { get; set; } = null!;
}

public class AlterarGrupoAcessoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public List<ManterGrupoAcessoPermissaoDto> Permissoes { get; set; } = new();
}

public class ManterGrupoAcessoPermissaoDto
{
    public int PermissaoId { get; set; }
    public string Tipo { get; set; } = null!;
}

public class CadastrarGrupoAcessoDto
{
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public List<ManterGrupoAcessoPermissaoDto> Permissoes { get; set; } = new();
}