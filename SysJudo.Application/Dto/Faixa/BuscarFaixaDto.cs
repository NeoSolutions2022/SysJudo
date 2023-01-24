using SysJudo.Application.Dto.Base;
using SysJudo.Core.Extension;

namespace SysJudo.Application.Dto.Faixa;

public class BuscarFaixaDto : BuscaPaginadaDto<Domain.Entities.Faixa>
{
    public string? Sigla { get; set; } = null!;
    public string? Descricao { get; set; } = null!;
    public int? MesesCarencia { get; set; }
    public int? IdadeMinima { get; set; }
    public int? OrdemExibicao { get; set; }

    public override void AplicarFiltro(ref IQueryable<Domain.Entities.Faixa> query)
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
        
        if (MesesCarencia.HasValue)
        {
            query = query.Where(s => s.MesesCarencia == MesesCarencia);
        }
        
        if (OrdemExibicao.HasValue)
        {
            query = query.Where(s => s.OrdemExibicao == OrdemExibicao);
        }
        
        if (IdadeMinima.HasValue)
        {
            query = query.Where(c => c.IdadeMinima == IdadeMinima.Value);
        }

        query = query.Where(expression);
    }

    public override void AplicarOrdenacao(ref IQueryable<Domain.Entities.Faixa> query)
    {
        if (DirecaoOrdenacao.EqualsIgnoreCase("asc"))
        {
            query = OrdenarPor.ToLower() switch
            {
                "id" => query.OrderBy(c => c.Id),
                "idademinima" => query.OrderBy(c => c.IdadeMinima),
                "ordemexibicao" => query.OrderBy(c => c.OrdemExibicao),
                "mesescarencia" or _ => query.OrderBy(c => c.MesesCarencia)
            };
            return;
        }
        
        query = OrdenarPor.ToLower() switch
        {
            "id" => query.OrderByDescending(c => c.Id),
            "idademinima" => query.OrderBy(c => c.IdadeMinima),
            "ordemexibicao" => query.OrderBy(c => c.OrdemExibicao),
            "mesescarencia" or _ => query.OrderByDescending(c => c.MesesCarencia)
        };
    }
}