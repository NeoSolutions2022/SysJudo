using FluentValidation.Results;
using SysJudo.Domain.Contracts;
using SysJudo.Domain.Validation;

namespace SysJudo.Domain.Entities;

public class Atleta : Entity, IAggregateRoot, ITenant
{
    public string? Foto { get; set; } = null!;
    public string RegistroFederacao { get; set; } = null!;
    public string? RegistroConfederacao { get; set; }
    public string Nome { get; set; } = null!;
    public DateTime DataNascimento { get; set; }
    public DateTime DataFiliacao { get; set; }
    public string Cep { get; set; } = null!;
    public string Endereco { get; set; } = null!;
    public string? Bairro { get; set; }
    public string? Complemento { get; set; }
    public string Telefone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Cpf { get; set; } = null!;
    public string Identidade { get; set; } = null!;
    public DateTime DataIdentidade { get; set; }
    public string? NomePai { get; set; }
    public string? NomeMae { get; set; }
    public string? Anotacoes { get; set; }

    public int IdFaixa { get; set; }
    public int IdSexo { get; set; }
    public int IdEstadoCivil { get; set; }
    public int IdProfissaoAtleta { get; set; }
    public int? IdProfissaoMae { get; set; }
    public int? IdProfissaoPai { get; set; }
    public int IdEmissor { get; set; }
    public int IdNacionalidade { get; set; }
    public int IdCidade { get; set; }
    public int IdEstado { get; set; }
    public int IdPais { get; set; }
    public int IdAgremiacao { get; set; }
    public int ClienteId { get; set; }

    public virtual Faixa Faixa { get; set; } = null!;
    public virtual Sexo Sexo { get; set; } = null!;
    public virtual EstadoCivil EstadoCivil { get; set; } = null!;
    public virtual Profissao Profissao { get; set; } = null!;
    public virtual EmissoresIdentidade EmissoresIdentidade { get; set; } = null!;
    public virtual Nacionalidade Nacionalidade { get; set; } = null!;
    public virtual Cidade Cidade { get; set; } = null!;
    public virtual Estado Estado { get; set; } = null!;
    public virtual Pais Pais { get; set; } = null!;
    public virtual Agremiacao Agremiacao { get; set; } = null!;
    public virtual Cliente Cliente { get; set; } = null!;

    public override bool Validar(out ValidationResult validationResult)
    {
        validationResult = new AtletaValidator().Validate(this);
        return validationResult.IsValid;
    }
}