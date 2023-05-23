using System.Net;
using AutoMapper;
using SysJudo.Application.Notifications;
using SysJudo.Domain.Contracts.Repositories;

namespace SysJudo.Application.Services;

public abstract class BaseService
{
    protected readonly IMapper Mapper;
    protected readonly INotificator Notificator;
    protected readonly IRegistroDeEventoRepository RegistroDeEventos;

    protected BaseService(IMapper mapper, INotificator notificator, IRegistroDeEventoRepository registroDeEventos)
    {
        Mapper = mapper;
        Notificator = notificator;
        RegistroDeEventos = registroDeEventos;
    }
    
    protected static string ObterIp()
    {
        var nomeMaquina = Dns.GetHostName();
        var ipLocal = Dns.GetHostAddresses(nomeMaquina);
        return ipLocal[1].ToString();
    }
}