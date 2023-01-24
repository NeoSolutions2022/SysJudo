namespace SysJudo.Application.Dto.Sistema;

public class CreateSistemaDto
{
    public string Sigla { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public string Versao { get; set; } = null!;
}