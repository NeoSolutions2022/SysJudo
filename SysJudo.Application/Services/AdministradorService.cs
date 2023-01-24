using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Administrador;
using SysJudo.Application.Notifications;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Services;

public class AdministradorService : BaseService, IAdministradorService
{
    private readonly IAdministradorRepository _administradorRepository;
    private readonly IPasswordHasher<Administrador> _passwordHasher;
    
    public AdministradorService(IMapper mapper, INotificator notificator, IAdministradorRepository administradorRepository, IPasswordHasher<Administrador> passwordHasher) : base(mapper, notificator)
    {
        _administradorRepository = administradorRepository;
        _passwordHasher = passwordHasher;
    }
    
    public async Task<AdministradorDto?> Adicionar(CreateAdministradorDto dto)
    {
        var administrador = Mapper.Map<Administrador>(dto);
        administrador.Senha = _passwordHasher.HashPassword(administrador, administrador.Senha);
        if (!await Validar(administrador))
        {
            return null;
        }
        
        _administradorRepository.Adicionar(administrador);
        if (await _administradorRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<AdministradorDto>(administrador);
        }
        
        Notificator.Handle("Não foi possível cadastrar o administrador");
        return null;
    }

    public async Task<AdministradorDto?> Alterar(int id, UpdateAdministradorDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("Os ids não conferem");
            return null;
        }

        var administrador = await _administradorRepository.ObterPorId(id);
        if (administrador == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        Mapper.Map(dto, administrador);
        administrador.Senha = _passwordHasher.HashPassword(administrador, administrador.Senha);
        if (!await Validar(administrador))
        {
            return null;
        }
        
        _administradorRepository.Alterar(administrador);
        if (await _administradorRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<AdministradorDto>(administrador);
        }
        
        Notificator.Handle("Não possível alterar o usuario");
        return null;
    }

    public async Task<AdministradorDto?> ObterPorId(int id)
    {
        var administrador = await _administradorRepository.ObterPorId(id);
        if (administrador != null)
        {
            return Mapper.Map<AdministradorDto>(administrador);
        }

        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task<AdministradorDto?> ObterPorEmail(string email)
    {
        var administrador = await _administradorRepository.ObterPorEmail(email);
        if (administrador != null)
        {
            return Mapper.Map<AdministradorDto>(administrador);
        }

        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task Remover(int id)
    {
        var administrador = await _administradorRepository.ObterPorId(id);
        if (administrador == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }
        
        _administradorRepository.Remover(administrador);
        if (!await _administradorRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível remover o administrador");
        }
    }
    
    private async Task<bool> Validar(Administrador administrador)
    {
        if (!administrador.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }

        var existente = await _administradorRepository.FirstOrDefault(s => s.Email == administrador.Email && s.Id != administrador.Id);
        if (existente != null)
        {
            Notificator.Handle("Já existe um administrador cadastrado com esse email");
        }

        return !Notificator.HasNotification;
    }
}