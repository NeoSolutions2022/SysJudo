using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Usuario;
using SysJudo.Application.Notifications;
using SysJudo.Core.Extension;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Services;

public class UsuarioService : BaseService, IUsuarioService
{
    private readonly HttpContextAccessor _httpContextAccessor;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public UsuarioService(IMapper mapper, INotificator notificator, IUsuarioRepository usuarioRepository,
        IPasswordHasher<Usuario> passwordHasher, IRegistroDeEventoRepository registroDeEventoRepository,
        IOptions<HttpContextAccessor> httpContextAccessor) : base(mapper,
        notificator, registroDeEventoRepository)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _httpContextAccessor = httpContextAccessor.Value;
    }

    public async Task<UsuarioDto?> Adicionar(CreateUsuarioDto dto)
    {
        var usuario = Mapper.Map<Usuario>(dto);
        usuario.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);
        if (!await Validar(usuario))
        {
            return null;
        }

        usuario.CriadoEm = DateTime.Now;
        usuario.DataExpiracao = DateTime.Now.AddMonths(1);
        _usuarioRepository.Adicionar(usuario);
        if (await _usuarioRepository.UnitOfWork.Commit())
        {
            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                DataHoraEvento = DateTime.Now,
                ComputadorId = null,
                Descricao = "Adicionar usuario",
                ClienteId = null,
                TipoOperacaoId = 4,
                UsuarioId = null,
                AdministradorId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                FuncaoMenuId = 91
            });

            await RegistroDeEventos.UnitOfWork.Commit();
            return Mapper.Map<UsuarioDto>(usuario);
        }

        Notificator.Handle("Não foi possível cadastrar o usuario");
        return null;
    }

    public async Task<UsuarioDto?> Alterar(int id, UpdateUsuarioDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("Os ids não conferem");
            return null;
        }

        var usuario = await _usuarioRepository.ObterPorId(id);
        if (usuario == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        Mapper.Map(dto, usuario);
        usuario.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);
        if (!await Validar(usuario))
        {
            return null;
        }

        _usuarioRepository.Alterar(usuario);
        if (await _usuarioRepository.UnitOfWork.Commit())
        {
            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                DataHoraEvento = DateTime.Now,
                ComputadorId = null,
                Descricao = "Alterar usuario",
                ClienteId = null,
                TipoOperacaoId = 5,
                UsuarioId = null,
                AdministradorId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                FuncaoMenuId = null
            });

            await RegistroDeEventos.UnitOfWork.Commit();
            return Mapper.Map<UsuarioDto>(usuario);
        }

        Notificator.Handle("Não possível alterar o usuario");
        return null;
    }

    public async Task<PagedDto<UsuarioDto>> Buscar(BuscarUsuarioDto dto)
    {
        var usuario = await _usuarioRepository.Buscar(dto);
        return Mapper.Map<PagedDto<UsuarioDto>>(usuario);
    }

    public async Task<UsuarioDto?> ObterPorEmail(string email)
    {
        var usuario = await _usuarioRepository.ObterPorEmail(email);
        if (usuario != null)
        {
            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                DataHoraEvento = DateTime.Now,
                ComputadorId = null,
                Descricao = "Visualizar usuario",
                ClienteId = null,
                TipoOperacaoId = 7,
                UsuarioId = null,
                AdministradorId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                FuncaoMenuId = null
            });

            await RegistroDeEventos.UnitOfWork.Commit();
            return Mapper.Map<UsuarioDto>(usuario);
        }

        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task<UsuarioDto?> ObterPorId(int id)
    {
        var usuario = await _usuarioRepository.ObterPorId(id);
        if (usuario != null)
        {
            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                DataHoraEvento = DateTime.Now,
                ComputadorId = null,
                Descricao = "Visualizar usuario",
                ClienteId = null,
                TipoOperacaoId = 7,
                UsuarioId = null,
                AdministradorId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                FuncaoMenuId = null
            });

            await RegistroDeEventos.UnitOfWork.Commit();
            return Mapper.Map<UsuarioDto>(usuario);
        }

        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task Remover(int id)
    {
        var usuario = await _usuarioRepository.ObterPorId(id);
        if (usuario == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        _usuarioRepository.Remover(usuario);
        if (!await _usuarioRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível remover o usuario");
        }
        
        RegistroDeEventos.Adicionar(new RegistroDeEvento
        {
            DataHoraEvento = DateTime.Now,
            ComputadorId = null,
            Descricao = "Remover usuario",
            ClienteId = null,
            TipoOperacaoId = 6,
            UsuarioId = null,
            AdministradorId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
            FuncaoMenuId = null
        });

        await RegistroDeEventos.UnitOfWork.Commit();
    }

    private async Task<bool> Validar(Usuario usuario)
    {
        if (!usuario.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }

        var existente = await _usuarioRepository.FirstOrDefault(s => s.Email == usuario.Email && s.Id != usuario.Id);
        if (existente != null)
        {
            Notificator.Handle("Já existe um usuario cadastrado com esse email");
        }

        return !Notificator.HasNotification;
    }
}