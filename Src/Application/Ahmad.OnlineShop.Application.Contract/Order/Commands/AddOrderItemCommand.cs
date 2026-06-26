using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Contract.Order.Commands;

public record AddOrderItemCommand(
    long    OrderId,
    long    ProductId,
    int     Quantity,
    decimal UnitPrice
) : ICommand<long>;
