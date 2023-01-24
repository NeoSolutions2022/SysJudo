using SysJudo.Domain.Contracts;

namespace SysJudo.Domain.Entities;

public class EstadoCivil : Entity, ITenant
{
    public string Sigla { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public int ClienteId { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;
    public virtual List<Atleta> Atletas { get; set; } = new();
}