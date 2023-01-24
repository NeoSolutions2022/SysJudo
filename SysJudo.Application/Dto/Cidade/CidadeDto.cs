namespace SysJudo.Application.Dto.Cidade;

public class CidadeDto
{
    public int Id { get; set; }
    public string Sigla { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public int IdPais { get; set; }
    public int IdEstado { get; set; }
}