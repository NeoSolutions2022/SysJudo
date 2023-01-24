using FluentValidation;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Validation;

public class NacionalidadeValidator : AbstractValidator<Nacionalidade>
{
    public NacionalidadeValidator()
    {
        RuleFor(c => c.Sigla)
            .NotEmpty()
            .WithMessage("Sigla não pode ser vazia")
            .MaximumLength(10)
            .WithMessage("Sigla deve ter no máximo 10 caracteres");

        RuleFor(c => c.Descricao)
            .NotEmpty()
            .WithMessage("Descricao não pode ser vazia")
            .MaximumLength(80)
            .WithMessage("Descricao deve ter no máximo 10 caracteres");
    }
}