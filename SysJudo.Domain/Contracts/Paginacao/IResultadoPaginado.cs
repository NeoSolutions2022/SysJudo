namespace SysJudo.Domain.Contracts.Paginacao;

public class IResultadoPaginado<T>
{
    public IList<T> Itens { get; set; }
    public IPaginacao Paginacao { get; set; }
}