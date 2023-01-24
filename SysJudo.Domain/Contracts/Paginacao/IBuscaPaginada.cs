using System.Linq.Expressions;

namespace SysJudo.Domain.Contracts.Paginacao;

public interface IBuscaPaginada<T> where T : IEntity
{
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public string OrdenarPor { get; set; }
    public string DirecaoOrdenacao { get; set; }

    public void AplicarFiltro(ref IQueryable<T> queryable);
    public void AplicarOrdenacao(ref IQueryable<T> queryable);
    public Expression<Func<T, bool>> MontarExpressao();
}