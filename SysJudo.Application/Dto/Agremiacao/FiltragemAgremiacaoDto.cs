namespace SysJudo.Application.Dto.Agremiacao;

public class FiltragemAgremiacaoDto
{
    public string NomeParametro { get; set; } = null!;
    public int OperacaoId { get; set; }
    public string? ValorString { get; set; }
    public int? ValorId1 { get; set; }
    public int? ValorId2 { get; set; }
    public DateOnly? DataInicial { get; set; }
    public DateOnly? DataFinal { get; set; }
    public int? OperadorLogico { get; set; }
    public bool OperacoesMatematicas { get; set; }
}