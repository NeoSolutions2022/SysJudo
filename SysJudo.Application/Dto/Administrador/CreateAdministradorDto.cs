namespace SysJudo.Application.Dto.Administrador;

public class CreateAdministradorDto
{
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!;
}