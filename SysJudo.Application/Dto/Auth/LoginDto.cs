namespace SysJudo.Application.Dto.Auth;

public class LoginDto
{
    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!;
    public string Ip { get; set; }  = null!;
}