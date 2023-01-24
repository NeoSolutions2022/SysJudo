namespace SysJudo.Application.Dto.Estado;

public class CreateEstadoDto
{
    public string Sigla { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public int IdPais { get; set; }
}