using AhmadBase.Application.Query;
using Ahmad.OnlineShop.Application.Dtos;
using Ahmad.OnlineShop.Domain.Enums;

namespace Ahmad.OnlineShop.Application.Query.Queries;

public record GetProductsQuery(
    int            Page,
    int            PageSize,
    string?        Search,
    long?          CategoryId,
    ProductStatus? Status
) : IQuery<PagedResult<ProductDto>>;
