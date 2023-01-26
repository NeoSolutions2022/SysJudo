using FluentValidation.Results;
using SysJudo.Domain.Contracts;

namespace SysJudo.Domain.Entities;

public abstract class Entity : BaseEntity
{
    public virtual bool Validar(out ValidationResult validationResult)
    {
        validationResult = new ValidationResult();
        return validationResult.IsValid;
    }
}

public abstract class BaseEntity : IEntity
{
    public int Id { get; set; }
}

public abstract class EntityFiltro : IEntityFiltro
{
    public int Identificador { get; set; }
    public int Id { get; set; }
}