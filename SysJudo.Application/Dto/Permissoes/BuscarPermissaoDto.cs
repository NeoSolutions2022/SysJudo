using SysJudo.Application.Dto.Base;
using SysJudo.Core.Extension;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Dto.Permissoes;

public class BuscarPermissaoDto : BuscaPaginadaDto<Permissao>
{
    public string? Nome { get; set; } = null!;
    public string? Categoria { get; set; } = null!;
    
    public override void AplicarFiltro(ref IQueryable<Permissao> query)
    {
        if (!string.IsNullOrWhiteSpace(Nome))
        {
            query = query.Where(c => c.Nome.Contains(Nome));
        }

        if (!string.IsNullOrWhiteSpace(Categoria))
        {
            query = query.Where(c => c.Categoria.Contains(Categoria));
        }
    }

    public override void AplicarOrdenacao(ref IQueryable<Permissao> query)
    {
        if (DirecaoOrdenacao.EqualsIgnoreCase("desc"))
        {
            query = OrdenarPor.ToLower() switch
            {
                "id" => query.OrderByDescending(c => c.Id),
                "categoria" => query.OrderByDescending(c => c.Categoria),
                "nome" or _ => query.OrderByDescending(c => c.Nome)
            };
            return;
        }

        query = OrdenarPor.ToLower() switch
        {
            "id" => query.OrderBy(c => c.Id),
            "categoria" => query.OrderBy(c => c.Categoria),
            "nome" or _ => query.OrderBy(c => c.Nome)
        };
    }
}