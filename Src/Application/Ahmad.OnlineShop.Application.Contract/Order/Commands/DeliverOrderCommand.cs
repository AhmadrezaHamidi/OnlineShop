using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Contract.Order.Commands;

public record DeliverOrderCommand(
    long OrderId
) : ICommand<long>;
