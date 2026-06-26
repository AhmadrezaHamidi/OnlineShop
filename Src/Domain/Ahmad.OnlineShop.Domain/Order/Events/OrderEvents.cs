using Ahmad.OnlineShop.Domain.Order.Enums;
using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Order.Events;

public sealed record OrderShippedEvent(
    long OrderId,
    long UserId
) : IEvent;

public sealed record OrderDeliveredEvent(
    long OrderId,
    long UserId
) : IEvent;

public sealed record OrderCancelledEvent(
    long                             OrderId,
    long                             UserId,
    string                           Reason,
    IReadOnlyList<OrderItemSnapshot> Items
) : IEvent;

public sealed record PaymentRecordedEvent(
    long          PaymentId,
    long          OrderId,
    decimal       Amount,
    PaymentStatus Status,
    string?       Provider
) : IEvent;

public sealed record OrderItemSnapshot(
    long    ProductId,
    int     Quantity,
    decimal UnitPrice
);
