using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record ReleaseStockCommand(
    long ProductId,
    int  Quantity
) : ICommand<long>;
