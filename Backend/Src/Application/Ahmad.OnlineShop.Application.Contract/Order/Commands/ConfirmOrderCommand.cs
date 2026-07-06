using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Contract.Order.Commands;

public record ConfirmOrderCommand(
    long OrderId
) : ICommand<long>;
