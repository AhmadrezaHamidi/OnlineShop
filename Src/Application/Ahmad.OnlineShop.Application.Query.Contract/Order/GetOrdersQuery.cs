using Ahmad.OnlineShop.Domain.Order.Enums;
using AhmadBase.Application.Query;

namespace Ahmad.OnlineShop.Application.Query.Queries;

public record GetOrdersQuery(
    int Page,
    int PageSize,
    long? UserId = null,
    OrderStatus? Status = null
) : IQuery<PagedResult<GetOrderQueryResponse>>;

