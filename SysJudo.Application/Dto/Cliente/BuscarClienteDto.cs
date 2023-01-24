using SysJudo.Application.Dto.Base;
using SysJudo.Core.Extension;

namespace SysJudo.Application.Dto.Cliente;

public class BuscarClienteDto : BuscaPaginadaDto<Domain.Entities.Cliente>
{
    public string? Sigla { get; set; } = null!;
    public string? Nome { get; set; } = null!;
    public int? IdSistema { get; set; }

    public override void AplicarFiltro(ref IQueryable<Domain.Entities.Cliente> query)
    {
        var expression = MontarExpressao();

        if (!string.IsNullOrWhiteSpace(Sigla))
        {
            query = query.Where(s => s.Sigla.Contains(Sigla));
        }
        
        if (!string.IsNullOrWhiteSpace(Nome))
        {
            query = query.Where(s => s.Sigla.Contains(Nome));
        }
        
        if (IdSistema.HasValue)
        {
            query = query.Where(s => s.IdSistema == IdSistema);
        }

        query = query.Where(expression);
    }

    public override void AplicarOrdenacao(ref IQueryable<Domain.Entities.Cliente> query)
    {
        if (DirecaoOrdenacao.EqualsIgnoreCase("asc"))
        {
            query = OrdenarPor.ToLower() switch
            {
                "id" => query.OrderBy(c => c.Id),
                "nome" => query.OrderBy(c => c.Nome),
                "idsistema" => query.OrderBy(c => c.IdSistema),
                "sigla" or _ => query.OrderBy(c => c.Sigla)
            };
            return;
        }
        
        query = OrdenarPor.ToLower() switch
        {
            "id" => query.OrderByDescending(c => c.Id),
            "nome" => query.OrderByDescending(c => c.Nome),
            "idsistema" => query.OrderByDescending(c => c.IdSistema),
            "sigla" or _ => query.OrderByDescending(c => c.Sigla)
        };
    }
}