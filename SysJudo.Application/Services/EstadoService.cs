using AutoMapper;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Estado;
using SysJudo.Application.Notifications;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Services;

public class EstadoService : BaseService, IEstadoService
{
    private readonly IEstadoRepository _estadoRepository;
    
    public EstadoService(IMapper mapper, INotificator notificator, IEstadoRepository estadoRepository) : base(mapper, notificator)
    {
        _estadoRepository = estadoRepository;
    }

    public async Task<EstadoDto?> Adicionar(CreateEstadoDto dto)
    {
        var estado = Mapper.Map<Estado>(dto);
        if (!await Validar(estado))
        {
            return null;
        }
        
        _estadoRepository.Adicionar(estado);
        if (await _estadoRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<EstadoDto>(estado);
        }
        
        Notificator.Handle("Não foi possível cadastrar o estado");
        return null;
    }

    public async Task<EstadoDto?> Alterar(int id, UpdadeEstadoDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("Os ids não conferem");
            return null;
        }

        var estado = await _estadoRepository.ObterPorId(id);
        if (estado == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        Mapper.Map(dto, estado);
        if (!await Validar(estado))
        {
            return null;
        }
        
        _estadoRepository.Alterar(estado);
        if (await _estadoRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<EstadoDto>(estado);
        }
        
        Notificator.Handle("Não possível alterar o estado");
        return null;
    }

    public async Task<PagedDto<EstadoDto>> Buscar(BuscarEstadoDto dto)
    {
        var estado = await _estadoRepository.Buscar(dto);
        return Mapper.Map<PagedDto<EstadoDto>>(estado);
    }

    public async Task<EstadoDto?> ObterPorId(int id)
    {
        var estado = await _estadoRepository.ObterPorId(id);
        if (estado != null)
        {
            return Mapper.Map<EstadoDto>(estado);
        }

        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task Remover(int id)
    {
        var estado = await _estadoRepository.ObterPorId(id);
        if (estado == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }
        
        _estadoRepository.Remover(estado);
        if (!await _estadoRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível remover o estado");
        }
    }
    
    private async Task<bool> Validar(Estado estado)
    {
        if (!estado.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
            
        }

        var existente = await _estadoRepository.FirstOrDefault(s => s.Sigla == estado.Sigla || s.Descricao == estado.Descricao && s.Id != estado.Id);
        if (existente != null)
        {
            Notificator.Handle("Já existe um estado cadastrado com essa sigla e/ou descrição");
        }

        return !Notificator.HasNotification;
    }
}