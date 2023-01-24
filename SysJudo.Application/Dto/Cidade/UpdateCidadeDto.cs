namespace SysJudo.Application.Dto.Cidade;

public class UpdateCidadeDto
{
    public int Id { get; set; }
    public string? Sigla { get; set; }
    public string? Descricao { get; set; }
    public int IdPais { get; set; }
    public int IdEstado { get; set; }
}