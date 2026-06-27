using Ahmad.OnlineShop.Domain.Products.Enums;
using AhmadBase.Application.Query;

namespace Ahmad.OnlineShop.Application.Query.Queries;

public record GetProductsQuery(
    int            Page,
    int            PageSize,
    string?        Search,
    long?          CategoryId,
    ProductStatus? Status
) : IQuery<PagedResult<GetProductQueryResponse>>;
