namespace SysJudo.Application.Dto.Estado;

public class UpdadeEstadoDto
{
    public int Id { get; set; }
    public string Sigla { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public int IdPais { get; set; }
}