namespace SysJudo.Application.Dto.Pais;

public class UpdatePaisDto
{
    public int Id { get; set; }
    public string Descricao { get; set; } = null!;
    public string Sigla3 { get; set; } = null!;
    public string Sigla2 { get; set; } = null!;
    public string? Nacionalidade { get; set; }
}