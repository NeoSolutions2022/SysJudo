using FluentValidation.Results;
using SysJudo.Domain.Contracts;
using SysJudo.Domain.Validation;

namespace SysJudo.Domain.Entities;

public class Agremiacao : Entity, IAggregateRoot, ITenant
{
    public string Sigla { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string? Fantasia { get; set; }
    public string Responsavel { get; set; } = null!;
    public string Representante { get; set; } = null!;
    
    public byte[]? Conteudo { get; set; }
    public string DocumentosUri { get; set; } = null!;
    public DateOnly DataFiliacao { get; set; }
    public DateOnly DataNascimento { get; set; }
    public string Cep { get; set; } = null!;
    public string Endereco { get; set; } = null!;
    public string Bairro { get; set; } = null!;
    public string Complemento { get; set; } = null!;
    public string Telefone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Cnpj { get; set; } = null!;
    public string? InscricaoMunicipal { get; set; }
    public string? InscricaoEstadual { get; set; }
    public DateOnly? DataCnpj { get; set; }
    public DateOnly? DataAta { get; set; }
    public string? Foto { get; set; }
    public bool AlvaraLocacao { get; set; }
    public bool Estatuto { get; set; }
    public bool ContratoSocial { get; set; }
    public bool DocumentacaoAtualizada { get; set; }
    public string Anotacoes { get; set; } = " ";

    public int IdPais { get; set; }
    public int IdCidade { get; set; }
    public int IdEstado { get; set; }
    public int IdRegiao { get; set; }
    public int ClienteId { get; set; }

    public virtual Pais Pais { get; set; } = null!;
    public virtual Estado Estado { get; set; } = null!;
    public virtual Cidade Cidade { get; set; } = null!;
    public virtual Regiao Regiao { get; set; } = null!;
    public virtual Cliente Cliente { get; set; } = null!;
    public virtual List<Atleta> Atletas { get; set; } = new();

    public override bool Validar(out ValidationResult validationResult)
    {
        validationResult = new AgremiacaoValidation().Validate(this);
        return validationResult.IsValid;
    }
}