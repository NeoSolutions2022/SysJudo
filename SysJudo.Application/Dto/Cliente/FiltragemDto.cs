namespace SysJudo.Application.Dto.Cliente;

public class FiltragemDto
{
    public string NomeParametro { get; set; } = null!;
    public int OperacaoId { get; set; }
    public string Valor1 { get; set; } = null!;
    // public DateOnly? Data1 { get; set; }
    // public DateOnly? Data2 { get; set; }
}