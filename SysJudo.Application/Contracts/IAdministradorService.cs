using SysJudo.Application.Dto.Administrador;

namespace SysJudo.Application.Contracts;

public interface IAdministradorService
{
    Task<AdministradorDto?> Adicionar(CreateAdministradorDto dto);
    Task<AdministradorDto?> Alterar(int id, UpdateAdministradorDto dto);
    Task<AdministradorDto?> ObterPorId(int id);
    Task<AdministradorDto?> ObterPorEmail(string email);
    Task Remover(int id);
}