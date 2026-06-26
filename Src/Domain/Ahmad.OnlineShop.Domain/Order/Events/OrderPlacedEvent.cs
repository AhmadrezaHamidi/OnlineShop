using Ahmad.OnlineShop.Domain.Order.Enums;
using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Order.Events;

public sealed record OrderPlacedEvent(
    long OrderId,
    long UserId,
    decimal TotalAmount,
    PaymentMethod PaymentMethod,
    IReadOnlyList<OrderItemSnapshot> Items
) : IEvent;
