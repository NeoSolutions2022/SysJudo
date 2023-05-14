namespace SysJudo.Application.Dto.Usuario;

public class CreateUsuarioDto
{
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!;
    public bool Inadiplente { get; set; }
    public int ClienteId { get; set; }
    public List<GrupoAcessoUsuarioDto> GrupoAcessos { get; set; } = new();
}

public class GrupoAcessoUsuarioDto
{
    public int GrupoAcessoId { get; set; }
}