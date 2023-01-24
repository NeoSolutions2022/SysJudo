using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Usuario;

namespace SysJudo.Application.Contracts;

public interface IUsuarioService
{
    Task<UsuarioDto?> Adicionar(CreateUsuarioDto dto);
    Task<UsuarioDto?> Alterar(int id, UpdateUsuarioDto dto);
    Task<PagedDto<UsuarioDto>> Buscar(BuscarUsuarioDto dto);
    Task<UsuarioDto?> ObterPorEmail(string email);
    Task<UsuarioDto?> ObterPorId(int id);
    Task Remover(int id);
}