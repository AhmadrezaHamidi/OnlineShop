using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Order.Events;

public sealed record OrderConfirmedEvent(
    long    OrderId,
    long    UserId,
    decimal TotalAmount
) : IEvent;
