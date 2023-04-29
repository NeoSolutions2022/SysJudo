using SysJudo.Domain.Contracts;

namespace SysJudo.Domain.Entities;

public class FuncaoMenu : Entity, ITenant
{
    public string Sigla { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public int ClienteId { get; set; }

    public Cliente Cliente { get; set; } = null!;
    public virtual List<RegistroDeEvento> RegistroDeEventos { get; set; } = new();
}