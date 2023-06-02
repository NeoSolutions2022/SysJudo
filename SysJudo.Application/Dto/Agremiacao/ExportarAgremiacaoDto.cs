
namespace SysJudo.Application.Dto.Agremiacao;

public class ExportarAgremiacaoDto
{
    public bool Nome { get; set; }
    public bool Sigla { get; set; }
    public bool Fantasia { get; set; }
    public bool Responsavel { get; set; }
    public bool Representante { get; set; }
    public bool DataFiliacao { get; set; }
    public bool DataNascimento { get; set; }
    public bool Cep { get; set; }
    public bool Endereco { get; set; }
    public bool Bairro { get; set; }
    public bool Complemento { get; set; }
    public bool Cidade { get; set; }
    public bool Estado { get; set; }
    public bool IdRegiao { get; set; }
    public bool Pais { get; set; }
    public bool Telefone { get; set; }
    public bool Email { get; set; }
    public bool InscricaoMunicipal { get; set; }
    public bool InscricaoEstadual { get; set; }
    public bool Foto { get; set; }
    public bool DataCnpj { get; set; }
    public bool DataAta { get; set; }
    public bool Cnpj { get; set; }
    public bool AlvaraLocacao { get; set; }
    public bool Estatuto { get; set; }
    public bool ContratoSocial { get; set; }
    public bool DocumentacaoAtualizada { get; set; }
    public bool Anotacoes { get; set; }
    public SortByExportarDto? Ordenacao { get; set; }
}

public class SortByExportarDto
{
    public string Propriedade { get; set; }
    public bool Ascendente { get; set; }
}

