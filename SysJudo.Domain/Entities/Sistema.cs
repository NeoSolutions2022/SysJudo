using FluentValidation.Results;
using SysJudo.Domain.Contracts;
using SysJudo.Domain.Validation;

namespace SysJudo.Domain.Entities;

public class Sistema : Entity, IAggregateRoot
{
    public string Sigla { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public string Versao { get; set; } = null!;

    public virtual List<Cliente> Clientes { get; set; } = new();
    
    public override bool Validar(out ValidationResult validationResult)
    {
        validationResult = new SistemaValidator().Validate(this);
        return validationResult.IsValid;
    }
}