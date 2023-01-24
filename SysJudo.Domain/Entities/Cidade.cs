using FluentValidation.Results;
using SysJudo.Domain.Contracts;
using SysJudo.Domain.Validation;

namespace SysJudo.Domain.Entities;

public class Cidade : Entity, IAggregateRoot, ITenant
{
    public string Sigla { get; set; } = null!;
    public string Descricao { get; set; } = null!;

    public int ClienteId { get; set; }
    public int IdPais { get; set; }
    public int IdEstado { get; set; }
    
    public virtual Pais Pais { get; set; } = null!;
    public virtual Estado Estado { get; set; } = null!;
    public virtual Cliente Cliente { get; set; } = null!;
    public virtual List<Agremiacao> Agremiacoes { get; set; } = new();
    public virtual List<Regiao> Regioes { get; set; } = new();
    public virtual List<Atleta> Atletas { get; set; } = new();

    public override bool Validar(out ValidationResult validationResult)
    {
        validationResult = new CidadeValidator().Validate(this);
        return validationResult.IsValid;
    }
}