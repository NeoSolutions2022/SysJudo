using SysJudo.Application.Dto.Base;
using SysJudo.Core.Extension;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Dto.GruposDeAcesso;

public class BuscarGrupoAcessoDto : BuscaPaginadaDto<GrupoAcesso>
{
    public string? Nome { get; set; } = null!;
    public bool? Desativado { get; set; }
    
    public override void AplicarFiltro(ref IQueryable<GrupoAcesso> query)
    {
        if (!string.IsNullOrWhiteSpace(Nome))
        {
            query = query.Where(c => c.Nome.Contains(Nome));
        }

        if (Desativado.HasValue)
        {
            query = query.Where(c => c.Desativado == Desativado.Value);
        }
    }

    public override void AplicarOrdenacao(ref IQueryable<GrupoAcesso> query)
    {
        if (DirecaoOrdenacao.EqualsIgnoreCase("desc"))
        {
            query = OrdenarPor.ToLower() switch
            {
                "desativado" => query.OrderByDescending(c => c.Desativado),
                "id" => query.OrderByDescending(c => c.Id),
                "nome" or _ => query.OrderByDescending(c => c.Nome)
            };
            return;
        }

        query = OrdenarPor.ToLower() switch
        {
            "desativado" => query.OrderBy(c => c.Desativado),
            "id" => query.OrderBy(c => c.Id),
            "nome" or _ => query.OrderBy(c => c.Nome)
        };
    }
}