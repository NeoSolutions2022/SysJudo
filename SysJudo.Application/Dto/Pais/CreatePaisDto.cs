namespace SysJudo.Application.Dto.Pais;

public class CreatePaisDto
{
    public string Descricao { get; set; } = null!;
    public string Sigla3 { get; set; } = null!;
    public string Sigla2 { get; set; } = null!;
    public string? Nacionalidade { get; set; }
}