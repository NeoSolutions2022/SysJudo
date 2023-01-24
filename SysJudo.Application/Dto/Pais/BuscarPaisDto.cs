using SysJudo.Application.Dto.Base;
using SysJudo.Core.Extension;

namespace SysJudo.Application.Dto.Pais;

public class BuscarPaisDto : BuscaPaginadaDto<Domain.Entities.Pais>
{
    public string? Sigla3 { get; set; }
    public string? Sigla2 { get; set; }

    public override void AplicarFiltro(ref IQueryable<Domain.Entities.Pais> query)
    {
        var expression = MontarExpressao();
        
        if (!string.IsNullOrWhiteSpace(Sigla3))
        {
            query = query.Where(s => s.Sigla3.Contains(Sigla3));
        }
        
        if (!string.IsNullOrWhiteSpace(Sigla2))
        {
            query = query.Where(s => s.Sigla2.Contains(Sigla2));
        }

        query = query.Where(expression);
    }

    public override void AplicarOrdenacao(ref IQueryable<Domain.Entities.Pais> query)
    {
        if (DirecaoOrdenacao.EqualsIgnoreCase("asc"))
        {
            query = OrdenarPor.ToLower() switch
            {
                "id" => query.OrderBy(c => c.Id),
                "sigla3" => query.OrderBy(c => c.Sigla3),
                "sigla2" or _ => query.OrderBy(c => c.Sigla2)
            };
            return;
        }
        
        query = OrdenarPor.ToLower() switch
        {
            "id" => query.OrderByDescending(c => c.Id),
            "sigla3" => query.OrderByDescending(c => c.Sigla3),
            "sigla2" or _ => query.OrderByDescending(c => c.Sigla2)
        };
    }
}