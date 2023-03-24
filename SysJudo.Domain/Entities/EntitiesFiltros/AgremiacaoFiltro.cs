using SysJudo.Domain.Contracts;

namespace SysJudo.Domain.Entities.EntitiesFiltros;

public class AgremiacaoFiltro : EntityFiltro, IAggregateRoot, ITenant
{
    public int Identificador { get; set; }
    public string Sigla { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string? Fantasia { get; set; }
    public string Responsavel { get; set; } = null!;
    public string Representante { get; set; } = null!;

    public byte[]? Conteudo { get; set; }
    public DateOnly DataFiliacao { get; set; }
    public DateOnly DataNascimento { get; set; }
    public string Cep { get; set; } = null!;
    public string Endereco { get; set; } = null!;
    public string Bairro { get; set; } = null!;
    public string? Complemento { get; set; }
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
    public string Anotacoes { get;set;}= " ";
    public string Cidade { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public string RegiaoNome { get; set; } = null!;
    public string Pais { get; set; } = null!;
    public int IdRegiao { get; set; }
    public int ClienteId { get; set; }
}