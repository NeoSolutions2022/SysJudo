using Microsoft.AspNetCore.Http;

namespace SysJudo.Application.Dto.Agremiacao;

public class CadastrarAgremiacaoDto
{
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
    public DateOnly? DataCnpj { get; set; }
    public DateOnly? DataAta { get; set; }
    public IFormFile? Foto { get; set; }
    public IFormFile? AlvaraLocacao { get; set; }
    public IFormFile? Estatuto { get; set; }
    public IFormFile? ContratoSocial { get; set; }
    public IFormFile? DocumentacaoAtualizada { get; set; }
    public string? Anotacoes { get; set; }
    public int IdCidade { get; set; }
    public int IdEstado { get; set; }
    public int IdPais { get; set; }
    public int IdRegiao { get; set; }
}