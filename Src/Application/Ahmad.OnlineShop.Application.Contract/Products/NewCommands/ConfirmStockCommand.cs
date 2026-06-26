using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record ConfirmStockCommand(
    long ProductId,
    int  Quantity
) : ICommand<long>;
