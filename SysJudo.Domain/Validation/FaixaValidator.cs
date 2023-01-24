using FluentValidation;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Validation;

public class FaixaValidator : AbstractValidator<Faixa>
{
    public FaixaValidator()
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
        
        RuleFor(s => s.MesesCarencia)
            .NotEmpty()
            .WithMessage("MesesCarencia não pode ser vazio")
            .GreaterThanOrEqualTo(0)
            .WithMessage("MesesCarencia deve ser maior ou igual a 0")
            .NotNull()
            .WithMessage("MesesCarencia não pode ser nulo");
        
        RuleFor(s => s.IdadeMinima)
            .NotEmpty()
            .WithMessage("A IdadeMinima não pode ser vazia")
            .GreaterThanOrEqualTo(0)
            .WithMessage("A IdadeMinima deve ser maior ou igual a 0")
            .NotNull()
            .WithMessage("A IdadeMinima não pode ser nula");
        
        RuleFor(s => s.OrdemExibicao)
            .NotEmpty()
            .WithMessage("A OrdemExibicao não pode ser vazia")
            .GreaterThanOrEqualTo(1)
            .WithMessage("A OrdemExibicao deve ser maior ou igual a 1")
            .NotNull()
            .WithMessage("A OrdemExibicao não pode ser nula");
    }
}