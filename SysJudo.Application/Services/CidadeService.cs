using AutoMapper;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Cidade;
using SysJudo.Application.Notifications;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Services;

public class CidadeService : BaseService, ICidadeService
{
    private readonly ICidadeRepository _cidadeRepository;
    
    public CidadeService(IMapper mapper, INotificator notificator, ICidadeRepository cidadeRepository) : base(mapper, notificator)
    {
        _cidadeRepository = cidadeRepository;
    }

    public async Task<CidadeDto?> Adicionar(CreateCidadeDto dto)
    {
        var cidade = Mapper.Map<Cidade>(dto);
        if (!await Validar(cidade))
        {
            return null;
        }
        
        _cidadeRepository.Adicionar(cidade);
        if (await _cidadeRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<CidadeDto>(cidade);
        }
        
        Notificator.Handle("Não foi possível cadastrar a cidade");
        return null;
    }

    public async Task<CidadeDto?> Alterar(int id, UpdateCidadeDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("Os ids não conferem");
            return null;
        }

        var cidade = await _cidadeRepository.ObterPorId(id);
        if (cidade == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        Mapper.Map(dto, cidade);
        if (!await Validar(cidade))
        {
            return null;
        }
        
        _cidadeRepository.Alterar(cidade);
        if (await _cidadeRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<CidadeDto>(cidade);
        }
        
        Notificator.Handle("Não possível alterar a cidade");
        return null;
    }

    public async Task<PagedDto<CidadeDto>> Buscar(BuscarCidadeDto dto)
    {
        var cidade = await _cidadeRepository.Buscar(dto);
        return Mapper.Map<PagedDto<CidadeDto>>(cidade);
    }

    public async Task<CidadeDto?> ObterPorId(int id)
    {
        var cidade = await _cidadeRepository.ObterPorId(id);
        if (cidade != null)
        {
            return Mapper.Map<CidadeDto>(cidade);
        }

        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task Remover(int id)
    {
        var cidade = await _cidadeRepository.ObterPorId(id);
        if (cidade == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }
        
        _cidadeRepository.Remover(cidade);
        if (!await _cidadeRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível remover a cidade");
        }
    }
    
    private async Task<bool> Validar(Cidade cidade)
    {
        if (!cidade.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
            
        }

        var existente = await _cidadeRepository.FirstOrDefault(s => s.Sigla == cidade.Sigla || s.Descricao == cidade.Descricao && s.Id != cidade.Id);
        if (existente != null)
        {
            Notificator.Handle("Já existe uma cidade cadastrado com essa sigla e/ou descricao");
        }

        return !Notificator.HasNotification;
    }
}