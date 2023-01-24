namespace SysJudo.Application.Dto.Sistema;

public class UpdateSistemaDto
{
    public int Id { get; set; }
    public string Sigla { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public string Versao { get; set; } = null!;
}