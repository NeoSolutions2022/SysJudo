using FluentValidation.Results;
using SysJudo.Domain.Contracts;
using SysJudo.Domain.Entities.EntitiesFiltros;
using SysJudo.Domain.Validation;

namespace SysJudo.Domain.Entities;

public class Cliente : Entity, IAggregateRoot
{
    public string Sigla { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string PastaArquivo { get; set; } = null!;
    
    public int IdSistema { get; set; }

    public virtual Sistema Sistema { get; set; } = null!;
    public virtual List<Faixa> Faixas { get; set; } = new();
    public virtual List<Usuario> Usuarios { get; set; } = new();
    public virtual List<Agremiacao> Agremiacoes { get; set; } = new();
    public virtual List<AgremiacaoFiltro> AgremiacoesFiltro { get; set; } = new();
    public virtual List<Regiao> Regioes { get; set; } = new();
    public virtual List<EmissoresIdentidade> EmissoresIdentidades { get; set; } = new();
    public virtual List<Profissao> Profissoes { get; set; } = new();
    public virtual List<EstadoCivil> EstadosCivis { get; set; } = new();
    public virtual List<Nacionalidade> Nacionalidades { get; set; } = new();
    public virtual List<Atleta> Atletas { get; set; } = new();
    public virtual List<FuncaoMenu> FuncoesMenus { get; set; } = new();
    public virtual List<TipoOperacao> TiposOperacoes { get; set; } = new();
    public virtual List<RegistroDeEvento> RegistroDeEventos { get; set; } = new();

    public override bool Validar(out ValidationResult validationResult)
    {
        validationResult = new ClienteValidator().Validate(this);
        return validationResult.IsValid;
    }
}