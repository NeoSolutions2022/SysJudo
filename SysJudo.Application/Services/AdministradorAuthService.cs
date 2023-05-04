using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Administrador;
using SysJudo.Application.Dto.Auth;
using SysJudo.Core.Enums;
using SysJudo.Core.Extension;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using INotificator = SysJudo.Application.Notifications.INotificator;


namespace SysJudo.Application.Services;

public class AdministradorAuthService : BaseService, IAdministradorAuthService
{
    private readonly IPasswordHasher<Administrador> _passwordHasher;
    private readonly IAdministradorRepository _administradorRepository;

    public AdministradorAuthService(IMapper mapper, INotificator notificator,
        IPasswordHasher<Administrador> passwordHasher, IAdministradorRepository administradorRepository,
        IRegistroDeEventoRepository registroDeEventoRepository) : base(mapper, notificator, registroDeEventoRepository)
    {
        _passwordHasher = passwordHasher;
        _administradorRepository = administradorRepository;
    }

    public async Task<UsuarioAutenticadoDto?> Login(LoginAdministradorDto loginDto)
    {
        var administrador = await _administradorRepository.ObterPorEmail(loginDto.Email);
        if (administrador == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(administrador, administrador.Senha, loginDto.Senha);
        if (result == PasswordVerificationResult.Failed)
        {
            Notificator.Handle("Combinação de email e senha incorreta");
            return null;
        }

        return new UsuarioAutenticadoDto
        {
            Id = administrador.Id,
            Email = administrador.Email,
            Nome = administrador.Nome,
            Token = await CreateToken(administrador, loginDto.Ip)
        };
    }

    public Task<string> CreateToken(Administrador administrador, string ip)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Settings.Settings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, administrador.Id.ToString()),
                new Claim(ClaimTypes.Name, administrador.Nome),
                new Claim(ClaimTypes.Email, administrador.Email),
                new Claim("TipoUsuario", ETipoUsuario.Administrador.ToDescriptionString()),
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