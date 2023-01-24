using FluentValidation;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Validation;

public class PaisValidator : AbstractValidator<Pais>
{
    public PaisValidator()
    {
        RuleFor(p => p.Descricao)
            .NotEmpty()
            .WithMessage("Descricao não pode ser vazio")
            .NotNull()
            .WithMessage("Descricao não pode ser nulo")
            .MaximumLength(60)
            .WithMessage("Descricao deve ter no máximo 60 caracteres");
        
        RuleFor(p => p.Sigla2)
            .NotEmpty()
            .WithMessage("Sigla2 não pode ser vazio")
            .NotNull()
            .WithMessage("Sigla2 não pode ser nulo")
            .MaximumLength(3)
            .WithMessage("Sigla2 deve ter no máximo 3 caracteres");
        
        RuleFor(p => p.Sigla3)
            .NotEmpty()
            .WithMessage("Sigla3 não pode ser vazio")
            .NotNull()
            .WithMessage("Sigla3 não pode ser nulo")
            .MaximumLength(3)
            .WithMessage("Sigla3 deve ter no máximo 3 caracteres");
        
        RuleFor(p => p.Nacionalidade)
            .MaximumLength(30)
            .WithMessage("A nacionalidade deve ter no máximo 30 caracteres");
    }
}