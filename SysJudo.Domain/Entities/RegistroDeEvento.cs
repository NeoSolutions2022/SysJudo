using SysJudo.Domain.Contracts;

namespace SysJudo.Domain.Entities;

public class RegistroDeEvento : Entity, IAggregateRoot
{
    public DateTime? DataHoraEvento { get; set; }
    public string? ComputadorId { get; set; }
    public string? Descricao { get; set; }
    public int? ClienteId { get; set; }
    public int? TipoOperacaoId { get; set; }
    public int? UsuarioId { get; set; }
    public int? AdministradorId { get; set; }
    public int? FuncaoMenuId{ get; set; }

    public Cliente? Cliente { get; set; }
    public Administrador? Administrador { get; set; }
    public TipoOperacao? TipoOperacao { get; set; }
    public Usuario? Usuario { get; set; }
    public FuncaoMenu? FuncaoMenu { get; set; }
}