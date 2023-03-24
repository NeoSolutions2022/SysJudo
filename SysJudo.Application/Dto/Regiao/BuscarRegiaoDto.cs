using SysJudo.Application.Dto.Base;
using SysJudo.Core.Extension;

namespace SysJudo.Application.Dto.Regiao;

public class BuscarRegiaoDto : BuscaPaginadaDto<Domain.Entities.Regiao>
{
    public string? Sigla { get; set; } = null!;
    public string? Cep { get; set; } = null!;
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Pais { get; set; }

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
        
        if (!string.IsNullOrWhiteSpace(Cidade))
        {
            query = query.Where(c => c.Cidade.Contains(Cidade));
        }

        if (!string.IsNullOrWhiteSpace(Estado))
        {
            query = query.Where(c => c.Estado.Contains(Estado));
        }

        if (!string.IsNullOrWhiteSpace(Pais))
        {
            query = query.Where(c => c.Pais.Contains(Pais));
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
                "cidade" => query.OrderBy(c => c.Cidade),
                "estado" => query.OrderBy(c => c.Estado),
                "pais" => query.OrderBy(c => c.Pais),
                "cep" or _ => query.OrderBy(c => c.Cep)
            };
            return;
        }
        
        query = OrdenarPor.ToLower() switch
        {
            "id" => query.OrderByDescending(c => c.Id),
            "cidade" => query.OrderByDescending(c => c.Cidade),
            "estado" => query.OrderByDescending(c => c.Estado),
            "pais" => query.OrderByDescending(c => c.Pais),
            "cep" or _ => query.OrderByDescending(c => c.Cep)
        };
    }
}