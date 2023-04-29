using SysJudo.Domain.Contracts;

namespace SysJudo.Domain.Entities;

public class RegistroDeEvento : Entity, ITenant, IAggregateRoot
{
    public DateTime DataHoraEvento { get; set; }
    public int? ComputadorId { get; set; }
    public string Descricao { get; set; } = null!;
    public int ClienteId { get; set; }
    public int TipoOperacaoId { get; set; }
    public int UsuarioId { get; set; }
    public int FuncaoMenuId{ get; set; }

    public Cliente Cliente { get; set; } = null!;
    public TipoOperacao TipoOperacao { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
    public FuncaoMenu FuncaoMenu { get; set; } = null!;
}