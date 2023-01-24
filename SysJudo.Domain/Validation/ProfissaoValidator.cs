using FluentValidation;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Validation;

public class ProfissaoValidator : AbstractValidator<Profissao>
{
    public ProfissaoValidator()
    {
        RuleFor(c => c.Sigla)
            .NotEmpty()
            .WithMessage("Sigla não pode ser vazia")
            .MaximumLength(10)
            .WithMessage("Sigla deve ter no máximo 10 caracteres");

        RuleFor(c => c.Descricao)
            .NotEmpty()
            .WithMessage("Descricao não pode ser vazia")
            .MaximumLength(60)
            .WithMessage("Descricao deve ter no máximo 10 caracteres");
    }
}