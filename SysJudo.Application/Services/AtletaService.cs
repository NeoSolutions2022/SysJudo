using AutoMapper;
using Microsoft.AspNetCore.Http;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Atleta;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Notifications;
using SysJudo.Core.Enums;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Services;

public class AtletaService : BaseService, IAtletaService
{
    private readonly IFileService _fileService;
    private readonly IAtletaRepository _atletaRepository;

    public AtletaService(IMapper mapper, INotificator notificator, IAtletaRepository atletaRepository,
        IFileService fileService) : base(mapper, notificator)
    {
        _atletaRepository = atletaRepository;
        _fileService = fileService;
    }

    public async Task<AtletaDto?> Adicionar(CreateAtletaDto dto)
    {
        if (!ValidarAnexo(dto))
        {
            return null;
        }
        
        var atleta = Mapper.Map<Atleta>(dto);
        if (!await Validar(atleta))
        {
            return null;
        }

        if (dto.Foto is { Length: > 0 })
        {
            atleta.Foto = await _fileService.Upload(dto.Foto, EUploadPath.FotosAgremiacao);
        }

        _atletaRepository.Adicionar(atleta);
        if (await _atletaRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<AtletaDto>(atleta);
        }

        Notificator.Handle("Não foi possível cadastrar o atleta");
        return null;
    }

    public async Task<AtletaDto?> Alterar(int id, UpdateAtletaDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("Os ids não conferem");
            return null;
        }

        var atleta = await _atletaRepository.ObterPorId(id);
        if (atleta == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        if (dto.Foto is { Length: > 0 } && !await ManterFoto(dto.Foto, atleta))
        {
            return null;
        }

        Mapper.Map(dto, atleta);
        if (!await Validar(atleta))
        {
            return null;
        }

        _atletaRepository.Alterar(atleta);
        if (await _atletaRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<AtletaDto>(atleta);
        }

        Notificator.Handle("Não possível alterar o atleta");
        return null;
    }

    public async Task<PagedDto<AtletaDto>> Buscar(BuscarAtletaDto dto)
    {
        var atleta = await _atletaRepository.Buscar(dto);
        return Mapper.Map<PagedDto<AtletaDto>>(atleta);
    }

    public async Task<AtletaDto?> ObterPorId(int id)
    {
        var atleta = await _atletaRepository.ObterPorId(id);
        if (atleta != null)
        {
            return Mapper.Map<AtletaDto>(atleta);
        }

        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task Remover(int id)
    {
        var atleta = await _atletaRepository.ObterPorId(id);
        if (atleta == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        _atletaRepository.Remover(atleta);
        if (!await _atletaRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível remover o atleta");
        }
    }

    private async Task<bool> Validar(Atleta atleta)
    {
        if (!atleta.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }

        var existente = await _atletaRepository.FirstOrDefault(e =>
            e.Cpf == atleta.Cpf && e.Id != atleta.Id);
        if (existente != null)
        {
            Notificator.Handle("Já existe um atleta cadastrado com esse cpf");
        }

        return !Notificator.HasNotification;
    }

    private async Task<bool> ManterFoto(IFormFile foto, Atleta atleta)
    {
        if (!string.IsNullOrWhiteSpace(atleta.Foto) && !_fileService.Apagar(new Uri(atleta.Foto)))
        {
            Notificator.Handle("Não foi possível remover a foto anterior.");
            return false;
        }

        atleta.Foto = await _fileService.Upload(foto, EUploadPath.FotosAtleta);
        return true;
    }

    private bool ValidarAnexo(CreateAtletaDto dto)
    {
        if (dto.Foto.Length > 10000000)
        {
            Notificator.Handle("Foto deve ter no maximo 10Mb");
        }

        if (!dto.Foto.FileName.Split(".").Last().Contains("pdf"))
        {
            Notificator.Handle("Foto deve ser no formato PDF");
        }

        return !Notificator.HasNotification;
    }
}