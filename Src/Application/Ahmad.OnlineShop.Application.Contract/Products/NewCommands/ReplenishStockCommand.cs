using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record ReplenishStockCommand(
    long ProductId,
    int  Quantity
) : ICommand<long>;
