using FluentValidation;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Validation;

public class SistemaValidator : AbstractValidator<Sistema>
{
    public SistemaValidator()
    {
        RuleFor(s => s.Sigla)
            .NotEmpty()
            .WithMessage("A sigla não pode ser vazia")
            .NotNull()
            .WithMessage("A sigla não pode ser nula");

        RuleFor(s => s.Descricao)
            .NotEmpty()
            .WithMessage("A descrição não pode ser vazia")
            .NotNull()
            .WithMessage("A descrição não pode ser nula");

        RuleFor(s => s.Versao)
            .NotEmpty()
            .WithMessage("A versao não pode ser vazia")
            .NotNull()
            .WithMessage("A versao não pode ser nula");
    }
}