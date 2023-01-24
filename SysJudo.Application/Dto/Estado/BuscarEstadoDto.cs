using Hangfire.Storage.Monitoring;
using SysJudo.Application.Dto.Base;
using SysJudo.Core.Extension;

namespace SysJudo.Application.Dto.Estado;

public class BuscarEstadoDto : BuscaPaginadaDto<Domain.Entities.Estado>
{
    public string? Sigla { get; set; }

    public override void AplicarFiltro(ref IQueryable<Domain.Entities.Estado> query)
    {
        var expression = MontarExpressao();

        if (!string.IsNullOrWhiteSpace(Sigla))
        {
            query = query.Where(e => e.Sigla.Contains(Sigla));
        }

        query = query.Where(expression);
    }

    public override void AplicarOrdenacao(ref IQueryable<Domain.Entities.Estado> query)
    {
        if (DirecaoOrdenacao.EqualsIgnoreCase("asc"))
        {
            query = OrdenarPor.ToLower() switch
            {
                "id" => query.OrderBy(c => c.Id),
                "sigla" or _ => query.OrderBy(c => c.Sigla)
            };
            return;
        }
        
        query = OrdenarPor.ToLower() switch
        {
            "id" => query.OrderByDescending(c => c.Id),
            "sigla" or _ => query.OrderByDescending(c => c.Sigla)
        };
    }
}