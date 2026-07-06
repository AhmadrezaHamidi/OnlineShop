using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Contract.Order.Commands;

public record PlaceOrderCommand(
    long OrderId
) : ICommand<long>;
