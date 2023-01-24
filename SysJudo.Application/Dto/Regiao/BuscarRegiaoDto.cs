using SysJudo.Application.Dto.Base;
using SysJudo.Core.Extension;

namespace SysJudo.Application.Dto.Regiao;

public class BuscarRegiaoDto : BuscaPaginadaDto<Domain.Entities.Regiao>
{
    public string? Sigla { get; set; } = null!;
    public string? Cep { get; set; } = null!;
    public int? IdCidade { get; set; }
    public int? IdEstado { get; set; }
    public int? IdPais { get; set; }

    public override void AplicarFiltro(ref IQueryable<Domain.Entities.Regiao> query)
    {
        var expression = MontarExpressao();
        
        if (!string.IsNullOrWhiteSpace(Sigla))
        {
            query = query.Where(s => s.Sigla.Contains(Sigla));
        }

        if (!string.IsNullOrWhiteSpace(Cep))
        {
            query = query.Where(s => s.Cep.Contains(Cep));
        }
        
        if (IdCidade.HasValue)
        {
            query = query.Where(s => s.IdCidade == IdCidade);
        }
        
        if (IdEstado.HasValue)
        {
            query = query.Where(s => s.IdEstado == IdEstado);
        }
        
        if (IdPais.HasValue)
        {
            query = query.Where(s => s.IdPais == IdPais);
        }

        query = query.Where(expression);
    }

    public override void AplicarOrdenacao(ref IQueryable<Domain.Entities.Regiao> query)
    {
        if (DirecaoOrdenacao.EqualsIgnoreCase("asc"))
        {
            query = OrdenarPor.ToLower() switch
            {
                "id" => query.OrderBy(c => c.Id),
                "idcidade" => query.OrderBy(c => c.IdCidade),
                "idestado" => query.OrderBy(c => c.IdEstado),
                "idpais" => query.OrderBy(c => c.IdPais),
                "cep" or _ => query.OrderBy(c => c.Cep)
            };
            return;
        }
        
        query = OrdenarPor.ToLower() switch
        {
            "id" => query.OrderByDescending(c => c.Id),
            "idcidade" => query.OrderByDescending(c => c.IdCidade),
            "idestado" => query.OrderByDescending(c => c.IdEstado),
            "idpais" => query.OrderByDescending(c => c.IdPais),
            "cep" or _ => query.OrderByDescending(c => c.Cep)
        };
    }
}