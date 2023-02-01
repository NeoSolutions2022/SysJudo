namespace SysJudo.Application.Dto.Agremiacao;

public class ExportarAgremiacaoDto
{
    public string ParametroNome = "Nome";
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
    public bool IdCidade { get; set; }
    public bool IdEstado { get; set; }
    public bool IdRegiao { get; set; }
    public bool IdPais { get; set; }
    public bool Telefone { get; set; }
    public bool Email { get; set; }
    public bool Cnpj { get; set; }
}

