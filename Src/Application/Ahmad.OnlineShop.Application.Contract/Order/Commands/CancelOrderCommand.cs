using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Contract.Order.Commands;

public record CancelOrderCommand(
    long   OrderId,
    string Reason
) : ICommand<long>;
