using Ahmad.OnlineShop.Domain.Products.Enums;
using AhmadBase.Application.Query;

namespace Ahmad.OnlineShop.Application.Query.Queries;

public record GetProductsQuery(
    int            Page,
    int            PageSize,
    string?        Search,
    long?          CategoryId,
    ProductStatus? Status
) : IQuery<QueryPagedResult<GetProductQueryResponse>>;

public record QueryPagedResult<T>(
    List<T> Items,
    int     TotalCount,
    int     Page,
    int     PageSize)
{
    public int  TotalPages      => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    public bool HasNextPage     => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
