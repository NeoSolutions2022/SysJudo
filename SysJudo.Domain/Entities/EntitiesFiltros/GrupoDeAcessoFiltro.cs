using SysJudo.Domain.Contracts;

namespace SysJudo.Domain.Entities.EntitiesFiltros;

public class GrupoDeAcessoFiltro : EntityFiltro, IAggregateRoot, ITenant
{
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public bool Administrador { get; protected set; }
    public bool Desativado { get; set; }
    public int ClienteId { get; set; }
}