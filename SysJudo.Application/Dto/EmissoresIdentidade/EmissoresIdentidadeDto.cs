namespace SysJudo.Application.Dto.EmissoresIdentidade;

public class EmissoresIdentidadeDto
{
    public int Id { get; set; }
    public string Sigla { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public int ClienteId { get; set; }
}