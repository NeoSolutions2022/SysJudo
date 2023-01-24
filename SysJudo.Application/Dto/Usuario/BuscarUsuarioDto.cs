using SysJudo.Application.Dto.Base;
using SysJudo.Core.Extension;

namespace SysJudo.Application.Dto.Usuario;

public class BuscarUsuarioDto : BuscaPaginadaDto<Domain.Entities.Usuario>
{
    public string? Nome { get; set; }
    public bool? Inadiplente { get; set; }

    public override void AplicarFiltro(ref IQueryable<Domain.Entities.Usuario> query)
    {
        var expression = MontarExpressao();
        
        if (!string.IsNullOrWhiteSpace(Nome))
        {
            query = query.Where(c => c.Nome.Contains(Nome));
        }
        
        if (Inadiplente.HasValue)
        {
            query = query.Where(c => c.Inadiplente == Inadiplente);
        }

        query = query.Where(expression);
    }

    public override void AplicarOrdenacao(ref IQueryable<Domain.Entities.Usuario> query)
    {
        if (DirecaoOrdenacao.EqualsIgnoreCase("asc"))
        {
            query = OrdenarPor.ToLower() switch
            {
                "id" => query.OrderBy(c => c.Id),
                "inadiplente" => query.OrderBy(c => c.Inadiplente),
                "nome" or _ => query.OrderBy(c => c.Nome)
            };
            return;
        }
        
        query = OrdenarPor.ToLower() switch
        {
            "id" => query.OrderByDescending(c => c.Id),
            "inadiplente" => query.OrderByDescending(c => c.Inadiplente),
            "nome" or _ => query.OrderByDescending(c => c.Nome)
        };
    }
}