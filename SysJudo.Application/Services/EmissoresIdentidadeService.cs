using AutoMapper;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.EmissoresIdentidade;
using SysJudo.Application.Notifications;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Services;

public class EmissoresIdentidadeService : BaseService, IEmissoresIdentidadeService
{
    private readonly IEmissoresIdentidadeRepository _emissoresIdentidadeRepository;

    public EmissoresIdentidadeService(IMapper mapper, INotificator notificator,
        IEmissoresIdentidadeRepository emissoresIdentidadeRepository,
        IRegistroDeEventoRepository registroDeEventoRepository) : base(mapper, notificator, registroDeEventoRepository)
    {
        _emissoresIdentidadeRepository = emissoresIdentidadeRepository;
    }

    public async Task<EmissoresIdentidadeDto?> Adicionar(CreateEmissoresIdentidadeDto dto)
    {
        var emissoresIdentidade = Mapper.Map<EmissoresIdentidade>(dto);
        if (!await Validar(emissoresIdentidade))
        {
            return null;
        }

        _emissoresIdentidadeRepository.Adicionar(emissoresIdentidade);
        if (await _emissoresIdentidadeRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<EmissoresIdentidadeDto>(emissoresIdentidade);
        }

        Notificator.Handle("Não foi possível cadastrar o emissor");
        return null;
    }

    public async Task<EmissoresIdentidadeDto?> Alterar(int id, UpdateEmissoresIdentidadeDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("Os ids não conferem");
            return null;
        }

        var emissoresIdentidade = await _emissoresIdentidadeRepository.ObterPorId(id);
        if (emissoresIdentidade == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        Mapper.Map(dto, emissoresIdentidade);
        if (!await Validar(emissoresIdentidade))
        {
            return null;
        }

        _emissoresIdentidadeRepository.Alterar(emissoresIdentidade);
        if (await _emissoresIdentidadeRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<EmissoresIdentidadeDto>(emissoresIdentidade);
        }

        Notificator.Handle("Não possível alterar o emissor");
        return null;
    }

    public async Task<PagedDto<EmissoresIdentidadeDto>> Buscar(BuscarEmissoresIdentidadeDto dto)
    {
        var emissorIdentidade = await _emissoresIdentidadeRepository.Buscar(dto);
        return Mapper.Map<PagedDto<EmissoresIdentidadeDto>>(emissorIdentidade);
    }

    public async Task<EmissoresIdentidadeDto?> ObterPorId(int id)
    {
        var emissorIdentidade = await _emissoresIdentidadeRepository.ObterPorId(id);
        if (emissorIdentidade != null)
        {
            return Mapper.Map<EmissoresIdentidadeDto>(emissorIdentidade);
        }

        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task Remover(int id)
    {
        var emissorIdentidade = await _emissoresIdentidadeRepository.ObterPorId(id);
        if (emissorIdentidade == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        _emissoresIdentidadeRepository.Remover(emissorIdentidade);
        if (!await _emissoresIdentidadeRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível remover o emissor");
        }
    }

    private async Task<bool> Validar(EmissoresIdentidade emissoresIdentidade)
    {
        if (!emissoresIdentidade.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }

        var existente = await _emissoresIdentidadeRepository.FirstOrDefault(e =>
            e.Sigla == emissoresIdentidade.Sigla ||
            e.Descricao == emissoresIdentidade.Descricao && e.Id != emissoresIdentidade.Id);
        if (existente != null)
        {
            Notificator.Handle("Já existe um emissor cadastrado com essa sigla e/ou descrição");
        }

        return !Notificator.HasNotification;
    }
}