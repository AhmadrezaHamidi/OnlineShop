using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Contract.Order.Commands;

public record ShipOrderCommand(
    long OrderId
) : ICommand<long>;
