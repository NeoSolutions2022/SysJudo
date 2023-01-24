namespace SysJudo.Application.Dto.Cliente;

public class CreateClienteDto
{
    public string Sigla { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string PastaArquivo { get; set; } = null!;
    public int IdSistema { get; set; }
}