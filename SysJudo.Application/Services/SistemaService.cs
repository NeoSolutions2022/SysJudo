using AutoMapper;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Sistema;
using SysJudo.Application.Notifications;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Services;

public class SistemaService : BaseService, ISistemaService
{
    private readonly ISistemaRepository _sistemaRepository;

    public SistemaService(IMapper mapper, INotificator notificator, ISistemaRepository sistemaRepository) : base(mapper,
        notificator)
    {
        _sistemaRepository = sistemaRepository;
    }

    public async Task<SistemaDto?> Adicionar(CreateSistemaDto dto)
    {
        var sistema = Mapper.Map<Sistema>(dto);
        if (!await Validar(sistema))
        {
            return null;
        }

        _sistemaRepository.Adicionar(sistema);
        if (await _sistemaRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<SistemaDto>(sistema);
        }

        Notificator.Handle("Não foi possível cadastrar o sistema");
        return null;
    }

    public async Task<SistemaDto?> Alterar(int id, UpdateSistemaDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("Os ids não conferem");
            return null;
        }

        var sistema = await _sistemaRepository.ObterPorId(id);
        if (sistema == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        Mapper.Map(dto, sistema);
        if (!await Validar(sistema))
        {
            return null;
        }

        _sistemaRepository.Alterar(sistema);
        if (await _sistemaRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<SistemaDto>(sistema);
        }

        Notificator.Handle("Não possível alterar o sistema");
        return null;
    }

    public async Task<PagedDto<SistemaDto>> Buscar(BuscarSistemaDto dto)
    {
        var sistema = await _sistemaRepository.Buscar(dto);
        return Mapper.Map<PagedDto<SistemaDto>>(sistema);
    }

    public async Task<SistemaDto?> ObterPorId(int id)
    {
        var sistema = await _sistemaRepository.ObterPorId(id);
        if (sistema != null)
        {
            return Mapper.Map<SistemaDto>(sistema);
        }

        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task Remover(int id)
    {
        var sistema = await _sistemaRepository.ObterPorId(id);
        if (sistema == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        _sistemaRepository.Remover(sistema);
        if (!await _sistemaRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível remover o sistema");
        }
    }

    private async Task<bool> Validar(Sistema sistema)
    {
        if (!sistema.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }

        var existente = await _sistemaRepository.FirstOrDefault(s =>
            s.Sigla == sistema.Sigla || s.Descricao == sistema.Descricao && s.Id != sistema.Id);
        if (existente != null)
        {
            Notificator.Handle("Já existe um sistema cadastrado com essa sigla e/ou descrição");
        }

        return !Notificator.HasNotification;
    }
}