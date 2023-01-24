using FluentValidation.Results;
using SysJudo.Domain.Contracts;
using SysJudo.Domain.Validation;

namespace SysJudo.Domain.Entities;

public class Cliente : Entity, IAggregateRoot
{
    public string Sigla { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string PastaArquivo { get; set; } = null!;
    
    public int IdSistema { get; set; }

    public virtual Sistema Sistema { get; set; } = null!;
    public virtual List<Cidade> Cidades { get; set; } = new();
    public virtual List<Pais> Paises { get; set; } = new();
    public virtual List<Estado> Estados { get; set; } = new();
    public virtual List<Faixa> Faixas { get; set; } = new();
    public virtual List<Usuario> Usuarios { get; set; } = new();
    public virtual List<Agremiacao> Agremiacoes { get; set; } = new();
    public virtual List<Regiao> Regioes { get; set; } = new();
    public virtual List<EmissoresIdentidade> EmissoresIdentidades { get; set; } = new();
    public virtual List<Profissao> Profissoes { get; set; } = new();
    public virtual List<EstadoCivil> EstadosCivis { get; set; } = new();
    public virtual List<Nacionalidade> Nacionalidades { get; set; } = new();
    public virtual List<Atleta> Atletas { get; set; } = new();


    public override bool Validar(out ValidationResult validationResult)
    {
        validationResult = new ClienteValidator().Validate(this);
        return validationResult.IsValid;
    }
}