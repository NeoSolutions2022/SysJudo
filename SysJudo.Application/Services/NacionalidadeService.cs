using AutoMapper;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Nacionalidade;
using SysJudo.Application.Notifications;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using SysJudo.Domain.Validation;

namespace SysJudo.Application.Services;

public class NacionalidadeService : BaseService, INacionalidadeService
{
    private readonly INacionalidadeRepositoty _nacionalidadeRepositoty;
    public NacionalidadeService(IMapper mapper, INotificator notificator, INacionalidadeRepositoty nacionalidadeRepositoty) : base(mapper, notificator)
    {
        _nacionalidadeRepositoty = nacionalidadeRepositoty;
    }

    public async Task<NacionalidadeDto?> Adicionar(CreateNacionalidadeDto dto)
    {
        var nacionalidade = Mapper.Map<Nacionalidade>(dto);
        if (!await Validar(nacionalidade))
        {
            return null;
        }

        _nacionalidadeRepositoty.Adicionar(nacionalidade);
        if (await _nacionalidadeRepositoty.UnitOfWork.Commit())
        {
            return Mapper.Map<NacionalidadeDto>(nacionalidade);
        }

        Notificator.Handle("Não foi possível cadastrar a nacionalidade");
        return null;
    }

    public async Task<NacionalidadeDto?> Alterar(int id, UpdateNacionalidadeDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("Os ids não conferem");
            return null;
        }

        var nacionalidade = await _nacionalidadeRepositoty.ObterPorId(id);
        if (nacionalidade == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        Mapper.Map(dto, nacionalidade);
        if (!await Validar(nacionalidade))
        {
            return null;
        }

        _nacionalidadeRepositoty.Alterar(nacionalidade);
        if (await _nacionalidadeRepositoty.UnitOfWork.Commit())
        {
            return Mapper.Map<NacionalidadeDto>(nacionalidade);
        }

        Notificator.Handle("Não possível alterar a nacionalidade");
        return null;
    }

    public async Task<PagedDto<NacionalidadeDto>> Buscar(BuscarNacionalidadeDto dto)
    {
        var nacionalidade = await _nacionalidadeRepositoty.Buscar(dto);
        return Mapper.Map<PagedDto<NacionalidadeDto>>(nacionalidade);
    }

    public async Task<NacionalidadeDto?> ObterPorId(int id)
    {
        var nacionalidade = await _nacionalidadeRepositoty.ObterPorId(id);
        if (nacionalidade != null)
        {
            return Mapper.Map<NacionalidadeDto>(nacionalidade);
        }

        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task Remover(int id)
    {
        var nacionalidade = await _nacionalidadeRepositoty.ObterPorId(id);
        if (nacionalidade == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        _nacionalidadeRepositoty.Remover(nacionalidade);
        if (!await _nacionalidadeRepositoty.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível remover a nacionalidade");
        }
    }
    
    private async Task<bool> Validar(Nacionalidade nacionalidade)
    {
        if (!nacionalidade.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }

        var existente = await _nacionalidadeRepositoty.FirstOrDefault(e =>
            e.Sigla == nacionalidade.Sigla ||
            e.Descricao == nacionalidade.Descricao && e.Id != nacionalidade.Id);
        if (existente != null)
        {
            Notificator.Handle("Já existe uma nacionalidade cadastrada com essa sigla e/ou descrição");
        }

        return !Notificator.HasNotification;
    }
}