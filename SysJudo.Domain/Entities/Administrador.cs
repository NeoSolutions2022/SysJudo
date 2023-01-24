using FluentValidation.Results;
using SysJudo.Domain.Contracts;
using SysJudo.Domain.Validation;

namespace SysJudo.Domain.Entities;

public class Administrador : Entity, IAggregateRoot
{
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!;

    public override bool Validar(out ValidationResult validationResult)
    {
        validationResult = new AdministradorValidator().Validate(this);
        return validationResult.IsValid;
    }
}