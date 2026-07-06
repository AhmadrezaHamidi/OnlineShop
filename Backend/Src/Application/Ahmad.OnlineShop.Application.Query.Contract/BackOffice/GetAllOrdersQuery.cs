using Ahmad.OnlineShop.Domain.Order.Enums;
using AhmadBase.Application.Query;

namespace BackOffice.Application.Query.Queries;

/// <summary>ادمین: دیدن همه سفارشات با فیلتر</summary>
public record GetAllOrdersQuery(
    int          Page       = 1,
    int          PageSize   = 20,
    long?        UserId     = null,
    OrderStatus? Status     = null,
    PaymentMethod? Method   = null,
    DateTime?    From       = null,
    DateTime?    To         = null
) : IQuery<PagedResult<AdminOrderQueryResponse>>;

public sealed record AdminOrderQueryResponse(
    long          Id,
    long          UserId,
    OrderStatus   Status,
    decimal       TotalAmount,
    PaymentMethod PaymentMethod,
    DateTime      PlacedAt,
    int           ItemCount,
    bool          HasSuccessfulPayment);
