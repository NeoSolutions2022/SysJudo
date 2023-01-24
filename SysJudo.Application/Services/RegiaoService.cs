using AutoMapper;
using Hangfire.Dashboard.Resources;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Regiao;
using SysJudo.Application.Notifications;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Services;

public class RegiaoService : BaseService ,IRegiaoService
{
    private readonly IRegiaoRepository _regiaoRepository;
    
    public RegiaoService(IMapper mapper, INotificator notificator, IRegiaoRepository regiaoRepository) : base(mapper, notificator)
    {
        _regiaoRepository = regiaoRepository;
    }

    public async Task<RegiaoDto?> Adicionar(CreateRegiaoDto dto)
    {
        var regiao = Mapper.Map<Regiao>(dto);
        if (!await Validar(regiao))
        {
            return null;
        }
        
        _regiaoRepository.Adicionar(regiao);
        if (await _regiaoRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<RegiaoDto>(regiao);
        }
        
        Notificator.Handle("Não foi possível cadastrar a região");
        return null;
    }

    public async Task<RegiaoDto?> Alterar(int id, UpdateRegiaoDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("Os ids não conferem");
            return null;
        }

        var regiao = await _regiaoRepository.ObterPorId(id);
        if (regiao == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        Mapper.Map(dto, regiao);
        if (!await Validar(regiao))
        {
            return null;
        }
        
        _regiaoRepository.Alterar(regiao);
        if (await _regiaoRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<RegiaoDto>(regiao);
        }
        
        Notificator.Handle("Não possível alterar a região");
        return null;
    }

    public async Task<PagedDto<RegiaoDto>> Buscar(BuscarRegiaoDto dto)
    {
        var regiao = await _regiaoRepository.Buscar(dto);
        return Mapper.Map<PagedDto<RegiaoDto>>(regiao);
    }

    public async Task<RegiaoDto?> ObterPorId(int id)
    {
        var regiao = await _regiaoRepository.ObterPorId(id);
        if (regiao != null)
        {
            return Mapper.Map<RegiaoDto>(regiao);
        }

        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task Remover(int id)
    {
        var regiao = await _regiaoRepository.ObterPorId(id);
        if (regiao == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }
        
        _regiaoRepository.Remover(regiao);
        if (!await _regiaoRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível remover a região");
        }
    }
    
    private async Task<bool> Validar(Regiao regiao)
    {
        if (!regiao.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
            
        }

        var existente = await _regiaoRepository.FirstOrDefault(s => s.Sigla == regiao.Sigla || s.Descricao == regiao.Descricao && s.Id != regiao.Id);
        if (existente != null)
        {
            Notificator.Handle("Já existe uma região cadastrado com essa sigla e/ou descrição");
        }

        return !Notificator.HasNotification;
    }
}