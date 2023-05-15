using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Auth;
using SysJudo.Application.Dto.Usuario;
using SysJudo.Application.Notifications;
using SysJudo.Core.Authorization;
using SysJudo.Core.Enums;
using SysJudo.Core.Extension;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Services;

public class AuthService : BaseService, IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly IGrupoAcessoRepository _grupoAcessoRepository;

    public AuthService(IMapper mapper, INotificator notificator, IUsuarioRepository usuarioRepository,
        IPasswordHasher<Usuario> passwordHasher, IRegistroDeEventoRepository registroDeEventoRepository, IGrupoAcessoRepository grupoAcessoRepository) : base(mapper,
        notificator, registroDeEventoRepository)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _grupoAcessoRepository = grupoAcessoRepository;
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
            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                DataHoraEvento = DateTime.Now,
                ComputadorId = null,
                Descricao = "Login",
                ClienteId = null,
                TipoOperacaoId = 1,
                UsuarioNome = null,
                AdministradorNome = null,
                UsuarioId = null,
                AdministradorId = null,
                FuncaoMenuId = null
            });
            await RegistroDeEventos.UnitOfWork.Commit();
            return new UsuarioAutenticadoDto
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Nome = usuario.Nome,
                Token = await CreateToken(usuario)
            };
        }
        
        Notificator.Handle("Combinação de email e senha incorreta");
        return null;
    }

    public async Task<string> CreateToken(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Settings.Settings.Secret);
        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, usuario.Email));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, usuario.Nome));
        claimsIdentity.AddClaim(new Claim("ClienteId", usuario.ClienteId.ToString()));
        claimsIdentity.AddClaim(new Claim("TipoUsuario", ETipoUsuario.Comum.ToDescriptionString()));
        claimsIdentity.AddClaim(new Claim("GrupoAcesso", "GrupoAcesso"));
        
        await AdicionarPermissoes(usuario, claimsIdentity);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    private async Task AdicionarPermissoes(Usuario usuario, ClaimsIdentity claimsIdentity)
    {
        if (!usuario.GrupoAcessos.Any())
        {
            return;
        }

        var gruposIds = usuario
            .GrupoAcessos
            .Select(g => g.GrupoAcessoId)
            .ToList();

        var grupos = await _grupoAcessoRepository.ObterTodos();
        var gruposFiltrados = grupos.Where(c => gruposIds.Contains(c.Id));

        var permissoes = MapPermissoes(gruposFiltrados.SelectMany(c => c.Permissoes)).Select(p => p.ToString());
        claimsIdentity.AddClaim(new Claim("permissoes", JsonConvert.SerializeObject(permissoes), JsonClaimValueTypes.JsonArray)); 
    }

    private static IEnumerable<PermissaoClaim> MapPermissoes(IEnumerable<GrupoAcessoPermissao> permissoes)
    {
        return permissoes
            .GroupBy(c => c.PermissaoId)
            .Select(grupo =>
            {
                var tipos = grupo
                    .SelectMany(gap => gap.Tipo.ToCharArray().Select(c => c.ToString()))
                    .Distinct();
                
                return new PermissaoClaim
                {
                    Nome = grupo.First().Permissao.Nome,
                    Tipo = string.Join("", tipos)
                };
            })
            .ToList();
    }
}