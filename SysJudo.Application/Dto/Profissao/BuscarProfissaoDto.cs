using SysJudo.Application.Dto.Base;
using SysJudo.Core.Extension;

namespace SysJudo.Application.Dto.Profissao;

public class BuscarProfissaoDto : BuscaPaginadaDto<Domain.Entities.Profissao>
{
    public string Sigla { get; set; } = null!;
    public string Descricao { get; set; } = null!;

    public override void AplicarFiltro(ref IQueryable<Domain.Entities.Profissao> query)
    {
        var expression = MontarExpressao();
        
        if (!string.IsNullOrWhiteSpace(Sigla))
        {
            query = query.Where(c => c.Sigla.Contains(Sigla));
        }
        
        if (!string.IsNullOrWhiteSpace(Descricao))
        {
            query = query.Where(c => c.Descricao.Contains(Descricao));
        }

        query = query.Where(expression);
    }

    public override void AplicarOrdenacao(ref IQueryable<Domain.Entities.Profissao> query)
    {
        if (DirecaoOrdenacao.EqualsIgnoreCase("asc"))
        {
            query = OrdenarPor.ToLower() switch
            {
                "id" => query.OrderBy(c => c.Id),
                "descrição" => query.OrderBy(c => c.Descricao),
                "sigla" or _ => query.OrderBy(c => c.Sigla)
            };
            return;
        }
        
        query = OrdenarPor.ToLower() switch
        {
            "id" => query.OrderByDescending(c => c.Id),
            "descrição" => query.OrderBy(c => c.Descricao),
            "sigla" or _ => query.OrderByDescending(c => c.Sigla)
        };
    }
}