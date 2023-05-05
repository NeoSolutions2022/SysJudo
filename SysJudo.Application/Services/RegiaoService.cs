using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Regiao;
using SysJudo.Application.Notifications;
using SysJudo.Core.Extension;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Services;

public class RegiaoService : BaseService, IRegiaoService
{
    private readonly IRegiaoRepository _regiaoRepository;
    private readonly HttpContextAccessor _httpContextAccessor;

    public RegiaoService(IMapper mapper, INotificator notificator, IRegiaoRepository regiaoRepository,
        IRegistroDeEventoRepository registroDeEventoRepository, IOptions<HttpContextAccessor> httpContextAccessor) :
        base(mapper, notificator, registroDeEventoRepository)
    {
        _regiaoRepository = regiaoRepository;
        _httpContextAccessor = httpContextAccessor.Value;
    }

    public async Task<RegiaoDto?> Adicionar(CreateRegiaoDto dto)
    {
        var regiao = Mapper.Map<Regiao>(dto);
        if (!await Validar(regiao))
        {
            return null;
        }

        _regiaoRepository.Adicionar(regiao);
        if (await _regiaoRepository.UnitOfWork.Commit())
        {
            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                DataHoraEvento = DateTime.Now,
                ComputadorId = ObterIp(),
                Descricao = "Adicionar regiao",
                ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
                TipoOperacaoId = 4,
                UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                AdministradorId = null,
                FuncaoMenuId = 8
            });

            await RegistroDeEventos.UnitOfWork.Commit();
            return Mapper.Map<RegiaoDto>(regiao);
        }

        Notificator.Handle("Não foi possível cadastrar a região");
        return null;
    }

    public async Task<RegiaoDto?> Alterar(int id, UpdateRegiaoDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("Os ids não conferem");
            return null;
        }

        var regiao = await _regiaoRepository.ObterPorId(id);
        if (regiao == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        Mapper.Map(dto, regiao);
        if (!await Validar(regiao))
        {
            return null;
        }

        _regiaoRepository.Alterar(regiao);
        if (await _regiaoRepository.UnitOfWork.Commit())
        {
            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                DataHoraEvento = DateTime.Now,
                ComputadorId = ObterIp(),
                Descricao = "Alterar regiao",
                ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
                TipoOperacaoId = 5,
                UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                AdministradorId = null,
                FuncaoMenuId = 8
            });

            await RegistroDeEventos.UnitOfWork.Commit();
            return Mapper.Map<RegiaoDto>(regiao);
        }

        Notificator.Handle("Não possível alterar a região");
        return null;
    }

    public async Task<PagedDto<RegiaoDto>> Buscar(BuscarRegiaoDto dto)
    {
        var regiao = await _regiaoRepository.Buscar(dto);
        return Mapper.Map<PagedDto<RegiaoDto>>(regiao);
    }

    public async Task<RegiaoDto?> ObterPorId(int id)
    {
        var regiao = await _regiaoRepository.ObterPorId(id);
        if (regiao != null)
        {
            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                DataHoraEvento = DateTime.Now,
                ComputadorId = ObterIp(),
                Descricao = "Visualizar regiao",
                ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
                TipoOperacaoId = 7,
                UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                AdministradorId = null,
                FuncaoMenuId = 8
            });

            await RegistroDeEventos.UnitOfWork.Commit();
            return Mapper.Map<RegiaoDto>(regiao);
        }

        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task Remover(int id)
    {
        var regiao = await _regiaoRepository.ObterPorId(id);
        if (regiao == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        _regiaoRepository.Remover(regiao);
        if (!await _regiaoRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível remover a região");
        }

        RegistroDeEventos.Adicionar(new RegistroDeEvento
        {
            DataHoraEvento = DateTime.Now,
            ComputadorId = ObterIp(),
            Descricao = "Remover regiao",
            ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
            TipoOperacaoId = 6,
            UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
            AdministradorId = null,
            FuncaoMenuId = 8
        });

        await RegistroDeEventos.UnitOfWork.Commit();
    }

    private async Task<bool> Validar(Regiao regiao)
    {
        if (!regiao.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }

        var existente = await _regiaoRepository.FirstOrDefault(s => s.Id != regiao.Id &&
                                                                    (s.Sigla == regiao.Sigla ||
                                                                     s.Descricao == regiao.Descricao));
        if (existente != null)
        {
            Notificator.Handle("Já existe uma região cadastrado com essa sigla e/ou descrição");
        }

        return !Notificator.HasNotification;
    }
}