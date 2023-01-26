namespace SysJudo.Domain.Contracts;

public interface IEntity
{
    public int Id { get; set; }
}

public interface IEntityFiltro
{
    public int Identificador { get; set; }
    public int Id { get; set; }
}