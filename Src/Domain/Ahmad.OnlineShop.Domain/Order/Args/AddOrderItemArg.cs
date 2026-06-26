namespace Ahmad.OnlineShop.Domain.Order.Args;

public sealed record AddOrderItemArg(
    long ItemId,
    long ProductId,
    int Quantity,
    decimal UnitPrice
);
