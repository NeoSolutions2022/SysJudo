using AutoMapper;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Permissoes;
using SysJudo.Application.Notifications;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Services;

public class PermissaoService : BaseService, IPermissaoService
{
    private readonly IPermissaoRepository _permissaoRepository;

    public PermissaoService(IMapper mapper, INotificator notificator, IRegistroDeEventoRepository registroDeEventos, IPermissaoRepository permissaoRepository) : base(mapper, notificator, registroDeEventos)
    {
        _permissaoRepository = permissaoRepository;
    }
    
    public async Task<PagedDto<PermissaoDto>> Buscar(BuscarPermissaoDto dto)
    {
        var cursos = await _permissaoRepository.Buscar(dto);
        return Mapper.Map<PagedDto<PermissaoDto>>(cursos);
    }

    public async Task<PermissaoDto?> ObterPorId(int id)
    {
        var permissao = await ObterPermissao(id);
        return permissao != null ? Mapper.Map<PermissaoDto>(permissao) : null;
    }
    
    public async Task<PermissaoDto?> Adicionar(CadastrarPermissaoDto dto)
    {
        var permissao = Mapper.Map<Permissao>(dto);
        if (!await Validar(permissao))
        {
            return null;
        }
        
        _permissaoRepository.Cadastrar(permissao);

        return await CommitChanges() ? Mapper.Map<PermissaoDto>(permissao) : null;
    }
    
    public async Task<PermissaoDto?> Alterar(int id, AlterarPermissaoDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("IDs não conferem.");
            return null;
        }

        var permissao = await ObterPermissao(id);
        if (permissao == null)
        {
            return null;
        }
        
        Mapper.Map(dto, permissao);
        if (!await Validar(permissao))
        {
            return null;
        }

        _permissaoRepository.Alterar(permissao);
        return await CommitChanges() ? Mapper.Map<PermissaoDto>(permissao) : null;
    }

    public async Task Deletar(int id)
    {
        var permissao = await ObterPermissao(id);
        if (permissao == null)
        {
            return;
        }
        
        if (await _permissaoRepository.Any(c => c.Grupos.Any(i => i.PermissaoId == id)))
        {
            Notificator.Handle("Há grupos associados a esta permissão, não será possível deletá-la.");
            return;
        }
        
        _permissaoRepository.Deletar(permissao);
        await CommitChanges();
    }

    private async Task<Permissao?> ObterPermissao(int id)
    {
        var permissao = await _permissaoRepository.ObterPorId(id);
        if (permissao == null)
        {
            Notificator.HandleNotFoundResource();
        }
        
        return permissao;
    }
    
    private async Task<bool> Validar(Permissao permissao)
    {
        if (!permissao.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }

        var existente = await _permissaoRepository.FirstOrDefault(
            c => c.Nome == permissao.Nome && c.Categoria == permissao.Categoria && c.Id != permissao.Id);
        if (existente != null)
        {
            Notificator.Handle($"Já existe uma permissão com o mesmo nome e categoria.");
        }

        return !Notificator.HasNotification;
    }
    
    private async Task<bool> CommitChanges()
    {
        if (await _permissaoRepository.UnitOfWork.Commit())
        {
            return true;
        }
        
        Notificator.Handle("Não foi possível salvar as alterações.");
        return false;
    }
}