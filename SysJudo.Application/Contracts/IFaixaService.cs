using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Faixa;

namespace SysJudo.Application.Contracts;

public interface IFaixaService
{
    Task<FaixaDto?> Adicionar(CreateFaixaDto dto);
    Task<FaixaDto?> Alterar(int id, UpdateFaixaDto dto);
    Task<PagedDto<FaixaDto>> Buscar(BuscarFaixaDto dto);
    Task<FaixaDto?> ObterPorId(int id);
    Task Remover(int id);
}