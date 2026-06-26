using Order.Domain.Enums;

namespace Ahmad.OnlineShop.Application.Dtos;

public sealed record OrderDto(
    long                   Id,
    long                   UserId,
    OrderStatus            Status,
    decimal                TotalAmount,
    PaymentMethod          PaymentMethod,
    DateTime               PlacedAt,
    List<OrderItemDto>     Items,
    List<PaymentDto>       Payments
);
