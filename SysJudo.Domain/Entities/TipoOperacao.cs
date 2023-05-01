namespace SysJudo.Domain.Entities;

public class TipoOperacao : Entity
{
    public string Sigla { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public virtual List<RegistroDeEvento> RegistroDeEventos { get; set; } = new();
}