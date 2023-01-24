using SysJudo.Application.Dto.Atleta;
using SysJudo.Application.Dto.Base;

namespace SysJudo.Application.Contracts;

public interface IAtletaService
{
    Task<AtletaDto?> Adicionar(CreateAtletaDto dto);
    Task<AtletaDto?> Alterar(int id, UpdateAtletaDto dto);
    Task<PagedDto<AtletaDto>> Buscar(BuscarAtletaDto dto);
    Task<AtletaDto?> ObterPorId(int id);
    Task Remover(int id);
}