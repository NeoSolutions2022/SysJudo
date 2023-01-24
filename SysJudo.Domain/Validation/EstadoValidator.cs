using FluentValidation;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Validation;

public class EstadoValidator : AbstractValidator<Estado>
{
    public EstadoValidator()
    {
        RuleFor(e => e.Sigla)
            .NotEmpty()
            .WithMessage("A sigla não pode ser vazia")
            .NotNull()
            .WithMessage("A sigla não pode ser nula")
            .MaximumLength(2)
            .WithMessage("A sigla deve ter no máximo 2 caracteres");
        
        RuleFor(e => e.Descricao)
            .NotEmpty()
            .WithMessage("A descricao não pode ser vazia")
            .NotNull()
            .WithMessage("A descricao não pode ser nula")
            .MaximumLength(60)
            .WithMessage("A sigla deve ter no máximo 60 caracteres");
    }
}