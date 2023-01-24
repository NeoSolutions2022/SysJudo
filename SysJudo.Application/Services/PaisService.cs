using AutoMapper;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Pais;
using SysJudo.Application.Notifications;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Services;

public class PaisService : BaseService, IPaisService
{
    private readonly IPaisRepository _paisRepository;
    
    public PaisService(IMapper mapper, INotificator notificator, IPaisRepository paisRepository) : base(mapper, notificator)
    {
        _paisRepository = paisRepository;
    }

    public async Task<PaisDto?> Adicionar(CreatePaisDto dto)
    {
        var pais = Mapper.Map<Pais>(dto);
        if (!await Validar(pais))
        {
            return null;
        }
        
        _paisRepository.Adicionar(pais);
        if (await _paisRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<PaisDto>(pais);
        }
        
        Notificator.Handle("Não foi possível cadastrar o pais");
        return null;
    }

    public async Task<PaisDto?> Alterar(int id, UpdatePaisDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("Os ids não conferem");
            return null;
        }

        var pais = await _paisRepository.ObterPorId(id);
        if (pais == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        Mapper.Map(dto, pais);
        if (!await Validar(pais))
        {
            return null;
        }
        
        _paisRepository.Alterar(pais);
        if (await _paisRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<PaisDto>(pais);
        }
        
        Notificator.Handle("Não possível alterar o pais");
        return null;
    }

    public async Task<PagedDto<PaisDto>> Buscar(BuscarPaisDto dto)
    {
        var pais = await _paisRepository.Buscar(dto);
        return Mapper.Map<PagedDto<PaisDto>>(pais);
    }

    public async Task<PaisDto?> ObterPorId(int id)
    {
        var pais = await _paisRepository.ObterPorId(id);
        if (pais != null)
        {
            return Mapper.Map<PaisDto>(pais);
        }

        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task Remover(int id)
    {
        var pais = await _paisRepository.ObterPorId(id);
        if (pais == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }
        
        _paisRepository.Remover(pais);
        if (!await _paisRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível remover o pais");
        }
    }
    
    private async Task<bool> Validar(Pais pais)
    {
        if (!pais.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
            
        }

        var existente = await _paisRepository.FirstOrDefault(s => s.Sigla2 == pais.Sigla2 || s.Descricao == pais.Descricao && s.Id != pais.Id);
        if (existente != null)
        {
            Notificator.Handle("Já existe um pais cadastrado com essa sigla e/ou descrição");
        }

        return !Notificator.HasNotification;
    }
}