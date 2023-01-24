using SysJudo.Domain.Contracts;
using SysJudo.Domain.Contracts.Paginacao;

namespace SysJudo.Application.Dto.Base;

public class PagedDto<T> : IResultadoPaginado<T>, IViewModel
{
    public IList<T> Itens { get; set; }
    public IPaginacao Paginacao { get; set; }

    public PagedDto()
    {
        Itens = new List<T>();
        Paginacao = new PaginacaoDto();
    }
}