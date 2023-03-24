using SysJudo.Application.Dto.Cliente;
using SysJudo.Application.Dto.Regiao;

namespace SysJudo.Application.Dto.Agremiacao;

public class AgremiacaoDto
{
    public int Id { get; set; }
    public string Sigla { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string? Fantasia { get; set; }
    public string Responsavel { get; set; } = null!;
    public string Representante { get; set; } = null!;
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
    public string DocumentosUri { get; set; } = null!;
    public DateOnly? DataCnpj { get; set; }
    public DateOnly? DataAta { get; set; }
    public string? Foto { get; set; }
    public bool AlvaraLocacao { get; set; }
    public bool Estatuto { get; set; }
    public bool ContratoSocial { get; set; }
    public bool DocumentacaoAtualizada { get; set; }
    public int IdRegiao { get; set; }
    public string? Anotacoes { get; set; }
    public string Pais { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public string Cidade { get; set; } = null!;
    
    public virtual RegiaoDto Regiao { get; set; } = null!;
    public virtual ClienteDto Cliente { get; set; } = null!;
}