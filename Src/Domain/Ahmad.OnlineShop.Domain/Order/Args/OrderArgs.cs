using Ahmad.OnlineShop.Domain.Order.Enums;

namespace Ahmad.OnlineShop.Domain.Order.Args;

public sealed record CreateOrderArg(
    long          Id,
    long          UserId,
    PaymentMethod PaymentMethod
);
