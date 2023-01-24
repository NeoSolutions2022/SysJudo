using SysJudo.Application.Dto.Base;
using SysJudo.Core.Extension;

namespace SysJudo.Application.Dto.Sistema;

public class BuscarSistemaDto : BuscaPaginadaDto<Domain.Entities.Sistema>
{
    public string? Sigla { get; set; }
    public string? Descricao { get; set; }
    public string? Versao { get; set; }

    public override void AplicarFiltro(ref IQueryable<Domain.Entities.Sistema> query)
    {
        var expression = MontarExpressao();

        if (!string.IsNullOrWhiteSpace(Sigla))
        {
            query = query.Where(s => s.Sigla.Contains(Sigla));
        }
        
        if (!string.IsNullOrWhiteSpace(Descricao))
        {
            query = query.Where(s => s.Sigla.Contains(Descricao));
        }
        
        if (!string.IsNullOrWhiteSpace(Versao))
        {
            query = query.Where(s => s.Sigla.Contains(Versao));
        }

        query = query.Where(expression);
    }

    public override void AplicarOrdenacao(ref IQueryable<Domain.Entities.Sistema> query)
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