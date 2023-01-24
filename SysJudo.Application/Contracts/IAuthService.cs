using SysJudo.Application.Dto.Auth;

namespace SysJudo.Application.Contracts;

public interface IAuthService
{
    Task<UsuarioAutenticadoDto?> Login(LoginDto loginDto);
}