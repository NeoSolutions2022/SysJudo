using AutoMapper;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Profissao;
using SysJudo.Application.Notifications;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Services;

public class ProfissaoService : BaseService, IProfissaoService
{
    private readonly IProfissaoRepository _profissaoRepository;

    public ProfissaoService(IMapper mapper, INotificator notificator, IProfissaoRepository profissaoRepository) : base(
        mapper, notificator)
    {
        _profissaoRepository = profissaoRepository;
    }

    public async Task<ProfissaoDto?> Adicionar(CreateProfissaoDto dto)
    {
        var profissao = Mapper.Map<Profissao>(dto);
        if (!await Validar(profissao))
        {
            return null;
        }

        _profissaoRepository.Adicionar(profissao);
        if (await _profissaoRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<ProfissaoDto>(profissao);
        }

        Notificator.Handle("Não foi possível cadastrar a profissão");
        return null;
    }

    public async Task<ProfissaoDto?> ObterPorId(int id)
    {
        var profissao = await _profissaoRepository.ObterPorId(id);
        if (profissao != null)
        {
            return Mapper.Map<ProfissaoDto>(profissao);
        }

        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task<PagedDto<ProfissaoDto>> Buscar(BuscarProfissaoDto dto)
    {
        var profissao = await _profissaoRepository.Buscar(dto);
        return Mapper.Map<PagedDto<ProfissaoDto>>(profissao);
    }

    public async Task<ProfissaoDto?> Alterar(int id, UpdateProfissaoDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("Os ids não conferem");
            return null;
        }

        var profissao = await _profissaoRepository.ObterPorId(id);
        if (profissao == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        Mapper.Map(dto, profissao);
        if (!await Validar(profissao))
        {
            return null;
        }

        _profissaoRepository.Alterar(profissao);
        if (await _profissaoRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<ProfissaoDto>(profissao);
        }

        Notificator.Handle("Não possível alterar a profissão");
        return null;
    }

    public async Task Remover(int id)
    {
        var profissao = await _profissaoRepository.ObterPorId(id);
        if (profissao == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        _profissaoRepository.Remover(profissao);
        if (!await _profissaoRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível remover a profissão");
        }
    }

    private async Task<bool> Validar(Profissao profissao)
    {
        if (!profissao.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }

        var existente = await _profissaoRepository.FirstOrDefault(e =>
            e.Sigla == profissao.Sigla ||
            e.Descricao == profissao.Descricao && e.Id != profissao.Id);
        if (existente != null)
        {
            Notificator.Handle("Já existe uma profissão cadastrada com essa sigla e/ou descrição");
        }

        return !Notificator.HasNotification;
    }
}