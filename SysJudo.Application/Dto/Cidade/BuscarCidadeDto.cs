using SysJudo.Application.Dto.Base;
using SysJudo.Core.Extension;

namespace SysJudo.Application.Dto.Cidade;

public class BuscarCidadeDto : BuscaPaginadaDto<Domain.Entities.Cidade>
{
    public string? Sigla { get; set; }
    public string? Descricao { get; set; }

    public override void AplicarFiltro(ref IQueryable<Domain.Entities.Cidade> query)
    {
        var expression = MontarExpressao();

        if (!string.IsNullOrWhiteSpace(Sigla))
        {
            query = query.Where(c => c.Sigla.Contains(Sigla));
        }
        
        if (!string.IsNullOrWhiteSpace(Descricao))
        {
            query = query.Where(c => c.Sigla.Contains(Descricao));
        }

        query = query.Where(expression);
    }
    
    public override void AplicarOrdenacao(ref IQueryable<Domain.Entities.Cidade> query)
    {
        if (DirecaoOrdenacao.EqualsIgnoreCase("asc"))
        {
            query = OrdenarPor.ToLower() switch
            {
                "id" => query.OrderBy(c => c.Id),
                "descricao" => query.OrderBy(c => c.Descricao),
                "sigla" or _ => query.OrderBy(c => c.Sigla)
            };
            return;
        }
        
        query = OrdenarPor.ToLower() switch
        {
            "id" => query.OrderByDescending(c => c.Id),
            "descricao" => query.OrderByDescending(c => c.Descricao),
            "sigla" or _ => query.OrderByDescending(c => c.Sigla)
        };
    }
}