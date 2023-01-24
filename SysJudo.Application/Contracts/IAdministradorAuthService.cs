using SysJudo.Application.Dto.Administrador;
using SysJudo.Application.Dto.Auth;

namespace SysJudo.Application.Contracts;

public interface IAdministradorAuthService
{
    Task<UsuarioAutenticadoDto?> Login(LoginAdministradorDto loginDto);
}