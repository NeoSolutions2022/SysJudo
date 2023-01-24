namespace SysJudo.Application.Dto.Faixa;

public class UpdateFaixaDto
{
    public int Id { get; set; }
    public string Sigla { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public int MesesCarencia { get; set; }
    public int IdadeMinima { get; set; }
    public int OrdemExibicao { get; set; }
}