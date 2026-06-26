using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record ReserveStockCommand(
    long ProductId,
    int  Quantity
) : ICommand<long>;
