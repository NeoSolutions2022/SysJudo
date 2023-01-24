using AutoMapper;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Faixa;
using SysJudo.Application.Notifications;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Services;

public class FaixaService : BaseService, IFaixaService
{
    private readonly IFaixaRepository _faixaRepository;
    public FaixaService(IMapper mapper, INotificator notificator, IFaixaRepository faixaRepository) : base(mapper, notificator)
    {
        _faixaRepository = faixaRepository;
    }
    
    public async Task<FaixaDto?> Adicionar(CreateFaixaDto dto)
    {
        var faixa = Mapper.Map<Faixa>(dto);
        if (!await Validar(faixa))
        {
            return null;
        }
        
        _faixaRepository.Adicionar(faixa);
        if (await _faixaRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<FaixaDto>(faixa);
        }
        
        Notificator.Handle("Não foi possível cadastrar a faixa");
        return null;
    }

    public async Task<FaixaDto?> Alterar(int id, UpdateFaixaDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("Os ids não conferem");
            return null;
        }

        var faixa = await _faixaRepository.ObterPorId(id);
        if (faixa == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        Mapper.Map(dto, faixa);
        if (!await Validar(faixa))
        {
            return null;
        }
        
        _faixaRepository.Alterar(faixa);
        if (await _faixaRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<FaixaDto>(faixa);
        }
        
        Notificator.Handle("Não possível alterar a faixa");
        return null;
    }

    public async Task<PagedDto<FaixaDto>> Buscar(BuscarFaixaDto dto)
    {
        var faixa = await _faixaRepository.Buscar(dto);
        return Mapper.Map<PagedDto<FaixaDto>>(faixa);
    }

    public async Task<FaixaDto?> ObterPorId(int id)
    {
        var faixa = await _faixaRepository.ObterPorId(id);
        if (faixa != null)
        {
            return Mapper.Map<FaixaDto>(faixa);
        }

        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task Remover(int id)
    {
        var faixa = await _faixaRepository.ObterPorId(id);
        if (faixa == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }
        _faixaRepository.Remover(faixa);
        if (!await _faixaRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível remover o sistema");
        }
    }
    
    private async Task<bool> Validar(Faixa faixa)
    {
        if (!faixa.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }

        var existente = await _faixaRepository.FirstOrDefault(c => c.Sigla == faixa.Sigla || c.Descricao == faixa.Descricao && c.Id != faixa.Id);
        if (existente != null)
        {
            Notificator.Handle("Já existe uma faixa cadastrada com essa sigla e/ou descrição");
        }

        return !Notificator.HasNotification;
    }
}