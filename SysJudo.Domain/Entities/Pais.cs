using FluentValidation.Results;
using SysJudo.Domain.Contracts;
using SysJudo.Domain.Validation;

namespace SysJudo.Domain.Entities;

public class Pais : Entity, IAggregateRoot, ITenant
{
    public string Descricao { get; set; } = null!;
    public string Sigla3 { get; set; } = null!;
    public string Sigla2 { get; set; } = null!;
    public string? Nacionalidade { get; set; }

    public int ClienteId { get; set; }

     public virtual Cliente Cliente { get; set; } = null!;
     public virtual List<Agremiacao> Agremiacoes { get; set; } = new();
     public virtual List<Estado> Estados { get; set; } = new();
     public virtual List<Cidade> Cidades { get; set; } = new();
     public virtual List<Regiao> Regioes { get; set; } = new();
     public virtual List<Atleta> Atletas { get; set; } = new();


     public override bool Validar(out ValidationResult validationResult)
     {
         validationResult = new PaisValidator().Validate(this);
         return validationResult.IsValid;
     }
}