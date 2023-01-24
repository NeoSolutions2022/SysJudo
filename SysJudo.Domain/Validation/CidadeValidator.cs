using FluentValidation;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Validation;

public class CidadeValidator : AbstractValidator<Cidade>
{
    public CidadeValidator()
    {
        RuleFor(c => c.Sigla)
            .NotEmpty()
            .WithMessage("A sigla não pode ser vazia")
            .NotNull()
            .WithMessage("A sigla não pode ser nula")
            .MaximumLength(10)
            .WithMessage("A sigla deve ter no máximo 10 caracteres");
        
        RuleFor(c => c.Descricao)
            .NotEmpty()
            .WithMessage("A descricao não pode ser vazia")
            .NotNull()
            .WithMessage("A descricao não pode ser nula")
            .MaximumLength(60)
            .WithMessage("A descricao deve ter no máximo 60 caracteres");
    }
}