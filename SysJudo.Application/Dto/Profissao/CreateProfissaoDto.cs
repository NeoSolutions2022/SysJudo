namespace SysJudo.Application.Dto.Profissao;

public class CreateProfissaoDto
{
    public string Sigla { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public int ClienteId { get; set; }
}