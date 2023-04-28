using FluentValidation.Results;
using SysJudo.Domain.Contracts;
using SysJudo.Domain.Validation;

namespace SysJudo.Domain.Entities;

public class Faixa : Entity, IAggregateRoot, ITenant
{
    public string Sigla { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public int MesesCarencia { get; set; }
    public int IdadeMinima { get; set; }
    public int OrdemExibicao { get; set; }

    public int ClienteId { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;
    public virtual List<Atleta> Atletas { get; set; } = new();

    public override bool Validar(out ValidationResult validationResult)
    {
        validationResult = new FaixaValidator().Validate(this);
        return validationResult.IsValid;
    }
}