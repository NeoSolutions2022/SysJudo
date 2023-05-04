using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Auth;
using SysJudo.Application.Notifications;
using SysJudo.Core.Enums;
using SysJudo.Core.Extension;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Services;

public class AuthService : BaseService, IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public AuthService(IMapper mapper, INotificator notificator, IUsuarioRepository usuarioRepository,
        IPasswordHasher<Usuario> passwordHasher, IRegistroDeEventoRepository registroDeEventoRepository) : base(mapper,
        notificator, registroDeEventoRepository)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UsuarioAutenticadoDto?> Login(LoginDto loginDto)
    {
        var usuario = await _usuarioRepository.ObterPorEmail(loginDto.Email);
        if (usuario == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }
        
        var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, loginDto.Senha);
        if (result != PasswordVerificationResult.Failed)
        {
            usuario.UltimoLogin = DateTime.Now;
            await _usuarioRepository.UnitOfWork.Commit();
            return new UsuarioAutenticadoDto
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Nome = usuario.Nome,
                Token = await CreateToken(usuario, loginDto.Ip)
            };
        }
        
        Notificator.Handle("Combinação de email e senha incorreta");
        return null;
    }

    public Task<string> CreateToken(Usuario usuario, string ip)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Settings.Settings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim("ClienteId", usuario.ClienteId.ToString()),
                new Claim("TipoUsuario", ETipoUsuario.Comum.ToDescriptionString()),
                new Claim("IpMaquina", ip)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Task.FromResult(tokenHandler.WriteToken(token));
    }
}