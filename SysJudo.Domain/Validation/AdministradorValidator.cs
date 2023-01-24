using FluentValidation;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Validation;

public class AdministradorValidator : AbstractValidator<Administrador>
{
    public AdministradorValidator()
    {
        RuleFor(s => s.Nome)
            .NotEmpty()
            .WithMessage("O nome não pode ser vazio")
            .MaximumLength(60)
            .WithMessage("O nome deve ter no máximo 60 caracteres")
            .NotNull()
            .WithMessage("O nome não pode ser nulo");

        RuleFor(s => s.Email)
            .NotEmpty()
            .WithMessage("O email não pode ser vazio")
            .MaximumLength(200)
            .WithMessage("O email deve ter no máximo 200 caracteres")
            .NotNull()
            .WithMessage("O email não pode ser nulo")
            .Matches(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")
            .WithMessage("O email deve ser válido");
        
        RuleFor(s => s.Senha)
            .NotEmpty()
            .WithMessage("A senha não pode ser vazia")
            .MinimumLength(8)
            .WithMessage("A senha deve ter no mínimo 8 caracteres")
            .NotNull()
            .WithMessage("A senha não pode ser nula");
    }
}