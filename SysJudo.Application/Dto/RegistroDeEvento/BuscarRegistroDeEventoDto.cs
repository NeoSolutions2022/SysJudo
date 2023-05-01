using SysJudo.Application.Dto.Base;
using SysJudo.Core.Extension;

namespace SysJudo.Application.Dto.RegistroDeEvento;

public class BuscarRegistroDeEventoDto : BuscaPaginadaDto<Domain.Entities.RegistroDeEvento>
{
    public DateTime? DataHoraEvento { get; set; }
    public int? ComputadorId { get; set; }
    public string? Descricao { get; set; }
    public int? ClienteId { get; set; }
    public int? TipoOperacaoId { get; set; }
    public int? UsuarioId { get; set; }
    public int? FuncaoMenuId{ get; set; }

    public override void AplicarFiltro(ref IQueryable<Domain.Entities.RegistroDeEvento> query)
    {
        var expression = MontarExpressao();
        
        if (DataHoraEvento.HasValue)
        {
            query = query.Where(c => c.DataHoraEvento == DataHoraEvento);
        }
        
        if (ComputadorId.HasValue)
        {
            query = query.Where(c => c.DataHoraEvento == DataHoraEvento);
        }
        
        if (!string.IsNullOrWhiteSpace(Descricao))
        {
            query = query.Where(c => c.Descricao.Contains(Descricao));
        }
        
        if (ClienteId.HasValue)
        {
            query = query.Where(c => c.ClienteId == ClienteId);
        }
        
        if (TipoOperacaoId.HasValue)
        {
            query = query.Where(c => c.TipoOperacaoId == TipoOperacaoId);
        }
        
        if (UsuarioId.HasValue)
        {
            query = query.Where(c => c.UsuarioId == UsuarioId);
        }
        
        if (FuncaoMenuId.HasValue)
        {
            query = query.Where(c => c.FuncaoMenuId == FuncaoMenuId);
        }

        query = query.Where(expression);
    }

    public override void AplicarOrdenacao(ref IQueryable<Domain.Entities.RegistroDeEvento> query)
    {
        if (DirecaoOrdenacao.EqualsIgnoreCase("asc"))
        {
            query = OrdenarPor.ToLower() switch
            {
                "id" => query.OrderBy(c => c.Id),
                "datahoraevento" => query.OrderBy(c => c.DataHoraEvento),
                "computadorId" => query.OrderBy(c => c.ComputadorId),
                "clienteId" => query.OrderBy(c => c.ClienteId),
                "tipoOperacaoId" => query.OrderBy(c => c.TipoOperacaoId),
                "usuarioId" => query.OrderBy(c => c.UsuarioId),
                "funcaoMenuId" => query.OrderBy(c => c.FuncaoMenuId),
                "descricao" or _ => query.OrderBy(c => c.Descricao)
            };
            return;
        }
        
        query = OrdenarPor.ToLower() switch
        {
            "id" => query.OrderByDescending(c => c.Id),
            "datahoraevento" => query.OrderByDescending(c => c.DataHoraEvento),
            "computadorId" => query.OrderByDescending(c => c.ComputadorId),
            "clienteId" => query.OrderByDescending(c => c.ClienteId),
            "tipoOperacaoId" => query.OrderByDescending(c => c.TipoOperacaoId),
            "usuarioId" => query.OrderByDescending(c => c.UsuarioId),
            "funcaoMenuId" => query.OrderByDescending(c => c.FuncaoMenuId),
            "descricao" or _ => query.OrderByDescending(c => c.Descricao)
        };
    }
}