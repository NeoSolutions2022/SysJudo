using FluentValidation;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Validation;

public class ClienteValidator : AbstractValidator<Cliente>
{
    public ClienteValidator()
    {
        RuleFor(s => s.Sigla)
            .NotEmpty()
            .WithMessage("A sigla não pode ser vazia")
            .NotNull()
            .WithMessage("A sigla não pode ser nula");
        
        RuleFor(s => s.Nome)
            .NotEmpty()
            .WithMessage("O nome não pode ser vazio")
            .NotNull()
            .WithMessage("O nome não pode ser nulo");
        
        RuleFor(s => s.PastaArquivo)
            .NotEmpty()
            .WithMessage("PastaArquivo não pode ser vazio")
            .NotNull()
            .WithMessage("PastaArquivo não pode ser nulo");
        
        RuleFor(s => s.IdSistema)
            .NotEmpty()
            .WithMessage("O idSistema não pode ser vazio")
            .NotNull()
            .WithMessage("O idSistema não pode ser nulo");
    }
}