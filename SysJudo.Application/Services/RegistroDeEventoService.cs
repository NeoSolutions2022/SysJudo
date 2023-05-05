using AutoMapper;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.RegistroDeEvento;
using SysJudo.Application.Notifications;
using SysJudo.Domain.Contracts.Repositories;

namespace SysJudo.Application.Services;

public class RegistroDeEventoService : BaseService, IRegistroDeEventoService
{
    private readonly IRegistroDeEventoRepository _repository;
    public RegistroDeEventoService(IMapper mapper, INotificator notificator, IRegistroDeEventoRepository registroDeEventos, IRegistroDeEventoRepository repository) : base(mapper, notificator, registroDeEventos)
    {
        _repository = repository;
    }


    public async Task<List<RegistroDeEventoDto>> ObterTodos()
    {
        return Mapper.Map<List<RegistroDeEventoDto>>( await _repository.ObterTodos());
    }

    public async Task<RegistroDeEventoDto?> ObterPorId(int id)
    {
        var registroDeEvento = await _repository.ObterPorId(id);
        if (registroDeEvento != null)
        {
            return Mapper.Map<RegistroDeEventoDto>(registroDeEvento);
        }
        
        Notificator.HandleNotFoundResource();
        return null;
    }
}