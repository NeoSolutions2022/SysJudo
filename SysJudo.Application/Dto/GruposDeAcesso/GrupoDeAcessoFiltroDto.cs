namespace SysJudo.Application.Dto.GruposDeAcesso;

public class GrupoDeAcessoFiltroDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public bool Administrador { get; set; }
    public bool Desativado { get; set; }
}