using FluentValidation;
using SysJudo.Domain.Entities;

namespace SysJudo.Domain.Validation;

public class AtletaValidator : AbstractValidator<Atleta>
{
    public AtletaValidator()
    {
        RuleFor(c => c.RegistroFederacao)
            .NotEmpty()
            .WithMessage("RegistroFederacao não pode ser vazio")
            .MaximumLength(10)
            .WithMessage("RegistroFederacao deve ter no máximo 10 caracteres");
        
        RuleFor(c => c.RegistroConfederacao)
            .MaximumLength(10)
            .WithMessage("RegistroConfederacao deve ter no máximo 10 caracteres");
        
        RuleFor(c => c.Nome)
            .NotEmpty()
            .WithMessage("Nome não pode ser vazio")
            .MaximumLength(60)
            .WithMessage("Nome deve ter no máximo 60 caracteres");
        
        RuleFor(c => c.DataNascimento)
            .NotNull()
            .WithMessage("DataNascimento não pode ser nulo")
            .NotEmpty()
            .WithMessage("DataNascimento não pode ser vazio");
        
        RuleFor(c => c.DataFiliacao)
            .NotNull()
            .WithMessage("DataFiliacao não pode ser nulo")
            .NotEmpty()
            .WithMessage("DataFiliacao não pode ser vazio");
        
        RuleFor(c => c.IdAgremiacao)
            .NotNull()
            .WithMessage("IdAgremiacao não pode ser nulo")
            .NotEmpty()
            .WithMessage("IdAgremiacao não pode ser vazio");
        
        RuleFor(c => c.Cep)
            .NotEmpty()
            .WithMessage("Cep não pode ser vazio")
            .MaximumLength(8)
            .WithMessage("Cep deve ter no máximo 8 caracteres");
        
        RuleFor(c => c.Endereco)
            .NotEmpty()
            .WithMessage("Endereco não pode ser vazio")
            .MaximumLength(60)
            .WithMessage("Endereco deve ter no máximo 60 caracteres");
        
        RuleFor(c => c.Bairro)
            .MaximumLength(30)
            .WithMessage("Bairro deve ter no máximo 30 caracteres");
        
        RuleFor(c => c.Complemento)
            .MaximumLength(60)
            .WithMessage("Bairro deve ter no máximo 60 caracteres");
        
        RuleFor(c => c.Cidade)
            .NotNull()
            .WithMessage("Cidade não pode ser nulo")
            .MaximumLength(30)
            .WithMessage("Cidade deve ter no máximo 30 caracteres")
            .NotEmpty()
            .WithMessage("Cidade não pode ser vazio");
        
        RuleFor(c => c.Estado)
            .NotNull()
            .WithMessage("Estado não pode ser nulo")
            .MaximumLength(30)
            .WithMessage("Estado deve ter no máximo 30 caracteres")
            .NotEmpty()
            .WithMessage("Estado não pode ser vazio");
        
        RuleFor(c => c.Pais)
            .NotNull()
            .WithMessage("Pais não pode ser nulo")
            .MaximumLength(30)
            .WithMessage("Pais deve ter no máximo 30 caracteres")
            .NotEmpty()
            .WithMessage("Pais não pode ser vazio");
        
        RuleFor(c => c.Telefone)
            .NotEmpty()
            .WithMessage("Telefone não pode ser vazio")
            .MaximumLength(60)
            .WithMessage("Telefone deve ter no máximo 10 caracteres");
        
        RuleFor(c => c.Email)
            .EmailAddress()
            .NotEmpty()
            .WithMessage("Email não pode ser vazio")
            .MaximumLength(60)
            .WithMessage("Email deve ter no máximo 10 caracteres");
        
        RuleFor(c => c.Cpf)
            .NotEmpty()
            .WithMessage("Cpf não pode ser vazio")
            .MaximumLength(11)
            .WithMessage("Cpf deve ter no máximo 10 caracteres");
        
        RuleFor(c => c.Identidade)
            .NotEmpty()
            .WithMessage("Identidade não pode ser vazio")
            .MaximumLength(30)
            .WithMessage("Identidade deve ter no máximo 30 caracteres");
        
        RuleFor(c => c.DataIdentidade)
            .NotNull()
            .WithMessage("DataIdentidade não pode ser nulo")
            .NotEmpty()
            .WithMessage("DataIdentidade não pode ser vazio");
        
        RuleFor(c => c.IdEmissor)
            .NotNull()
            .WithMessage("IdEmissor não pode ser nulo")
            .NotEmpty()
            .WithMessage("IdEmissor não pode ser vazio");
        
        RuleFor(c => c.IdNacionalidade)
            .NotNull()
            .WithMessage("IdNacionalidade não pode ser nulo")
            .NotEmpty()
            .WithMessage("IdNacionalidade não pode ser vazio");
        
        RuleFor(c => c.IdProfissaoAtleta)
            .NotNull()
            .WithMessage("IdProfissaoAtleta não pode ser nulo")
            .NotEmpty()
            .WithMessage("IdProfissaoAtleta não pode ser vazio");
        
        RuleFor(c => c.NomePai)
            .MaximumLength(60)
            .WithMessage("NomePai deve ter no máximo 60 caracteres");
        
        RuleFor(c => c.NomeMae)
            .MaximumLength(60)
            .WithMessage("NomeMae deve ter no máximo 60 caracteres");
        
        RuleFor(c => c.IdFaixa)
            .NotNull()
            .WithMessage("IdFaixa não pode ser nulo")
            .NotEmpty()
            .WithMessage("IdFaixa não pode ser vazio");
        
        RuleFor(c => c.IdSexo)
            .NotNull()
            .WithMessage("IdSexo não pode ser nulo")
            .NotEmpty()
            .WithMessage("IdSexo não pode ser vazio");
        
        RuleFor(c => c.IdEstadoCivil)
            .NotNull()
            .WithMessage("IdEstadoCivil não pode ser nulo")
            .NotEmpty()
            .WithMessage("IdEstadoCivil não pode ser vazio");
    }
}