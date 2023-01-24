using FluentValidation;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Validation;

public class AgremiacaoValidation: AbstractValidator<Agremiacao>
{
    public AgremiacaoValidation()
    {
        RuleFor(c => c.Bairro)
            .NotEmpty()
            .WithMessage("Bairro não pode ser vazio!")
            .MaximumLength(30)
            .WithMessage("Bairro deve ter no máximo 30 caracteres");
        
        RuleFor(c => c.Cep)
            .NotEmpty()
            .WithMessage("Cep não pode ser vazio!")
            .MaximumLength(8)
            .WithMessage("Cep deve ter no máximo 8 caracteres");
        
        RuleFor(c => c.Cnpj)
            .NotEmpty()
            .WithMessage("Cnpj não pode ser vazio!")
            .MaximumLength(14)
            .WithMessage("Cnpj deve ter no máximo 14 caracteres");
        
        RuleFor(c => c.Complemento)
            .NotEmpty()
            .WithMessage("Complemento não pode ser vazio!")
            .MaximumLength(60)
            .WithMessage("Complemento deve ter no máximo 60 caracteres");
        
        
        RuleFor(c => c.DataFiliacao)
            .NotEmpty()
            .WithMessage("DataFiliacao não pode ser vazio!")
            .NotNull()
            .WithMessage("DataFiliacao não pode ser nulo!");
        
        RuleFor(c => c.DataNascimento)
            .NotEmpty()
            .WithMessage("DataNascimento não pode ser vazio!")
            .NotNull()
            .WithMessage("DataNascimento não pode ser nulo!");
        
        RuleFor(c => c.Email)
            .NotEmpty()
            .WithMessage("Email não pode ser vazio!")
            .MaximumLength(60)
            .WithMessage("Email deve ter no máximo 60 caracteres")
            .Matches(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")
            .WithMessage("O email deve ser válido");
        
        RuleFor(c => c.Endereco)
            .NotEmpty()
            .WithMessage("Endereco não pode ser vazio!")
            .MaximumLength(60)
            .WithMessage("Endereco deve ter no máximo 60 caracteres");
        
        RuleFor(c => c.Fantasia)
            .MaximumLength(60)
            .WithMessage("Fantasia deve ter no máximo 60 caracteres");

        RuleFor(c => c.IdCidade)
            .NotEmpty()
            .WithMessage("IdCidade não pode ser vazio!")
            .NotNull()
            .WithMessage("IdCidade não pode ser nulo!");
        
        RuleFor(c => c.IdEstado)
            .NotEmpty()
            .WithMessage("IdEstado não pode ser vazio!")
            .NotNull()
            .WithMessage("IdEstado não pode ser nulo!");
        
        RuleFor(c => c.IdPais)
            .NotEmpty()
            .WithMessage("IdPais não pode ser vazio!")
            .NotNull()
            .WithMessage("IdPais não pode ser nulo!");
        
        RuleFor(c => c.IdRegiao)
            .NotEmpty()
            .WithMessage("IdRegiao não pode ser vazio!")
            .NotNull()
            .WithMessage("IdRegiao não pode ser nulo!");
        
        RuleFor(c => c.InscricaoEstadual)
            .MaximumLength(60)
            .WithMessage("InscricaoEstadual deve ter no máximo 60 caracteres");
        
        RuleFor(c => c.InscricaoMunicipal)
            .MaximumLength(60)
            .WithMessage("InscricaoMunicipal deve ter no máximo 60 caracteres");

        RuleFor(c => c.Nome)
            .NotEmpty()
            .WithMessage("Nome não pode ser vazio!")
            .MaximumLength(60)
            .WithMessage("Nome deve ter no máximo 60 caracteres");
        
        RuleFor(c => c.Representante)
            .NotEmpty()
            .WithMessage("Representante não pode ser vazio!")
            .MaximumLength(60)
            .WithMessage("Representante deve ter no máximo 60 caracteres");
        
        RuleFor(c => c.Responsavel)
            .NotEmpty()
            .WithMessage("Responsavel não pode ser vazio!")
            .MaximumLength(60)
            .WithMessage("Responsavel deve ter no máximo 60 caracteres");

        RuleFor(c => c.Sigla)
            .NotEmpty()
            .WithMessage("Sigla não pode ser vazio!")
            .MaximumLength(10)
            .WithMessage("Sigla deve ter no máximo 10 caracteres");
        
        RuleFor(c => c.Telefone)
            .NotEmpty()
            .WithMessage("Telefone não pode ser vazio!")
            .MaximumLength(60)
            .WithMessage("Telefone deve ter no máximo 60 caracteres");
    }
}