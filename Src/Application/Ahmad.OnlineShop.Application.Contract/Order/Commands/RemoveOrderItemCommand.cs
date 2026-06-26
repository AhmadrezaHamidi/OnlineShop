using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Contract.Order.Commands;

public record RemoveOrderItemCommand(
    long OrderId,
    long ItemId
) : ICommand<long>;
