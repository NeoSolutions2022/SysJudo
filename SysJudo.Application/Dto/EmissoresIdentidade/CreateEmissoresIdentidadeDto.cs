namespace SysJudo.Application.Dto.EmissoresIdentidade;

public class CreateEmissoresIdentidadeDto
{
    public string Sigla { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public int ClienteId { get; set; }
}