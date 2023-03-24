using SysJudo.Domain.Contracts;
using SysJudo.Domain.Entities.EntitiesFiltros;

namespace SysJudo.Domain.Entities;

public class Regiao : Entity, IAggregateRoot, ITenant
{
    public string Sigla { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public string? Responsavel { get; set; }
    public string Cep { get; set; } = null!;
    public string Endereco { get; set; } = null!;
    public string? Bairro { get; set; }
    public string? Complemento { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Anotacoes { get; set; }

    public int ClienteId { get; set; }
    public string Cidade { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public string Pais { get; set; } = null!;
    
    public virtual List<Agremiacao> Agremiacoes { get; set; } = new();
    public virtual List<AgremiacaoFiltro> AgremiacoesFiltro { get; set; } = new();
    public virtual Cliente Cliente { get; set; } = null!;
}