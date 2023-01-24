namespace SysJudo.Application.Dto.Nacionalidade;

public class CreateNacionalidadeDto
{
    public string Sigla { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public int ClienteId { get; set; }
}