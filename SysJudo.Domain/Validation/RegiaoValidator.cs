using FluentValidation;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Validation;

public class RegiaoValidator : AbstractValidator<Regiao>
{
    public RegiaoValidator()
    {
        RuleFor(s => s.Sigla)
            .NotEmpty()
            .WithMessage("Sigla não pode ser vazio")
            .MaximumLength(10)
            .WithMessage("Sigla deve ter no máximo 60 caracteres")
            .NotNull()
            .WithMessage("Sigla não pode ser nulo");
        
        RuleFor(s => s.Descricao)
            .NotEmpty()
            .WithMessage("Descricao não pode ser vazio")
            .MaximumLength(60)
            .WithMessage("Descricao deve ter no máximo 60 caracteres")
            .NotNull()
            .WithMessage("Descricao não pode ser nulo");
        
        RuleFor(s => s.Responsavel)
            .MaximumLength(60)
            .WithMessage("Responsavel deve ter no máximo 60 caracteres");
        
        RuleFor(s => s.Cep)
            .NotEmpty()
            .WithMessage("Cep não pode ser vazio")
            .MaximumLength(8)
            .WithMessage("Cep deve ter no máximo 8 caracteres")
            .NotNull()
            .WithMessage("Cep não pode ser nulo");
        
        RuleFor(s => s.Endereco)
            .NotEmpty()
            .WithMessage("Endereco não pode ser vazio")
            .MaximumLength(60)
            .WithMessage("Endereco deve ter no máximo 60 caracteres")
            .NotNull()
            .WithMessage("Endereco não pode ser nulo");
        
        RuleFor(s => s.Bairro)
            .MaximumLength(30)
            .WithMessage("Bairro deve ter no máximo 30 caracteres");
        
        RuleFor(s => s.Complemento)
            .MaximumLength(60)
            .WithMessage("Complemento deve ter no máximo 60 caracteres");
        
        RuleFor(s => s.IdCidade)
            .NotEmpty()
            .WithMessage("IdCidade não pode ser vazio")
            .NotNull()
            .WithMessage("IdCidade não pode ser nulo");
        
        RuleFor(s => s.IdEstado)
            .NotEmpty()
            .WithMessage("IdEstado não pode ser vazio")
            .NotNull()
            .WithMessage("IdEstado não pode ser nulo");
        
        RuleFor(s => s.IdPais)
            .NotEmpty()
            .WithMessage("IdPais não pode ser vazio")
            .NotNull()
            .WithMessage("IdPais não pode ser nulo");
        
        RuleFor(s => s.Telefone)
            .MaximumLength(60)
            .WithMessage("Telefone deve ter no máximo 60 caracteres");
        
        RuleFor(s => s.Email)
            .Matches(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")
            .WithMessage("O email deve ser válido");
    }
}