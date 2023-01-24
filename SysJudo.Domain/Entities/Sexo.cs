namespace SysJudo.Domain.Entities;

public class Sexo
{
    public int Id { get; set; }
    public char Sigla { get; set; }
    public string Descricao { get; set; } = null!;
    
    public virtual List<Atleta> Atletas { get; set; } = new();
}