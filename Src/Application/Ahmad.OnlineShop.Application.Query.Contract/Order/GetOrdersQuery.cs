using AhmadBase.Application.Query;
using Ahmad.OnlineShop.Application.Dtos;
using Order.Domain.Enums;

namespace Ahmad.OnlineShop.Application.Query.Queries;

public record GetOrdersQuery(
    int          Page,
    int          PageSize,
    long?        UserId = null,
    OrderStatus? Status = null
) : IQuery<PagedResult<OrderDto>>;
