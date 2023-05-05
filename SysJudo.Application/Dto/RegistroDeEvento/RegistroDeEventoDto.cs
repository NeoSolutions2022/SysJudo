namespace SysJudo.Application.Dto.RegistroDeEvento;

public class RegistroDeEventoDto
{
    public int Id { get; set; }
    public DateTime DataHoraEvento { get; set; }
    public string? ComputadorId { get; set; }
    public string Descricao { get; set; } = null!;
    public int ClienteId { get; set; }
    public int TipoOperacaoId { get; set; }
    public int UsuarioId { get; set; }
    public int FuncaoMenuId{ get; set; }
}