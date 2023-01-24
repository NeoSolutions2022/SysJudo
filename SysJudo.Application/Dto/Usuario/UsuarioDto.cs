namespace SysJudo.Application.Dto.Usuario;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime? UltimoLogin { get; set; }
    public DateTime? DataExpiracao { get; set; }
    public DateTime CriadoEm { get; set; }
    public bool Inadiplente { get; set; }
    public int ClienteId { get; set; }
}