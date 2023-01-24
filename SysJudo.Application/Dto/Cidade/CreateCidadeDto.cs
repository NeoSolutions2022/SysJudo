namespace SysJudo.Application.Dto.Cidade;

public class CreateCidadeDto
{
    public string? Sigla { get; set; }
    public string? Descricao { get; set; }
    public int IdPais { get; set; }
    public int IdEstado { get; set; }
}